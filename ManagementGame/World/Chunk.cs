using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Point = Microsoft.Xna.Framework.Point;

namespace ManagementGame.World
{
    class Chunk : GameObject
    {
        public const int Size = 16;
        public const int TextureMapSize = Size * 2;
        public const int TextureMapSamplingSize = Size + 2;
        public bool IsActive = true;

        protected Tile[,] Tiles;
        public List<Entity> Entities { get; private set; }

        public Color debugColor;

        private Subject<EntityTransfer> transferSubject = new Subject<EntityTransfer>();
        public IObservable<EntityTransfer> EntityTransfer => transferSubject.AsObservable<EntityTransfer>();

        public int ChunkX { get; private set; }
        public int ChunkY { get; private set; }

        private Effect tilingEffect;
        private Texture2D solidMap;

        private Texture2D lightMap;
        private ChunkManager chunkManager;

        private Texture2D mask;

        public Chunk(int x, int y, Tile[,] tiles, ChunkManager chunkManager)
        {
            ChunkX = x;
            ChunkY = y;
            X = x * Chunk.Size * Tile.GridSize;
            Y = y * Chunk.Size * Tile.GridSize;
            this.Tiles = tiles;
            this.chunkManager = chunkManager;
            Entities = new List<Entity>();
            CollisionRectangleSize = new Point(Chunk.Size * Tile.GridSize);
            DrawRectangleSize = new Point(Chunk.Size * Tile.GridSize);
            Texture = ContentLoader.GetTexture2D("Grass");
            mask = ContentLoader.GetTexture2D("Mask");
            debugColor = Debug.GenerateRandomColor();
            MakeSolidMap();
        }

        public Chunk(ChunkData data)
        {
            //X = data.X;
            //Y = data.Y;
            //tiles = new Tile[Size, Size];
            //entities = new List<Entity>();
            //for (int y = 0; y < Size; y++)
            //{
            //    for (int x = 0; x < Size; x++)
            //    {
            //        int i = y * Size + x;
            //        string tileName = data.TileMappings[data.TileIds[i]];
            //        tiles[x, y] = new Tile(x, y, tileName);
            //    }
            //}
        }

        private void MakeSolidMap()
        {
            solidMap = new Texture2D(ManagementGame.CurrentGraphicsDevice, Size, Size);
            Color[] pixels = new Color[Size * Size];

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int i = y * Size + x;
                    if (Tiles[x, y].IsSolid)
                    {
                        pixels[i] = new Color(1f, 1f, 1f, 1f);
                    }
                    else
                    {
                        pixels[i] = new Color(0, 0, 0, 0);
                    }
                }
            }

            solidMap.SetData(pixels);
        }

        private void SpreadLight(Tile start)
        { 
            List<Tile> explored = new List<Tile>();
            Queue<Tile> bfsQueue = new Queue<Tile>();
            bfsQueue.Enqueue(start);
            while(bfsQueue.Count > 0)
            {
                var curr = bfsQueue.Dequeue();

                if (curr.LightLevel == 0)
                {
                    continue;
                }

                var tiles = GetAdjacentTiles(curr);
                foreach (var tile in tiles)
                {
                    if (!explored.Contains(tile) && tile.LightLevel < curr.LightLevel) {
                        tile.SetLightLevel(curr.LightLevel - Tile.LightFalloff);
                        bfsQueue.Enqueue(tile);
                        explored.Add(tile);
                    }
                }
            }
        }

        public void MakeLightMap()
        {
            Console.WriteLine($"Making light map for {ChunkX}, {ChunkY}");
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (Tiles[x, y].IsLightSource)
                    {
                        SpreadLight(Tiles[x, y]);
                    }
                }
            }


            lightMap = new Texture2D(ManagementGame.CurrentGraphicsDevice, TextureMapSize, TextureMapSize);
            Color[] pixels = new Color[TextureMapSize * TextureMapSize];

            for (int y = 0; y < TextureMapSamplingSize; y++)
            {
                for (int x = 0; x < TextureMapSamplingSize; x++)
                {
                    float worldX = X + (x - 1) * Tile.GridSize;
                    float worldY = Y + (y - 1) * Tile.GridSize;

                    int i = y * TextureMapSize + x;
                    var tile = chunkManager.GetTileAt(worldX, worldY);
                    float light = 0;
                    if (tile != null)
                    {
                        light = ((float)tile.LightLevel / Tile.MaxLightLevel);
                    }
                    pixels[i] = new Color(light, light, light, 1f);
                }
            }

            lightMap.SetData(pixels);
        }

        public void TransferEntities()
        {
            // Transfer entities that are not inside the chunk.
            foreach (var entity in Entities.ToList())
            {
                if (!CollisionRectangle.Intersects(entity.CollisionRectangle))
                {
                    transferSubject.OnNext(new EntityTransfer()
                    {
                        OldChunk = this,
                        Entity = entity,
                    });
                    Entities.Remove(entity);
                }
            }
        }

        public void Update(GameTime gameTime, ChunkManager chunkManager)
        {
            foreach (var entity in Entities)
            {
                PhysicsEngine.CalculateVelocity(entity, gameTime);

                if (CollisionRectangle.Contains(entity.CollisionRectangle))
                {
                    PhysicsEngine.ApplyVelocityY(entity);
                    PhysicsEngine.ResolveTerrainCollisionsY(entity, Tiles);
                    PhysicsEngine.ApplyVelocityX(entity);
                    PhysicsEngine.ResolveTerrainCollisionsX(entity, Tiles);
                }
                // If not complete inside, do collision checks agaisnt neighbors as well.
                else
                {
                    var chunks = chunkManager.GetNeighborsAndSelf(this);
                    PhysicsEngine.ApplyVelocityY(entity);
                    foreach (var chunk in chunks)
                    {
                        PhysicsEngine.ResolveTerrainCollisionsY(entity, chunk.Tiles);
                    }
                    PhysicsEngine.ApplyVelocityX(entity);
                    foreach (var chunk in chunks)
                    {
                        PhysicsEngine.ResolveTerrainCollisionsX(entity, chunk.Tiles);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {



            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);
            tilingEffect = ContentLoader.GetShader("tiling");
            //tilingEffect.Parameters["SpriteTexture1"].SetValue(ContentLoader.GetTexture2D("Grass"));
            tilingEffect.Parameters["ChunkX"]?.SetValue(ChunkX);
            tilingEffect.Parameters["ChunkY"]?.SetValue(ChunkY);
            tilingEffect.Parameters["ChunkSize"]?.SetValue(Size);
            tilingEffect.Parameters["GridSize"]?.SetValue(Tile.GridSize);
            tilingEffect.Parameters["SolidTileTexture"]?.SetValue(solidMap);
            tilingEffect.Parameters["Mask"]?.SetValue(mask);
            tilingEffect.Parameters["LightMap"]?.SetValue(lightMap);
            tilingEffect.Parameters["LightMapSize"]?.SetValue(TextureMapSize);
            tilingEffect.Parameters["ViewMatrix"]?.SetValue(camera.GetViewMatrix());
            //tilingEffect.Parameters["WorldMatrix"].SetValue(camera.GetWorldMatrix());
            tilingEffect.Parameters["ProjectionMatrix"]?.SetValue(camera.GetProjectionMatrix());
            tilingEffect.CurrentTechnique.Passes[0].Apply();
            foreach (var tile in Tiles)
            {
                tile.Draw(spriteBatch);
            }

            foreach (var entity in Entities)
            {
                entity.Draw(spriteBatch);
            }
            //spriteBatch.Draw(solidTiles, CollisionRectangle, debugColor);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp, null, null, null, camera.GetViewMatrix());
            //spriteBatch.Draw(lightMap, CollisionRectangle, new Rectangle(1, 1, 16, 16), debugColor * .5f);
            //spriteBatch.Draw(ContentLoader.DebugTexture, CollisionRectangle, debugColor * .5f);
            foreach (var tile in Tiles)
            {
                tile.DebugDraw(spriteBatch);
            }
            spriteBatch.End();

        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        public Tile GetTile(int x, int y)
        {
            return Tiles[x, y];
        }

        public List<Tile> GetAdjacentTiles(Tile tile)
        {
            List<Tile> surrounding = new List<Tile>();
            surrounding.Add(chunkManager.GetTileAt(tile.X + Tile.GridSize, tile.Y));
            surrounding.Add(chunkManager.GetTileAt(tile.X - Tile.GridSize, tile.Y));
            surrounding.Add(chunkManager.GetTileAt(tile.X, tile.Y + Tile.GridSize));
            surrounding.Add(chunkManager.GetTileAt(tile.X, tile.Y - Tile.GridSize));
            return surrounding.Where(x => x != null).ToList();
        }
    }

    struct EntityTransfer
    {
        public Chunk OldChunk;
        public Entity Entity;
    }
}

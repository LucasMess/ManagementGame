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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.World
{
    class Chunk
    {
        public const int Size = 16;
        public const int TextureMapSize = Size * 2;
        public const int TextureMapSamplingSize = Size + 2;
        public bool IsActive = true;

        protected Tile[,] Tiles;
        public List<Entity> Entities { get; private set; }
        private Texture2D Texture;
        public Rectangle CollisionRectangle;

        public Color debugColor = Debug.GenerateRandomColor();

        private Subject<EntityTransfer> transferSubject = new Subject<EntityTransfer>();
        public IObservable<EntityTransfer> EntityTransfer => transferSubject.AsObservable<EntityTransfer>();

        public int X { get; private set; }
        public int Y { get; private set; }

        private Effect tilingEffect;
        private Texture2D solidMap;

        private Texture2D lightMap;
        private ChunkManager chunkManager;


        public Chunk(int x, int y, Tile[,] tiles, ChunkManager chunkManager)
        {
            X = x;
            Y = y;
            this.Tiles = tiles;
            this.chunkManager = chunkManager;
            Entities = new List<Entity>();
            CollisionRectangle = new Rectangle(x * Chunk.Size * Tile.Size, y * Chunk.Size * Tile.Size, Chunk.Size * Tile.Size, Chunk.Size * Tile.Size);
            Texture = ContentLoader.GetTexture2D("Grass");
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

                var tiles = GetSurroundingTiles(curr);
                foreach (var tile in tiles)
                {
                    if (!explored.Contains(tile) && tile.LightLevel < curr.LightLevel) {
                        tile.SetLightLevel(curr.LightLevel - 2);
                        bfsQueue.Enqueue(tile);
                        explored.Add(tile);
                    }
                }
            }
        }

        public void MakeLightMap()
        {
            Console.WriteLine($"Making light map for {X}, {Y}");
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
                    float worldX = (X * Tile.Size * Size) + (x - 1) * Tile.Size;
                    float worldY = (Y * Tile.Size * Size) + (y - 1) * Tile.Size;

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
                    debugColor = Debug.GenerateRandomColor();
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
                        chunk.debugColor = Debug.GenerateRandomColor();
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



            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null, null);
            tilingEffect = ContentLoader.GetShader("tiling");
            //tilingEffect.Parameters["SpriteTexture1"].SetValue(ContentLoader.GetTexture2D("Grass"));
            tilingEffect.Parameters["ChunkX"].SetValue(X);
            tilingEffect.Parameters["ChunkY"].SetValue(Y);
            tilingEffect.Parameters["ChunkSize"].SetValue(Size);
            tilingEffect.Parameters["TileSize"].SetValue(Tile.Size);
            tilingEffect.Parameters["SolidTileTexture"].SetValue(solidMap);
            tilingEffect.Parameters["LightMap"].SetValue(lightMap);
            tilingEffect.Parameters["LightMapSize"].SetValue(TextureMapSize);
            tilingEffect.Parameters["ViewMatrix"].SetValue(camera.GetViewMatrix());
            //tilingEffect.Parameters["WorldMatrix"].SetValue(camera.GetWorldMatrix());
            tilingEffect.Parameters["ProjectionMatrix"].SetValue(camera.GetProjectionMatrix());
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

            //spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null, null, camera.GetViewMatrix());
            //spriteBatch.Draw(solidTiles, CollisionRectangle, Color.Red);
            //spriteBatch.End();

        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        public Tile GetTile(int x, int y)
        {
            return Tiles[x, y];
        }

        public List<Tile> GetSurroundingTiles(Tile tile)
        {
            List<Tile> surrounding = new List<Tile>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var tilePos = tile.Position + new Vector2(i, j) * Tile.Size - new Vector2(Tile.Size, Tile.Size);
                    var other = chunkManager.GetTileAt(tilePos.X, tilePos.Y);
                    if (other != null)
                    {
                        surrounding.Add(other);
                    }
                }
            }
            return surrounding;
        }
    }

    struct EntityTransfer
    {
        public Chunk OldChunk;
        public Entity Entity;
    }
}

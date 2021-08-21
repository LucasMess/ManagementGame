using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.Tasks;
using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.World
{
    class GameWorld
    {
        bool pressed = false;

        public const int MapHeightInChunks = 5;
        public const int MaxHeightInTiles = MapHeightInChunks * Chunk.Size;

        public const float GravitationalForce = 10;
        public const float FrictionCoeffAir = 5f;

        public static Random Random = new Random();

        private ChunkManager chunkManager;

        public Player player;

        Actor actor;

        public GameWorld()
        {
            player = new Player();
            chunkManager = new ChunkManager("test");
            SpawnEntity(player, 0, 0);

            actor = new Actor(this);
            SpawnEntity(actor, 300, 200);

            //for (int i = 0; i < 100; i++)
            //{
            //    int x = Random.Next(0, 200);
            //    int y = Random.Next(0, 200);
            //    SpawnEntity(new Ball(x, y), x, y);
            //}
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            Vector2 mousePos = Mouse.GetState().Position.ToVector2() - camera.Position;

            chunkManager.LoadChunksAroundCamera(camera);
            foreach (var chunk in chunkManager.GetChunks().ToList())
            {
                chunk.TransferEntities();
            }
            foreach (var chunk in chunkManager.GetChunks())
            {
                chunk.Update(gameTime, chunkManager);
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed && !pressed)
            {
                pressed = true;
                actor.GiveTask(new MoveTask(mousePos));
            }
            if (Mouse.GetState().RightButton == ButtonState.Released)
            {
                pressed = false;
            }

 

            player.Position = mousePos;
        }
        

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (var chunk in chunkManager.GetChunks())
            {
                
                chunk.Draw(spriteBatch, camera);
            }
        }

        public void SpawnEntity(Entity entity, int x, int y)
        {
            entity.X = x;
            entity.Y = y;
            int chunkX = x / Chunk.Size / Tile.GridSize;
            int chunkY = y / Chunk.Size / Tile.GridSize;
            chunkManager.GetChunk(chunkX, chunkY).AddEntity(entity);
        }

        public Tile GetTileAt(float x, float y)
        {
            return chunkManager.GetTileAt(x, y);
        }
        

        public List<Vector2> Pathfind(Vector2 source, Vector2 destination)
        {
            Point dest = (destination / Tile.GridSize).ToPoint();

            Queue<Tile> frontier = new Queue<Tile>();
            Tile start = chunkManager.GetTileAt(source.X, source.Y);
            Tile lastTile = null;
            frontier.Enqueue(start);

            Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
            cameFrom.Add(start, lastTile);

            List<Vector2> path = new List<Vector2>();

            Console.WriteLine((start.Position / Tile.GridSize).ToPoint() + " -> " + dest);

            while (frontier.Count != 0)
            {
                Tile tile = frontier.Dequeue();
                //tile.isPath = true;
                lastTile = tile;


                if ((tile.Position / Tile.GridSize).ToPoint() == dest)
                {
                    path.Add(tile.Position);
                    Tile t = cameFrom[tile];
                    while (t != null)
                    {
                        path.Add(t.Position);
                        t = cameFrom[t];
                    }
                    path.Reverse();
                    return path;
                }

                var neighbors = GetNeighbors(tile, destination);
                foreach (var n in neighbors)
                {
                    if (n != null && !n.IsSolid && !cameFrom.ContainsKey(n))
                    {
                        cameFrom.Add(n, tile);
                        frontier.Enqueue(n);
                    }
                }
                

            }
            return path;
        }

        public List<Tile> GetNeighbors(Tile tile, Vector2 endTarget)
        {
            Vector2 dir = endTarget - tile.Position;
            List<Tile> tiles = new List<Tile>();

            tiles.Add(chunkManager.GetTileAt(tile.Position.X + Tile.GridSize, tile.Position.Y));
            tiles.Add(chunkManager.GetTileAt(tile.Position.X - Tile.GridSize, tile.Position.Y));
            tiles.Add(chunkManager.GetTileAt(tile.Position.X, tile.Position.Y - Tile.GridSize));
            tiles.Add(chunkManager.GetTileAt(tile.Position.X, tile.Position.Y + Tile.GridSize));

            //if (dir.X > 0 && dir.Y > 0)
            //{
            //    // Start bottom right.
            //    tiles.Add(chunkManager.GetTileAt(tile.Position.X + Tile.GridSize, tile.Position.Y + Tile.GridSize));
            //}

            return tiles;
        }
    }
}

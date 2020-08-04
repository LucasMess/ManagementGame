using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.World
{
    class GameWorld
    {
        public const int MapHeightInChunks = 5;
        public const int MaxHeightInTiles = MapHeightInChunks * Chunk.Size;

        public const float GravitationalForce = 10;
        public const float FrictionCoeffAir = .999f;

        public static Random Random = new Random();

        private ChunkManager chunkManager;

        public Player player;

        public GameWorld()
        {
            player = new Player();
            chunkManager = new ChunkManager("test");
            SpawnEntity(player, 0, 0);

            //for (int i = 0; i < 100; i++)
            //{
            //    int x = Random.Next(0, 200);
            //    int y = Random.Next(0, 200);
            //    SpawnEntity(new Ball(x, y), x, y);
            //}
        }

        public void Update(GameTime gameTime, Camera camera)
        {
            chunkManager.LoadChunksAroundCamera(camera);
            foreach (var chunk in chunkManager.GetChunks().ToList())
            {
                chunk.TransferEntities();
            }
            foreach (var chunk in chunkManager.GetChunks())
            {
                chunk.Update(gameTime, chunkManager);
            }
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
            int chunkX = x / Chunk.Size / Tile.Size;
            int chunkY = y / Chunk.Size / Tile.Size;
            chunkManager.GetChunk(chunkX, chunkY).AddEntity(entity);
        }
    }
}

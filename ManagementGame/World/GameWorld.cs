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
        public const int MapWidthInChunks = 1;
        public const int MapHeightInChunks = 1;
        public const int MaxHeightInTiles = MapHeightInChunks * Chunk.Size;

        public const float GravitationalForce = 10;
        public const float FrictionCoeffAir = .999f;

        private ChunkLoader chunkLoader;
        private Dictionary<string, Chunk> chunks;

        public Player player;

        public GameWorld()
        {
            chunks = new Dictionary<string, Chunk>();
            player = new Player();
        }


        public void LoadChunks()
        {
            chunkLoader = new ChunkLoader("test");
            for (int y = 0; y < MapHeightInChunks; y++)
            {
                for (int x = 0; x < MapWidthInChunks; x++)
                {
                    SetChunk(x, y, chunkLoader.LoadChunk(x, y));
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            player.Update(gameTime);

            foreach (var chunk in chunks.Values)
            {
                chunk.Update(gameTime, player);
            }
        }

        public Chunk GetChunk(int x, int y)
        {
            string key = $"({x},{y})";
            if (chunks.ContainsKey(key))
            {
                return chunks[key];
            }
            return null;
        }

        public void SetChunk(int x, int y, Chunk chunk)
        {
            string key = $"({x},{y})";
            chunks.Add(key, chunk);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entry in chunks)
            {
                entry.Value.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);
        }
    }
}

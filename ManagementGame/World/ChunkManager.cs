using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.World
{
    class ChunkManager
    {
        private const int MaxLoadedChunks = 128;

        private string worldName;
        private TerrainGenerator terrainGenerator;
        private Dictionary<string, Chunk> chunks;

        public ChunkManager(string worldName)
        {
            this.worldName = worldName;
            chunks = new Dictionary<string, Chunk>();
            terrainGenerator = new TerrainGenerator();
        }

        public void LoadChunksAroundCamera(Camera camera)
        {
            foreach (var chunk in chunks.Values)
            {
                chunk.IsActive = false;
            }

            int centerX = (int)Math.Floor(camera.X / Tile.Size / Chunk.Size);
            int centerY = (int)Math.Floor(camera.Y / Tile.Size / Chunk.Size);
            for (int y = 0; y <= camera.ViewRadius * 2; y++)
            {
                for (int x = 0; x <= camera.ViewRadius * 2; x++)
                {
                    GetChunk(centerX + x - camera.ViewRadius, centerY + y - camera.ViewRadius);
                }
            }

            if (chunks.Count > MaxLoadedChunks)
            {
                Console.WriteLine("Removing chunks");
                foreach (var entry in chunks.ToList())
                {
                    if (!entry.Value.IsActive)
                    {
                        chunks.Remove(entry.Key);
                    }
                }
            }
        }

        private Chunk LoadChunk(int x, int y)
        {
            Chunk chunk = terrainGenerator.GenerateChunk(x, y);
            SetChunk(x, y, chunk);
            return chunk;
        }

        public Chunk GetChunk(int x, int y)
        {
            string key = $"({x},{y})";
            if (chunks.ContainsKey(key))
            {
                var chunk = chunks[key];
                chunk.IsActive = true;
                return chunk;
            }
            else
            {
                return LoadChunk(x, y);
            }
        }

        private void SetChunk(int x, int y, Chunk chunk)
        {
            string key = $"({x},{y})";
            chunks.Add(key, chunk);
            chunk.EntityTransfer.Subscribe(entityTransfer =>
            {
                Console.WriteLine($"({x},{y})");
                int chunkX = (int)Math.Floor(entityTransfer.Entity.X / Chunk.Size / Tile.Size);
                int chunkY = (int)Math.Floor(entityTransfer.Entity.Y / Chunk.Size / Tile.Size);
                Console.WriteLine($"({x},{y}) => ({chunkX},{chunkY})");
                GetChunk(chunkX, chunkY).AddEntity(entityTransfer.Entity);
            });
        }

        public Dictionary<string, Chunk>.ValueCollection GetChunks()
        {
            return chunks.Values;
        }

        public List<Chunk> GetNeighborsAndSelf(Chunk chunk)
        {
            var neighbors = new List<Chunk>(9);
            neighbors.Add(GetChunk(chunk.X - 1, chunk.Y - 1));
            neighbors.Add(GetChunk(chunk.X, chunk.Y - 1));
            neighbors.Add(GetChunk(chunk.X + 1, chunk.Y - 1));
            neighbors.Add(GetChunk(chunk.X - 1, chunk.Y));
            neighbors.Add(GetChunk(chunk.X, chunk.Y));
            neighbors.Add(GetChunk(chunk.X + 1, chunk.Y));
            neighbors.Add(GetChunk(chunk.X - 1, chunk.Y + 1));
            neighbors.Add(GetChunk(chunk.X, chunk.Y + 1));
            neighbors.Add(GetChunk(chunk.X + 1, chunk.Y + 1));
            return neighbors;
        }
    }
}

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
        private const int MaxLoadedChunks = 0;

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

            int centerX = (int)Math.Floor(camera.X / Tile.GridSize / Chunk.Size);
            int centerY = (int)Math.Floor(camera.Y / Tile.GridSize / Chunk.Size);
            for (int y = 0; y <= camera.ViewHeight * 2; y++)
            {
                for (int x = 0; x <= camera.ViewWidth * 2; x++)
                {
                    GetChunk(centerX + x - camera.ViewWidth, centerY + y - camera.ViewHeight);
                }
            }

            if (chunks.Count > MaxLoadedChunks)
            {
                // Console.WriteLine("Removing chunks");
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
            Chunk chunk = terrainGenerator.GenerateChunk(x, y, this);
            SetChunk(x, y, chunk);
            return chunk;
        }

        public Chunk GetChunk(int x, int y, bool load = true)
        {
            string key = $"({x},{y})";
            if (chunks.ContainsKey(key))
            {
                var chunk = chunks[key];
                chunk.IsActive = true;
                return chunk;
            }
            else if (load)
            {
                return LoadChunk(x, y);
            } 
            else
            {
                return null;
            }
        }

        private void SetChunk(int x, int y, Chunk chunk)
        {
            string key = $"({x},{y})";
            chunks.Add(key, chunk);
            chunk.EntityTransfer.Subscribe(entityTransfer =>
            {
                // Console.WriteLine($"({x},{y})");
                int chunkX = (int)Math.Floor(entityTransfer.Entity.X / Chunk.Size / Tile.GridSize);
                int chunkY = (int)Math.Floor(entityTransfer.Entity.Y / Chunk.Size / Tile.GridSize);
                // Console.WriteLine($"({x},{y}) => ({chunkX},{chunkY})");
                GetChunk(chunkX, chunkY).AddEntity(entityTransfer.Entity);
            });

            // Recalculate surrounding light.
            var surr = GetNeighborsAndSelf(chunk);
            foreach (var s in surr)
            {
                s.MakeLightMap();
            }
        }

        public Dictionary<string, Chunk>.ValueCollection GetChunks()
        {
            return chunks.Values;
        }

        public List<Chunk> GetNeighborsAndSelf(Chunk chunk)
        {
            var neighbors = new List<Chunk>();
            neighbors.Add(GetChunk(chunk.ChunkX - 1, chunk.ChunkY - 1, false));
            neighbors.Add(GetChunk(chunk.ChunkX, chunk.ChunkY - 1, false));
            neighbors.Add(GetChunk(chunk.ChunkX + 1, chunk.ChunkY - 1, false));
            neighbors.Add(GetChunk(chunk.ChunkX - 1, chunk.ChunkY, false));
            neighbors.Add(GetChunk(chunk.ChunkX, chunk.ChunkY, false));
            neighbors.Add(GetChunk(chunk.ChunkX + 1, chunk.ChunkY, false));
            neighbors.Add(GetChunk(chunk.ChunkX - 1, chunk.ChunkY + 1, false));
            neighbors.Add(GetChunk(chunk.ChunkX, chunk.ChunkY + 1, false));
            neighbors.Add(GetChunk(chunk.ChunkX + 1, chunk.ChunkY + 1, false));
            return neighbors.Where(x => x != null).ToList();
        }

        public Tile GetTileAt(float worldX, float worldY)
        {
            int chunkX = (int)Math.Floor(worldX / (float)Chunk.Size / Tile.GridSize);
            int chunkY = (int)Math.Floor(worldY / (float)Chunk.Size / Tile.GridSize);
            var chunk = GetChunk(chunkX, chunkY, false);
            if (chunk != null)
            {
                float relativeX = worldX - (chunkX * Chunk.Size * Tile.GridSize);
                float relativeY = worldY - (chunkY * Chunk.Size * Tile.GridSize);
                int x = (int)Math.Floor(relativeX / Tile.GridSize) % Chunk.Size;
                int y = (int)Math.Floor(relativeY / Tile.GridSize) % Chunk.Size;
                return chunk.GetTile(x, y);
            }
            else return null;
        }
    }
}

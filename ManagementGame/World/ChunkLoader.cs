using ManagementGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.World
{
    class ChunkLoader
    {
        private string worldName;
        private TerrainGenerator terrainGenerator;

        public ChunkLoader(string worldName)
        {
            this.worldName = worldName;
            terrainGenerator = new TerrainGenerator();
        }

        public Chunk LoadChunk(int x, int y)
        {
            return terrainGenerator.GenerateChunk(x, y);


            ChunkData chunkData = FileLoader.LoadChunk(worldName, x, y);
            Chunk chunk = new Chunk(chunkData);
            return chunk;
        }
    }


}

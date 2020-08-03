using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementGame.Objects;
using SimplexNoise;

namespace ManagementGame.World
{
    class TerrainGenerator
    { 
        public Chunk GenerateChunk(int chunkX, int chunkY)
        {
            Tile[,] tiles = new Tile[Chunk.Size, Chunk.Size];
            for (int y = 0; y < Chunk.Size; y++)
            {
                for (int x = 0; x < Chunk.Size; x++)
                {
                    int posX = chunkX * Chunk.Size + x;
                    int posY = chunkY * Chunk.Size + y;
                    float height = GetHeight(posX);
                    float noiseValue = Noise.CalcPixel2D(posX, posY, 1/10f) - 128;
                    Console.WriteLine(height);

                    if (posY > height)
                    {
                        tiles[x, y] = new Tile(posX, posY, "Grass");
                    }
                    else
                    {
                        tiles[x, y] = new Tile(posX, posY, "Air");
                    }
                }
            }
            return new Chunk(chunkX, chunkY, tiles);
        }

        private float GetHeight(int x)
        {
            float smooth = Noise.CalcPixel1D(x, 1 / 128f) / 256f * GameWorld.MaxHeightInTiles / 2;
            float rough = Noise.CalcPixel1D(x, 1 / 32f) / 256f * GameWorld.MaxHeightInTiles / 2;
            return (smooth + rough * 2) / 3;
        }
    }
}

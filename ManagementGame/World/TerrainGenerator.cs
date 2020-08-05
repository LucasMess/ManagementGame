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
        public Chunk GenerateChunk(int chunkX, int chunkY, ChunkManager chunkManager)
        {
            Tile[,] tiles = new Tile[Chunk.Size, Chunk.Size];
            for (int y = 0; y < Chunk.Size; y++)
            {
                for (int x = 0; x < Chunk.Size; x++)
                {
                    int posX = chunkX * Chunk.Size + x;
                    int posY = chunkY * Chunk.Size + y;
                    float height = GetHeight(posX);
                    bool isCave = IsCave(posX, posY);
                    if (!isCave)
                    {
                        if (posY > height + 20)
                        {
                            tiles[x, y] = new Tile(posX, posY, "Stone");
                        }
                        else if (posY > height)
                        {
                            tiles[x, y] = new Tile(posX, posY, "Grass");
                        }
                        else
                        {
                            tiles[x, y] = new Tile(posX, posY, "Air");
                        }
                    } else
                    {
                        tiles[x, y] = new Tile(posX, posY, "Air");
                    }

                   
                }
            }
            return new Chunk(chunkX, chunkY, tiles, chunkManager);
        }

        private float GetHeight(int x)
        {
            float smooth = Noise.CalcPixel1D(x, 1 / 128f) / 256f * GameWorld.MaxHeightInTiles / 2;
            float rough = Noise.CalcPixel1D(x + 100, 1 / 32f) / 256f * GameWorld.MaxHeightInTiles / 2;
            return (smooth + rough * 2) / 3;
        }

        private bool IsCave(int x, int y)
        {
            float baseNoise = GetNoise2D(x, y / 10, 0, 100, 32);
            float rareLargeCaves = GetNoise2D(x + 200, y + 200, 0, 100, 128);
            float details = GetNoise2D(x + 200, y + 200, 0, 100, 16);
            float res = (float)Math.Pow((baseNoise * 3 + rareLargeCaves * 1 + details * 5) / 9, 2);
            //Console.WriteLine(res);
            return res > 3000;
        }

        private float GetNoise1D(int x, int min, int max, float frequency = 256f)
        {
            return (Noise.CalcPixel1D(x, 1 / frequency) / 256f) * (max - min) + min;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="frequency">The smaller the frequency the smaller the features.</param>
        /// <returns></returns>
        private float GetNoise2D(int x, int y, int min, int max, float frequency = 256f)
        {
            return (Noise.CalcPixel2D(x, y, 1 / frequency) / 256f) * (max - min) + min;
        }
    }
}

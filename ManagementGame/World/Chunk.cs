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
    class Chunk : GameObject
    {
        public const int Size = 16;

        private Tile[,] tiles;

        public Chunk(int x, int y, Tile[,] tiles)
        {
            X = x;
            Y = y;
            this.tiles = tiles;
        }

        public Chunk(ChunkData data)
        {
            X = data.X;
            Y = data.Y;
            tiles = new Tile[Size, Size];
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    int i = y * Size + x;
                    string tileName = data.TileMappings[data.TileIds[i]];
                    tiles[x, y] = new Tile(x, y, tileName);
                }
            }
        }

        public void Update(GameTime gameTime, Player player)
        {
            PhysicsEngine.CalculateVelocity(player, gameTime);
            PhysicsEngine.ApplyVelocityAndSolveCollisions(player, tiles);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in tiles)
            {
                tile.Draw(spriteBatch);
            }
        }
    }
}

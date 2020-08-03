using ManagementGame.Objects.Interfaces;
using ManagementGame.Objects.Tiles;
using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Objects
{
    /// <summary>
    /// Tile is the base class for all game objects that are stationary on a grid.
    /// </summary>
    class Tile : GameObject
    {
        public const int Size = 16;
        public string Name;
        public static TileProperties[] Properties;

        public Tile(int x, int y, string name)
        {
            X = x * Size;
            Y = y * Size;
            Name = name;
            if (name == "Air")
            {
                Visible = false;
            } else
            {
                Texture = ContentLoader.GetTexture2D(name);
                DrawRectangleOffset = new Point(Size, Size);
                CollisionRectangleOffset = new Point(Size, Size);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(Texture, DrawRectangle, Color.White);
            }
        }

        public bool IsSolid => Name != "Air";
    }
}

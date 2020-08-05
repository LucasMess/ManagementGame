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
        public const sbyte MaxLightLevel = 16;

        public string Name;
        public static TileProperties[] Properties;
        public sbyte LightLevel = 0;
        public sbyte LightIntensity = 16;

        public Tile(int x, int y, string name)
        {
            X = x * Size;
            Y = y * Size;
            Name = name;
            if (name == "Air")
            {
                Visible = false;
                LightLevel = LightIntensity;
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
                float light = (LightLevel / (float)MaxLightLevel);
                Color color = new Color(light, light, light, 1);
                spriteBatch.Draw(Texture, DrawRectangle, Color.White);
            }
        }

        public void SetLightLevel(int newLightLevel)
        {
            if (!IsLightSource)
            {
                LightLevel = (sbyte)newLightLevel;
            }
        }

        public bool IsSolid => Name != "Air";
        public bool IsTransparent => Name == "Air";
        public bool IsLightSource => Name == "Air";

    }
}

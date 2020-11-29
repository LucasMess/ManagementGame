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
        public const int GridSize = 64;
        public const int DrawSize = 80;
        public const int MaxLightLevel = 8;
        public const int LightFalloff = 2;


        public TileType TileType;
        public int Id = -1;
        public static TileProperties[] Properties;
        public int LightLevel = 0;
        public int LightIntensity = 8;

        public int TileX;
        public int TileY;

        public Tile(int x, int y, TileType tileType)
        {
            X = x * GridSize;
            Y = y * GridSize;
            TileX = x;
            TileY = y;
            Id = (int)tileType;
            TileType = tileType;
            if (TileType == TileType.Air)
            {
                Visible = false;
                LightLevel = LightIntensity;
            } 
            else
            {
                Texture = ContentLoader.GetTexture2D(Name);                
            }
            DrawRectangleSize = new Point(DrawSize, DrawSize);
            CollisionRectangleSize = new Point(GridSize, GridSize);
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

        public void DebugDraw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                //spriteBatch.Draw(ContentLoader.DebugTexture, CollisionRectangle, Color.Red * .5f);
                //spriteBatch.Draw(ContentLoader.DebugTexture, DrawRectangle, Color.Blue * .5f);
                //spriteBatch.Draw(ContentLoader.DebugTexture, new Rectangle((int)Position.X, (int)Position.Y, 1, 1), Color.Green * .5f);
               //spriteBatch.DrawString(ContentLoader.GetFont("x32"), LightLevel.ToString(), Position - new Vector2(8, 24), Color.White * .5f);
            }
        }

        public void SetLightLevel(int newLightLevel)
        {
            if (!IsLightSource)
            {
                LightLevel = newLightLevel;
            }
        }

        public bool IsSolid => Name != "Air";
        public bool IsTransparent => Name == "Air";
        public bool IsLightSource => Name == "Air";

        public string Name => Properties[Id].Name;

    }
}

using ManagementGame.Utils;
using ManagementGame.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Objects.Entities
{
    class Ball :  Entity
    {
        Color color;
        public Ball(int x, int y)
        {
            X = x;
            Y = y;
            Texture = ContentLoader.GetTexture2D("Ball");
            CollisionRectangleOffset = new Point(16, 16);
            color = Debug.GenerateRandomColor();
            VelX = (GameWorld.Random.Next(0, 5) - 2);
            VelY = (GameWorld.Random.Next(0, 5) - 2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, CollisionRectangle, color);
            base.Draw(spriteBatch);
        }
    }
}

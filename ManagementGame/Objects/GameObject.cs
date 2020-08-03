using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Point = Microsoft.Xna.Framework.Point;

namespace ManagementGame.Objects
{
    /// <summary>
    /// The base class of all game objects.
    /// </summary>
    class GameObject
    {
        public Vector2 Position { get; protected set; }
        public bool Visible { get; set; } = true;
        public float Elasticity { get; set; } = 1;
        public float X
        {
            set
            {
                Position = new Vector2(value, Position.Y);
            }
            get { return Position.X; }
        }
        public float Y
        {
            set
            {
                Position = new Vector2(Position.X, value);
            }
            get { return Position.Y; }
        }

        protected Texture2D Texture { get; set; }
        public Rectangle DrawRectangle => new Rectangle(
            (int)Math.Ceiling(X) - DrawRectangleOffset.X / 2,
            (int)Math.Ceiling(Y) - DrawRectangleOffset.Y / 2,
            DrawRectangleOffset.X,
            DrawRectangleOffset.Y
        );
        public Rectangle CollisionRectangle => new Rectangle(
            (int)Math.Ceiling(X) - CollisionRectangleOffset.X / 2,
            (int)Math.Ceiling(Y) - CollisionRectangleOffset.Y / 2,
            CollisionRectangleOffset.X,
            CollisionRectangleOffset.Y
        );

        protected Point DrawRectangleOffset;
        protected Point CollisionRectangleOffset;

        public virtual void Draw(SpriteBatch spriteBatch) {

        }
    }
}

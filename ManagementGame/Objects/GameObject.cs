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
            (int)Math.Ceiling(X) - DrawRectangleSize.X / 2,
            (int)Math.Ceiling(Y) - DrawRectangleSize.Y / 2,
            DrawRectangleSize.X,
            DrawRectangleSize.Y
        );
        public Rectangle CollisionRectangle => new Rectangle(
            (int)Math.Ceiling(X) - CollisionRectangleSize.X / 2,
            (int)Math.Ceiling(Y) - CollisionRectangleSize.Y / 2,
            CollisionRectangleSize.X,
            CollisionRectangleSize.Y
        );

        protected Point DrawRectangleSize;
        protected Point CollisionRectangleSize;

        public virtual void Draw(SpriteBatch spriteBatch) {

        }
    }
}

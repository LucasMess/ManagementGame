using ManagementGame.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Objects
{
    /// <summary>
    /// The entity class is the base of all game objects that are not tied to a grid.
    /// </summary>
    class Entity : GameObject
    {
        public float Mass = 1f;
        
        public List<Vector2> Forces;

        public Vector2 Velocity { get; set; }
        public bool CanCollideWithOtherEntities { get; protected set; } = true;
        public bool CanCollideWithTerrain { get; protected set; } = true;
        public bool AffectedByGravity { get; protected set; } = true;

        public Entity()
        {
            Forces = new List<Vector2>();
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public void ApplyForce(Vector2 force) => Forces.Add(force);

        public void ApplyForceX(float force) => ApplyForce(new Vector2(force, 0));

        public void ApplyForceY(float force) => ApplyForce(new Vector2(0, force));



        public float VelX
        {
            set { Velocity = new Vector2(value, Velocity.Y); }
            get { return Velocity.X; }
        }
        public float VelY
        {
            set { Velocity = new Vector2(Velocity.X, value); }
            get { return Velocity.Y; }
        }
    }

}

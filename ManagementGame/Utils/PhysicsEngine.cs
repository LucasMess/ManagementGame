using ManagementGame.Objects;
using ManagementGame.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Utils
{
    static class PhysicsEngine
    {
        public static void CalculateVelocity(Entity entity, GameTime gameTime)
        {
            // Air friction.
            entity.ApplyForce(GameWorld.FrictionCoeffAir * -entity.Velocity);
            // Gravity.
            //if (entity.AffectedByGravity)
            //{
            //    entity.ApplyForceY(GameWorld.GravitationalForce);
            //}

            // Sum forces and get acceleration.
            var forceTotal = new Microsoft.Xna.Framework.Vector2(entity.Forces.Sum(i => i.X), entity.Forces.Sum(i => i.Y));
            var acceleration = forceTotal / entity.Mass;

            entity.Velocity += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            entity.Forces.Clear();
        }

        public static void ApplyVelocityX(Entity entity)
        {
            entity.X += entity.VelX;
        }

        public static void ApplyVelocityY(Entity entity)
        {
            entity.Y += entity.VelY;
        }

        public static void ResolveTerrainCollisionsY(Entity entity, Tile[,] tiles)
        {
            if (!entity.CanCollideWithTerrain)
            {
                return;
            }
            foreach (var tile in tiles)
            {
                if (!tile.IsSolid)
                {
                    continue;
                }
                if (entity.CollisionRectangle.Intersects(tile.CollisionRectangle))
                {

                    if (entity.VelY > 0)
                    {
                        if (entity.CollisionRectangle.Bottom > tile.CollisionRectangle.Top)
                        {
                            // Collided with top of tile.
                            Console.WriteLine("Collided Top");                            
                            entity.Y = tile.CollisionRectangle.Top - entity.CollisionRectangle.Height;
                            entity.VelY = (-entity.Elasticity * entity.VelY);
                        }
                    }
                    else if (entity.VelY < 0)
                    {
                        if (entity.CollisionRectangle.Top < tile.CollisionRectangle.Bottom)
                        {
                            // Collided from below.
                            Console.WriteLine("Collided below");
                            entity.Y = tile.CollisionRectangle.Bottom;
                            entity.VelY = -entity.Elasticity * entity.VelY;

                        }
                    }
                }
            }
        }

        public static void ResolveTerrainCollisionsX(Entity entity, Tile[,] tiles)
        {
            if (!entity.CanCollideWithTerrain)
            {
                return;
            }
            foreach (var tile in tiles)
            {
                if (!tile.IsSolid)
                {
                    continue;
                }
                if (entity.CollisionRectangle.Intersects(tile.CollisionRectangle))
                {
                    if (entity.VelX > 0)
                    {
                        if (entity.CollisionRectangle.Right > tile.CollisionRectangle.Left)
                        {
                            // Collided from left.
                            Console.WriteLine("Collided left");

                            entity.X = tile.CollisionRectangle.Left - entity.CollisionRectangle.Width;
                            entity.VelX = (int)(-entity.Elasticity * entity.VelX);

                        }
                    }
                    else if (entity.VelX < 0)
                    {
                        if (entity.CollisionRectangle.Left < tile.CollisionRectangle.Right)
                        {
                            // Collided from right.
                            Console.WriteLine("Collided right");

                            entity.X = tile.CollisionRectangle.Right;
                            entity.VelX = (int)(-entity.Elasticity * entity.VelX);

                        }
                    }
                }
            }
        }

        public static int Round(float value)
        {
            if (value >= 0)
            {
                return (int)Math.Floor(value);
            } 
            else
            {
                return (int)Math.Floor(value);
            }
        }
    }
}

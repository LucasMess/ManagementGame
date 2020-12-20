using ManagementGame.Objects;
using ManagementGame.Objects.Entities;
using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Point = Microsoft.Xna.Framework.Point;

namespace ManagementGame.World
{
    class Room : GameObject
    {
        public bool IsActive = true;

        public List<Entity> Entities { get; private set; }

        public Room()
        {
            Entities = new List<Entity>();
            Texture = ContentLoader.GetTexture2D("Grass");
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in Entities)
            {
                //PhysicsEngine.CalculateVelocity(entity, gameTime);
                //PhysicsEngine.ApplyVelocityY(entity);
                //PhysicsEngine.ResolveTerrainCollisionsY(entity, Tiles);
                //PhysicsEngine.ApplyVelocityX(entity);
                //PhysicsEngine.ResolveTerrainCollisionsX(entity, Tiles);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {



            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null);

            foreach (var entity in Entities)
            {
                entity.Draw(spriteBatch);
            }
            //spriteBatch.Draw(solidTiles, CollisionRectangle, debugColor);

            spriteBatch.End();

            //spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp, null, null, null, camera.GetViewMatrix());
            ////spriteBatch.Draw(solidMap, CollisionRectangle, debugColor * .5f);
            ////spriteBatch.Draw(ContentLoader.DebugTexture, CollisionRectangle, debugColor * .5f);
            //foreach (var tile in Tiles)
            //{
            //    tile.DebugDraw(spriteBatch);
            //}
            //spriteBatch.End();

        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

    }
}

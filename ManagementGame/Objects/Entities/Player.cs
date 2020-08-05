using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Objects.Entities
{
    class Player : Entity
    {

        public Player()
        {
            const float movementForce = 10;
            InputManager.KeyPressed(Keys.W).Subscribe(keyEvent =>
            {
                ApplyForceY(-movementForce * 3);
            });
            InputManager.KeyPressed(Keys.A).Subscribe(keyEvent =>
            {
                ApplyForceX(-movementForce);
            });
            InputManager.KeyPressed(Keys.S).Subscribe(keyEvent =>
            {
                ApplyForceY(movementForce);
            });
            InputManager.KeyPressed(Keys.D).Subscribe(keyEvent =>
            {
                ApplyForceX(movementForce);
            });

            Texture = ContentLoader.GetTexture2D("Grass");
            CanCollideWithTerrain = true;
            AffectedByGravity = true;
            DrawRectangleSize = new Point(Tile.TextureSize, Tile.TextureSize);
            CollisionRectangleSize = new Point(Tile.GridSize, Tile.GridSize);
            Elasticity = .8f;
        }

        public override void Update(GameTime gameTime)
        {
            // Console.WriteLine($"({X},{Y}) ({VelX},{VelY})");
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(Texture, DrawRectangle, Color.White);
            base.Draw(spriteBatch);
        }
    }
}

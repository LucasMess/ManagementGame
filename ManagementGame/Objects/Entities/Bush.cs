using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Objects.Entities
{
    class Bush : Entity, Interactable
    {

        private Texture2D textureWithBerries;
        private Texture2D textureNoBerries;

        public Bush()
        {
            textureWithBerries = ContentLoader.GetTexture2D("bush_with_berries");
            textureNoBerries = ContentLoader.GetTexture2D("bush_no_berries");
            Texture = textureWithBerries;
            DrawRectangleSize = new Point(Tile.DrawSize, Tile.DrawSize);
            CollisionRectangleSize = new Point(Tile.GridSize, Tile.GridSize);
        }

        public void Interact(Entity interactor)
        {
            Texture = textureNoBerries;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, DrawRectangle, Color.White);
            base.Draw(spriteBatch);
        }
    }
}

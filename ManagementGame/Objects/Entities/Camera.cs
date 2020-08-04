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
    class Camera : Entity
    {
        private Viewport viewport;

        public int ViewRadius;

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
            AffectedByGravity = false;
            ViewRadius = (int)Math.Ceiling((float)viewport.Width / Tile.Size / Chunk.Size / 2);
            Console.WriteLine($"ViewRadius is {ViewRadius}");
        }

        public void PointTo(GameObject gameObject)
        {
            Position = gameObject.Position;
        }

        public Matrix GetViewMatrix()
        {
            var pos3 = (new Vector3(Position, 0) - new Vector3(viewport.Width / 2f, viewport.Height * 2f / 3, 0)) * -1;
            pos3 = new Vector3((float)Math.Ceiling(pos3.X), (float)Math.Ceiling(pos3.Y), (float)Math.Ceiling(pos3.Z));
            return Matrix.CreateTranslation(pos3);
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.Invert(GetViewMatrix());
        }

        public Matrix GetProjectionMatrix()
        {
            PresentationParameters pp = ManagementGame.CurrentGraphicsDevice.PresentationParameters;
            return Matrix.CreateOrthographicOffCenter(0, pp.BackBufferWidth, pp.BackBufferHeight, 0, -2000f, 2000f);
        }

    }
}

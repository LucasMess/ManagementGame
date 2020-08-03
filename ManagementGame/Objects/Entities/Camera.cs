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

        public Camera(Viewport viewport)
        {
            this.viewport = viewport;
            AffectedByGravity = false;
        }

        public void PointTo(GameObject gameObject)
        {
            Position = gameObject.Position;
        }

        public Matrix GetTransformMatrix()
        {
            var pos3 = (new Vector3(Position, 0) - new Vector3(viewport.Width / 2f, viewport.Height * 2f / 3, 0)) * -1;
            pos3 = new Vector3((float)Math.Ceiling(pos3.X), (float)Math.Ceiling(pos3.Y), (float)Math.Ceiling(pos3.Z));
            return Matrix.CreateTranslation(pos3);
        }

    }
}

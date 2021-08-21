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
    class Cube
    {
        Model model;

        Matrix world;
        Matrix view;
        Matrix projection;

        public Cube()
        {
            model = ContentLoader.GetModel("cube");
            world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        }


        public void DrawModel(Camera camera)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = camera.CreateIsoView();
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }
}

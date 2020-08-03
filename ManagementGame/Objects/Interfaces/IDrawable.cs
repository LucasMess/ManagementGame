using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Objects.Interfaces
{
    interface IDrawable
    {
        Texture2D Texture { get; set; }
        Rectangle DrawRectangle { get; set; }

        bool Visible { get; set; }

        void Draw(SpriteBatch spriteBatch);

    }
}

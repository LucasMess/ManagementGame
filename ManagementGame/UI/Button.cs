using ManagementGame.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.UI
{
    class Button: UiElement
    {


        public Button(string text)
        {
            Texture = ContentLoader.GetTexture2D("Grass");
            SetText(text);
        }
    }
}

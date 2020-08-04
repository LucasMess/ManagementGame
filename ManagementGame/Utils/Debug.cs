using ManagementGame.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Utils
{
    static class Debug
    {

        public static Color GenerateRandomColor()
        {
            return new Color(GameWorld.Random.Next(0, 256), GameWorld.Random.Next(0, 256), GameWorld.Random.Next(0, 256), 128);
        }
    }
}

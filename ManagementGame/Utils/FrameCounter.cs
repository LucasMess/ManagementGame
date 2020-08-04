using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.Utils
{
    class FrameCounter
    {
        private double timer;
        private int totalFrames;
        private int fps;

        public void Reset()
        {
            totalFrames++;
        }
        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer > 1000)
            {
                fps = totalFrames;
                totalFrames = 0;
                timer = 0;
            }
        }

        public int GetFps()
        {
            return fps;
        }
    }
}

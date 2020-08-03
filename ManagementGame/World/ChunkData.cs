using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.World
{
    [Serializable]
    class ChunkData
    {
        public int X;
        public int Y;
        public Dictionary<int, string> TileMappings;
        public int[] TileIds;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game
{
    struct Tile
    {
        public char c;
        public Color col;
        public bool porous;
        public int durability;

        public int hitsNeeded(int depth)
        {
            return (int)(durability * Math.Exp(depth * 0.01f));
        }
    }
}

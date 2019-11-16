using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game
{
    class TileSet
    {
        static Tile[] Data =
        {
            new Tile() { c = ' ', col =  Color.White, porous = true },
            new Tile() { c = '#', col =  Color.White, porous = false },
            new Tile() { c = 'o', col =  Color.LightGray, porous = true },
            new Tile() { c = 'X', col =  Color.Brown, porous = false },
        };

        public static int TileCount { get { return Data.Length; } }

        public static Tile Tile(TileType index) { return Data[(int)index]; }

        public enum TileType
        {
            AIR,
            STONE,
            GRAVEL,
            DIRT
        }
    }
}

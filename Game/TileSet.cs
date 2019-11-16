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
            new Tile() { c = '#', col =  Color.FromArgb(180,180,180), porous = false, durability = 50, value = 1 },
            new Tile() { c = 'o', col =  Color.FromArgb(220,220,220), porous = true, durability = 20, value = 1 },
            new Tile() { c = 'X', col =  Color.Brown, porous = false, durability = 15 },
            new Tile() { c = 'C', col =  Color.Orange, porous = false, durability = 60, value = 10 },
            new Tile() { c = 'I', col =  Color.FromArgb(255,200,150), porous = false, durability = 100, value = 25 },
            new Tile() { c = 'S', col =  Color.White, porous = false, durability = 80, value = 50 },
            new Tile() { c = 'G', col =  Color.Gold, porous = false, durability = 60, value = 80 },
            new Tile() { c = 'P', col =  Color.FromArgb(220,220,255), porous = false, durability = 150, value = 120 }
        };

        public static int TileCount { get { return Data.Length; } }

        public static Tile Tile(TileType index) { return Data[(int)index]; }

        public enum TileType
        {
            AIR,
            STONE,
            GRAVEL,
            DIRT,
            COPPER,
            IRON,
            SILVER,
            GOLD,
            PLATINUM
        }
    }
}

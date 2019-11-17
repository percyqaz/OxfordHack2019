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
            new Tile() { c = '#', col =  Color.FromArgb(150,150,150), durability = 50, value = 1 },
            new Tile() { c = 'o', col =  Color.FromArgb(220,220,220), porous = true, durability = 20, value = 1 },
            new Tile() { c = 'X', col =  Color.Brown, porous = true, durability = 15 },
            //ores
            new Tile() { c = 'C', col =  Color.Orange, durability = 60, value = 10 },
            new Tile() { c = 'Z', col =  Color.Silver, durability = 80, value = 25 },
            new Tile() { c = 'N', col =  Color.Gray, durability = 100, value = 50 },
            new Tile() { c = 'I', col =  Color.FromArgb(255,200,150), durability = 200, value = 100 },
            new Tile() { c = 'S', col =  Color.White, durability = 200, value = 250 },
            new Tile() { c = 'L', col =  Color.MidnightBlue, durability = 300, value = 600 },
            new Tile() { c = 'G', col =  Color.Gold, durability = 150, value = 900 },
            new Tile() { c = 'P', col =  Color.FromArgb(220,220,255), durability = 250, value = 1200 },
            new Tile() { c = 'C', col =  Color.LightBlue, durability = 500, value = 2500 },
            new Tile() { c = 'P', col =  Color.FromArgb(255,180,80), durability = 400, value = 2000 },
            new Tile() { c = 'W', col =  Color.FromArgb(255,255,200), durability = 500, value = 3000 },
            new Tile() { c = 'I', col =  Color.FromArgb(255,255,255), durability = 700, value = 6000 },
            new Tile() { c = 'U', col =  Color.FromArgb(100,255,100), durability = 1000, value = 10000 },
            //gems
            new Tile() { c = '#', col =  Color.FromArgb(50,0,100), durability = 500, value = 500 },
            new Tile() { c = '*', col =  Color.FromArgb(255,100,100), durability = 100000, value = 50000 },
            new Tile() { c = '*', col =  Color.FromArgb(100,255,100), durability = 100000, value = 50000 },
            new Tile() { c = '*', col =  Color.FromArgb(100,100,255), durability = 100000, value = 50000 },
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
            ZINC,
            NICKEL,
            IRON,
            SILVER,
            LEAD,
            GOLD,
            PLATINUM,
            COBALT,
            PALLADIUM,
            TUNGSTEN,
            IRIDIUM,
            URANIUM,
            OBSIDIAN,
            RUBY,
            EMERALD,
            SAPPHIRE
        }
    }
}

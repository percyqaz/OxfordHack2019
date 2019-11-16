using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimplexNoise;

namespace Game
{
    class WorldGenerator
    {
        public static TileSet.TileType GenTile(int x, int y)
        {
            var n = Octaves(x, y);
            if (n > 150) return TileSet.TileType.AIR;
            if (n > 130) return TileSet.TileType.DIRT;
            if (n > 80 && n < 90) return TileSet.TileType.GRAVEL;
            return Gen_Ore(x, y);
        }

        static float Octaves(int x, int y)
        {
            return (
                4 * Noise.CalcPixel2D(x, y, 0.02f) +
                2 * Noise.CalcPixel2D(x, y, 0.06f) +
                Noise.CalcPixel2D(x, y, 0.1f) +
                Noise.CalcPixel2D(x, y, 0.3f)
                ) / 8f;
        }

        static TileSet.TileType Gen_Ore(int x, int y)
        {
            float value = Noise.CalcPixel2D(x, y, 0.1f) / 256f;

            double range = 0.1d + Math.Log(y + 1) * 0.02d;
            if (value > range) return TileSet.TileType.STONE;
            else
            {
                value = (float)Math.Pow(value / range, y / 400f);
                int tier = 16 - (int)Math.Floor(value * 13); //13 ores
                return (TileSet.TileType)(tier);
            }
        }

        //gen_gem

        public static int GenFluid(int x, int y)
        {
            float n = Noise.CalcPixel2D(x, y, 0.05f) / 255f;
            if (n < 0.08f) return 10;
            else if (n > 0.92f) return -10;
            return 0;
        }
    }
}

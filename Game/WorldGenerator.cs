using System;
using System.IO;
using System.Reflection;
using SimplexNoise;

namespace Game
{
    class WorldGenerator
    {
        static string[] S500 = LoadStructure("Game.Structures.500.txt");
        static string[] S1000 = LoadStructure("Game.Structures.1000.txt");

        public static TileSet.TileType GenTile(int x, int y)
        {
            if (y >= 530 && y < 530 + S500.Length)
            {
                int w = S500[0].Length - 1;
                if (x >= (Screen.WIDTH -w) / 2 && x < (Screen.WIDTH + w) / 2)
                {
                    var t = Gen_Structure(x - (Screen.WIDTH - w) / 2, y - 530, S500);
                    if (t != TileSet.TileType.AIR) return t;
                }
            }
            else if (y >= 1030 && y < 1030 + S1000.Length)
            {
                int w = S1000[0].Length - 1;
                if (x >= (Screen.WIDTH - w) / 2 && x < (Screen.WIDTH + w) / 2)
                {
                    var t = Gen_Structure(x - (Screen.WIDTH - w) / 2, y - 1030, S1000);
                    if (t != TileSet.TileType.AIR) return t;
                }
            }
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
            if (value > range + 0.005) return TileSet.TileType.STONE;
            else if (value > range) return TileSet.TileType.RUBY + y % 3;
            else
            {
                value = (float)Math.Pow(value / range, y / 400f);
                int tier = 16 - (int)Math.Floor(value * 13); //13 ores
                return (TileSet.TileType)(tier);
            }
        }

        public static int GenFluid(int x, int y)
        {
            float n = Noise.CalcPixel2D(x, y, 0.05f) / 255f;
            if (n < 0.08f) return 10;
            else if (n > 0.92f) return -10;
            return 0;
        }

        static TileSet.TileType Gen_Structure(int x, int y, string[] structure)
        {
            char c = structure[y][x];
            return c == '1' ? TileSet.TileType.RUBY + y % 3 : (c == '0' ? TileSet.TileType.OBSIDIAN : TileSet.TileType.AIR);
        }

        static string[] LoadStructure(string assetPath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assetPath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd().Split('\n');
            }
        }
    }
}

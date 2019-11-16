using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game
{
    class World
    {
        TileSet.TileType[,] Tiles;

        int Camera = 0;

        int Player_X = 0;

        int Player_Y = 0;

        public World()
        {
            var r = new Random();
            Tiles = new TileSet.TileType[Screen.WIDTH, 100];
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    Tiles[x, y] = (TileSet.TileType)r.Next(TileSet.TileCount);
                }
            }
        }

        public void Update()
        {
            switch (Utils.PressedKey)
            {
                case ConsoleKey.LeftArrow:
                    Player_X -= 1;
                    break;
                case ConsoleKey.RightArrow:
                    Player_X += 1;
                    break;
                case ConsoleKey.UpArrow:
                    Player_Y += 1;
                    break;
                default:
                    break;
            }
        }

        public void Draw(Screen s)
        {
            for (int y = 0; y < 30; y++)
            {
                if (y >= 100) continue;
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    var t = TileSet.Tile(Tiles[x, y + Camera]);
                    s.WritePixel(x, y, t.c, t.col, Color.Black); //fluids replace color.black
                }
            }
            // draw tiles
            s.WritePixel(Player_X, Player_Y - Camera, '@', Color.White, Color.LightGreen);
            s.WritePixel(Player_X, Player_Y - Camera + 1, '@', Color.White, Color.LightGreen);
        }
    }
}

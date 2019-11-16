using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TrueColorConsole;

namespace Game
{
    class Screen
    {
        public static readonly int WIDTH = 80;
        public static readonly int HEIGHT = 30;

        struct Pixel
        {
            public char Char;
            public Color Fore;
            public Color Back;
            public int Checksum;

            public Pixel(char c, Color f, Color b)
            {
                Char = c;
                Fore = f;
                Back = b;
                Checksum = c + f.ToArgb() * 7 + b.ToArgb() * 11;
            }

            public override string ToString()
            {
                //color code goes here
                return VTConsole.GetColorForegroundString(Fore.R, Fore.G, Fore.B) +
                    VTConsole.GetColorBackgroundString(Back.R, Back.G, Back.B) +
                    Char.ToString();
            }
        }

        Pixel[,] Pixels;
        Pixel[,] OldPixels;

        public Screen()
        {
            VTConsole.Enable();
            Console.CursorVisible = false;
            Console.SetWindowSize(WIDTH, HEIGHT);
            Pixels = new Pixel[WIDTH, HEIGHT];
            OldPixels = new Pixel[WIDTH, HEIGHT];
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Pixels[x, y] = new Pixel(' ', Color.White, Color.Black);
                    OldPixels[x, y] = new Pixel(' ', Color.White, Color.Black);
                }
            }
        }

        public void Redraw()
        {
            string buf = "";
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    if (Pixels[x, y].Checksum != OldPixels[x, y].Checksum)
                    {
                        if (buf == "")
                        {
                            Console.SetCursorPosition(x, y);
                        }
                        buf += Pixels[x, y].ToString();
                    }
                    else
                    {
                        if (buf != "")
                        {
                            Console.Write(buf);
                            buf = "";
                        }
                    }
                }
            }

            Console.Write(buf);
        }

        public void WritePixel(int x, int y, char c, Color f, Color b)
        {
            if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT) return;
            OldPixels[x, y] = Pixels[x, y];
            Pixels[x, y] = new Pixel(c, f, b);
        }

        public void WriteText(int x, int y, string text, Color f, Color b)
        {
            for (int i = 0; i < text.Length; i++)
            {
                WritePixel(x + i, y, text[i], f, b);
            }
        }
    }
}

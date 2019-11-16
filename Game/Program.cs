using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new Screen();
            var w = new World();
            while (true)
            {
                Utils.PressedKey = Console.KeyAvailable ? Console.ReadKey(true).Key : ConsoleKey.NoName;
                w.Update();
                w.Draw(s);
                s.Redraw();
            }
        }
    }
}

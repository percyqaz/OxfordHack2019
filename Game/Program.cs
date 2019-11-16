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
            int i = 0;
            while (true)
            {
                s.WriteText(i+=1, 0, "Hello world!", Color.White, Color.Black);
                s.Redraw();
            Console.ReadLine();
            }
        }
    }
}

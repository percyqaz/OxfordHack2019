using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace Game
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var s = new Screen();
            var w = new World();
            while (true)
            {
                w.Update();
                w.Draw(s);
                s.Redraw();
                Thread.Sleep(15);
            }
        }
    }
}

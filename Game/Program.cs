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
            //todo: menu
            var s = new Screen();
            UI.Menu("ABYSS", new List<Tuple<Func<string>, Action>>()
            {
                new Tuple<Func<string>, Action>(() => "Play", () => {
                    var w = new World();
                    while (!w.Game_Over)
                    {
                        w.Update();
                        w.Draw(s);
                        s.Redraw();
                        Thread.Sleep(15);
                    }
                }),
                new Tuple<Func<string>, Action>(() => "Play (Dev Cheats)", () => { }),
                new Tuple<Func<string>, Action>(() => "How to Play", () => { }),
                new Tuple<Func<string>, Action>(() => "Credits", () => { })
            }, s);
        }
    }
}

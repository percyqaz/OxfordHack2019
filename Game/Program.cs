using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Game
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var s = new Screen();
            UI.Menu("ABYSS", new List<Tuple<Func<string>, Action>>()
            {
                new Tuple<Func<string>, Action>(() => "Play", () => {
                    var w = new World(false);
                    while (!w.Game_Over)
                    {
                        w.Update();
                        w.Draw(s);
                        s.Redraw();
                        Thread.Sleep(15);
                    }
                }),
                new Tuple<Func<string>, Action>(() => "Play (Dev Cheats)", () => {
                    var w = new World(true);
                    while (!w.Game_Over)
                    {
                        w.Update();
                        w.Draw(s);
                        s.Redraw();
                        Thread.Sleep(15);
                    }}),
                new Tuple<Func<string>, Action>(() => "How to Play", () => {
                    UI.Menu("How to Play", new List<Tuple<Func<string>, Action>>()
                    {
                        new Tuple<Func<string>, Action>(()=>"Arrow keys to move/jump",()=>{ }),
                        new Tuple<Func<string>, Action>(()=>"Space + Arrow keys to mine blocks",()=>{ }),
                        new Tuple<Func<string>, Action>(()=>"You get money depending on the value of the blocks you mine",()=>{ }),
                        new Tuple<Func<string>, Action>(()=>"Press Tab to buy stronger pickaxes and dynamite",()=>{ }),
                        new Tuple<Func<string>, Action>(()=>"Press Z to use dynamite (and step back)",()=>{ }),
                        new Tuple<Func<string>, Action>(()=>"Avoid hazards like lava and falling rubble",()=>{ })
                    },s);
                }),
                new Tuple<Func<string>, Action>(() => "Credits", () => { })
            }, s);
        }
    }
}

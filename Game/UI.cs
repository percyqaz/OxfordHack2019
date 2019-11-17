using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using System.Threading;

namespace Game
{
    class UI
    {
        public static void Menu(string title, List<Tuple<Func<string>,Action>> options, Screen s)
        {
            int selection = options.Count;
            bool f = true;
            while (true)
            {
                if (Keyboard.IsKeyDown(Key.Up))
                {

                    if (!f)
                    {
                        f = true;
                        if (selection == 0)
                            selection = options.Count;
                        else selection -= 1;
                    }
                }
                else if (Keyboard.IsKeyDown(Key.Down))
                {

                    if (!f)
                    {
                        f = true;
                        if (selection == options.Count)
                            selection = 0;
                        else selection += 1;
                    }
                }
                else if (Keyboard.IsKeyDown(Key.Enter))
                {
                    if (!f)
                    {
                        f = true;
                        if (selection == options.Count) break;
                        else options[selection].Item2();
                    }
                }
                else
                {
                    f = false;
                }
                CentreText(s, 5, "  " + title + "  ", Color.DarkBlue, Color.FromArgb(200, 255, 255));
                for (int i = 0; i < options.Count; i++)
                {
                    CentreText(s, 8 + i, "  " + options[i].Item1() + "  ", Color.Black, selection != i ? Color.FromArgb(200, 255, 255) : Color.White);
                }

                CentreText(s, 9 + options.Count, "  Exit  ", Color.Black, selection != options.Count ? Color.FromArgb(200, 255, 255) : Color.White);
                s.Redraw();
                Thread.Sleep(10);
            }
        }

        static void CentreText(Screen s, int y, string t, Color f, Color b)
        {
            s.WriteText((Screen.WIDTH - t.Length) / 2, y, t, f, b);
        }

        public static void StatsScreen(int score, int depth, Dictionary<TileSet.TileType, int> blocks, Screen s)
        {
            List<Tuple<Func<string>, Action>> info = new List<Tuple<Func<string>, Action>>();
            info.Add(new Tuple<Func<string>, Action>(() => "Depth Reached: " + depth.ToString(), () => { }));
            foreach (TileSet.TileType t in blocks.Keys)
            {
                info.Add(new Tuple<Func<string>, Action>(() => t.ToString() + " x" + blocks[t].ToString(), () => { }));
            }
            info.Add(new Tuple<Func<string>, Action>(() => "Total Score: " + score.ToString(), () => { }));
            Menu("Your Stats", info, s); s.Clear();
        }
    }
}

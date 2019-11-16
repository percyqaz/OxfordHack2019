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
            while (true)
            {
                if (Keyboard.IsKeyDown(Key.Up))
                {
                    if (selection == 0)
                        selection = options.Count;
                    else selection -= 1;
                }
                else if (Keyboard.IsKeyDown(Key.Down))
                {
                    if (selection == options.Count)
                        selection = 0;
                    else selection += 1;
                }
                else if (Keyboard.IsKeyDown(Key.Enter))
                {
                    if (selection == options.Count) break;
                    else options[selection].Item2();
                }
                CentreText(s, 5, "  " + title + "  ", Color.DarkBlue, Color.FromArgb(200, 255, 255));
                for (int i = 0; i < options.Count; i++)
                {
                    CentreText(s, 8 + i, "  " + options[i].Item1() + "  ", Color.Black, selection != i ? Color.FromArgb(200, 255, 255) : Color.White);
                }

                CentreText(s, 9 + options.Count, "  Back  ", Color.Black, selection != options.Count ? Color.FromArgb(200, 255, 255) : Color.White);
                s.Redraw();
                Thread.Sleep(200);
            }
        }

        static void CentreText(Screen s, int y, string t, Color f, Color b)
        {
            s.WriteText((Screen.WIDTH - t.Length) / 2, y, t, f, b);
        }
    }
}

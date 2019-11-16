using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Game
{
    class Utils
    {
        public static ConsoleKey PressedKey;

        public static bool IsKeyDown(Key k)
        {
            return Keyboard.IsKeyDown(k);
        }
    }
}

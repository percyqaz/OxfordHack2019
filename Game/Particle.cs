using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game
{
    class Particle
    {
        public int lifespan;
        public int age;
        public Func<int, float> x;
        public Func<int, float> y;
        public Func<int, Color> col;

        public void LinearParticle(int duration, int x, int y, float vx, float vy, Color col1, Color col2)
        {
            lifespan = duration;
            age = 0;
            this.x = (a) => x + a * vx;
            this.y = (a) => y + a * vy;
            col = (a) =>
            {
                var f = (float)a / duration;
                return Color.FromArgb(Lerp(col1.R, col2.R, f), Lerp(col1.G, col2.G, f), Lerp(col1.B, col2.B, f));
            };
        }

        public void FireParticle(int x, int y)
        {
            lifespan = 20;
            age = 0;
            this.x = (a) => x + (float)(Math.Sin(a / 2) * 1.5f);
            this.y = (a) => y - a * 0.25f;
            col = (a) =>
            {
                var f = a / 20f;
                return Color.FromArgb((int)(255 * (1 - f)), 0, 0);
            };
        }

        static int Lerp(int a, int b, float f)
        {
            return (int)(b * f + a * (1 - f));
        }
    }
}

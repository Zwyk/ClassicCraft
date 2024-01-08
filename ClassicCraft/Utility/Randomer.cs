using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Randomer
    {
        private static Random random = new Random();

        public static int Next(int min, int max)
        {
            lock (random)
            {
                return random.Next(min, max);
            }
        }
        public static int Next(double min, double max)
        {
            return Next((int)min,(int)max);
        }

        public static double NextDouble()
        {
            lock (random)
            {
                return random.NextDouble();
            }
        }
    }
}

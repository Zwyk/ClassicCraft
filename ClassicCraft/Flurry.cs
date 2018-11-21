using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Flurry : Buff
    {
        public Flurry(double start, double end, int stacks = 3)
            : base(start, end, stacks)
        {
        }


        public override string ToString()
        {
            return "Flurry";
        }
    }
}

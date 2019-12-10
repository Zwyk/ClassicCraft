using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class OmenBuff : Effect
    {
        public static double PROC_RATE = 0.06;

        public static int LENGTH = 15;

        public OmenBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override string ToString()
        {
            return "Omen of Clarity Buff";
        }
    }
}

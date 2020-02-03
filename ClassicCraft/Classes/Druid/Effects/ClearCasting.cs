using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ClearCasting : Effect
    {
        public override string ToString() { return NAME; } public static new string NAME = "Clearcasting";

        public static double PROC_RATE = 0.06;
        public static int LENGTH = 15;

        public ClearCasting(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public static void CheckProc(Player p)
        {
            if (Randomer.NextDouble() < PROC_RATE)
            {
                if (p.Effects.ContainsKey(NAME))
                {
                    p.Effects[NAME].Refresh();
                }
                else
                {
                    new ClearCasting(p).StartEffect();
                }
            }
        }
    }
}

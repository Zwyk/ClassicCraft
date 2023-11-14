using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowTrance : Effect
    {
        public override string ToString() { return NAME; } public static new string NAME = "Shadow Trance";

        public static double PROC_RATE_BY_RANK = 0.02;
        public static int LENGTH = 10;

        public ShadowTrance(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public static double ProcRate(Player p)
        {
            return PROC_RATE_BY_RANK * p.GetTalentPoints("NF");
        }

        public static void CheckProc(Player p)
        {
            if (Randomer.NextDouble() < ProcRate(p))
            {
                if (p.Target.Effects.ContainsKey(NAME))
                {
                    p.Target.Effects[NAME].Refresh();
                }
                else
                {
                    new ShadowTrance(p).StartEffect();
                }
            }
        }
    }
}

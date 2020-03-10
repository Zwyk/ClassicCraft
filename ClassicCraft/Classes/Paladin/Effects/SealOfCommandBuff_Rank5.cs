using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SealOfCommandBuff_Rank5 : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "--- BUFF --- SoC (Rank 5)";

        public static int LENGTH = 30;

        public SealOfCommandBuff_Rank5(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public static void CheckProc(Player p, ResultType type, int points)
        {
            if (type == ResultType.Hit || type == ResultType.Crit || type == ResultType.Glance)
            {
                //  SoC is normalized by 7 procs-per-minute using the following formula
                double procRate = 7 * p.MH.Speed / 60;
                if (Randomer.NextDouble() < procRate)
                {
                    //  It proc'ed, so see if it hits/does damage
                    new SealOfCommandProc(p, p.Sim.Boss).DoAction();
                }
            }
        }

        public override void StartEffect()
        {
            base.StartEffect();
        }

        public override void EndEffect()
        {
            base.EndEffect();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SealOfRighteousnessBuff : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "--- BUFF --- SoR";

        public static int LENGTH = 30;

        public SealOfRighteousnessBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public static void CheckProc(Player p, ResultType type, int points)
        {
            if (type == ResultType.Hit || type == ResultType.Crit || type == ResultType.Glance)
            {
                //  SoR procs on every hit
                new SealOfRighteousnessProc(p, p.Sim.Boss).DoAction();
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

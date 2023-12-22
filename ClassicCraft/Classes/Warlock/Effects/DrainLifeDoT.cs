using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class DrainLifeDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Drain Life";

        public static double DURATION = 15;
        public static double RATIO = 0.5;
        public static int TICK_DELAY = 1;
        public static int NB_TICKS = (int)(DURATION / TICK_DELAY);

        public override double BaseDmg()
        {
            return DrainLife.DMG(Player.Level) * DURATION / DrainLife.CAST_TIME;
        }

        public DrainLifeDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, RATIO, TICK_DELAY, 1, School.Shadow, 1 + (p.Tanking ? 1.5 * 0.5 : 0) / p.Sim.NbTargets)
        {
        }
    }
}

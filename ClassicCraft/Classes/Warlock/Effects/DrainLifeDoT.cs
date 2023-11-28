using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DrainLifeDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Drain Life";

        public static double DURATION = 15;
        public static double RATIO = 1;
        public static int TICK_DELAY = 1;
        public static int NB_TICKS = (int)(DURATION / TICK_DELAY);

        public override double BaseDmg()
        {
            return DMG(Player.Level);
        }

        public static int DMG(int level)
        {
            if (level >= 54) return 71 * 15;
            else if (level >= 46) return 55 * 15;
            else if (level >= 38) return 41 * 15;
            else if (level >= 30) return 29 * 15;
            else if (level >= 22) return 17 * 15;
            else if (level >= 14) return 10 * 15;
            else return 0;
        }

        public DrainLifeDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, RATIO, TICK_DELAY, 1, School.Shadow)
        {
        }
    }
}

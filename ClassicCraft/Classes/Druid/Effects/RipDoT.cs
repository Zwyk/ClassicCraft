using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class RipDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rip";

        public override double BaseDmg()
        {
            return dmg[Player.Combo-1];
        }

        // rank 1
        public static int[] dmg =
        {
            42,
            66,
            90,
            114,
            138,
        };

        public static double DURATION = 12;

        public static int TICK_DELAY = 2;

        public static double RATIO = 0;

        public RipDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, RATIO, TICK_DELAY, 1, School.Physical)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class RendDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rend";

        public static double DURATION = 15;
        public static double RATIO = 1;
        public static double TICK_DELAY = 3;
        public static int NB_TICKS = (int)(DURATION / TICK_DELAY);

        public override double BaseDmg()
        {
            return DMG(Player.Level);
        }

        public static int DMG(int level)
        {
            return 45;
        }

        public RendDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, 0, 3, 1, School.Physical)
        {
        }
    }
}

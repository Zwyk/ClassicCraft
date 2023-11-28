using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DevouringPlagueDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Devouring Plague";

        public static double DURATION = 24;
        public static double RATIO = 1;
        public static int NB_TICKS = (int)(DURATION / 3);

        public override double BaseDmg()
        {
            return DMG(Player.Level);
        }

        public static int DMG(int level)
        {
            return 904;
        }

        public DevouringPlagueDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, RATIO, 3, 1, School.Shadow)
        {
        }
    }
}

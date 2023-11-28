using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SWPDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "SW:P";

        public static double DURATION = 18;
        public static double RATIO = 1;
        public static int NB_TICKS = (int)(DURATION / 3);

        public override double BaseDmg()
        {
            return DMG(Player.Level);
        }

        public static int DMG(int level)
        {
            return 852;
        }

        public SWPDoT(Player p, Entity target)
            : base(p, target, false, DURATION + 3 * p.GetTalentPoints("ISWP"), 1, RATIO, 3, 1, School.Shadow)
        {
        }
    }
}

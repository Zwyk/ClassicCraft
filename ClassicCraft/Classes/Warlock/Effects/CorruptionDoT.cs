using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassicCraft
{
    class CorruptionDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Corruption";

        public static double DURATION(int level)
        {
            if (level >= 24) return 18;
            else if (level >= 14) return 15;
            else return 12;
        }

        public static double RATIO = 1;
        public static int TICK_DELAY = 3;
        public static int NB_TICKS(int level)
        {
            return (int)(DURATION(level) / TICK_DELAY);
        }

        public override double BaseDmg()
        {
            return DMG(Player.Level);
        }

        public static int DMG(int level)
        {
            if (level >= 60) return 822;
            else if (level >= 54) return 666;
            else if (level >= 44) return 486;
            else if (level >= 34) return 324;
            else if (level >= 24) return 222;
            else if (level >= 14) return 90;
            else if (level >= 4) return 40;
            else return 0;
        }

        public CorruptionDoT(Player p, Entity target)
            : base(p, target, false, DURATION(p.Level), 1, RATIO, 3, 1, School.Shadow)
        {
        }

        public override void ApplyTick(int damage)
        {
            base.ApplyTick(damage);

            if(Player.GetTalentPoints("NF") > 0)
            {
                ShadowTrance.CheckProc(Player);
            }
        }
    }
}

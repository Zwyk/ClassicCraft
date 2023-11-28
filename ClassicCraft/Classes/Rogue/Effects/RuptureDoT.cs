using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class RuptureDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rupture";
        public override double BaseDmg()
        {
            return DMG(Player.Level, Player.Combo);
        }

        public static int DMG(int level, int combo)
        {
            return BASE_DMG[combo-1];
        }

        public static double[] DURATION =
        {
            8,
            10,
            12,
            14,
            16,
        };

        public static int[] BASE_DMG =
        {
            324,
            460,
            618,
            798,
            1000,
        };

        public static double[] AP_RATIO =
        {
            0.04,
            0.10,
            0.18,
            0.21,
            0.24,
        };

        public static double DurationCalc(Player p)
        {
            return p.Combo == 0 ? 0 : DURATION[p.Combo - 1];
        }

        public RuptureDoT(Player p, Entity target)
            : base(p, target, false, DurationCalc(p), 1, AP_RATIO[p.Combo - 1], 2, 1, School.Physical)
        {
            BonusDmg = Player.NbSet("Deathmantle") >= 2 ? 40 : 0;
        }
    }
}

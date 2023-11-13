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

        public int Combo;

        public static double[] DURATION =
        {
            8,
            10,
            12,
            14,
            16,
        };

        public static double[] BASE_DMG =
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
            : base(p, target, false, DURATION[0], 1, 2)
        {
        }

        public override void StartEffect()
        {
            Combo = Player.Combo;
            Duration = DURATION[Combo - 1];
            base.StartEffect();
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((BASE_DMG[Combo - 1] + Player.AP * AP_RATIO[Combo - 1] + (Player.NbSet("Deathmantle") >= 2 ? 40 : 0)) / (Duration / TickDelay)
                * Player.DamageMod
                * (1 + (0.01 * Player.GetTalentPoints("Murder")))
                );
        }

        public override double GetExternalModifiers()
        {
            return base.GetExternalModifiers()
                * ((Target.Effects.ContainsKey("Mangle") ? 1.3 : 1)
                );
        }
    }
}

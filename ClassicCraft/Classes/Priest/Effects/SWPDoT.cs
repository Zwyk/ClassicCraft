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

        public static double DMG = 852;

        public SWPDoT(Player p, Entity target)
            : base(p, target, false, DURATION + 3 * p.GetTalentPoints("ISWP"))
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("Darkness"))
                * 1.15 // shadow form
                * Player.DamageMod
                );
        }

        public override double GetExternalModifiers()
        {
            return base.GetExternalModifiers()
                * (Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Target.Effects[ShadowVulnerability.NAME]).Modifier : 1
                * 1.15 // shadow weaving
                );
        }
    }
}

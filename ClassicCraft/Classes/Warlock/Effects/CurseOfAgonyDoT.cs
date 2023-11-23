using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class CurseOfAgonyDoT : EffectOnTime
    {
        public override string ToString() { return NAME; } public static new string NAME = "Curse of Agony";

        public static double DURATION = 24;
        public static double RATIO = 1;
        public static int TICK_DELAY = 2;
        public static int NB_TICKS = (int)(DURATION / TICK_DELAY);

        public int DMG(int level)
        {
            if (level >= 58) return 1044;
            else if (level >= 48) return 780;
            else if (level >= 38) return 504;
            else if (level >= 28) return 324;
            else if (level >= 18) return 180;
            else if (level >= 8) return 84;
            else return 0;
        }

        public CurseOfAgonyDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, 2)
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG(Player.Level) + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("ICA"))
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * Math.Max(Player.Tanking ? 0 : (1 + 0.15 * Player.GetTalentPoints("DS")), (1 + 0.02 * Player.GetTalentPoints("MD")) * (1 + 0.03 * Player.GetTalentPoints("SL")))
                * Player.DamageMod
                );
        }

        public override double GetExternalModifiers()
        {
            return base.GetExternalModifiers()
                * (Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Target.Effects[ShadowVulnerability.NAME]).Modifier : 1
                * (Target.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                );
        }
    }
}

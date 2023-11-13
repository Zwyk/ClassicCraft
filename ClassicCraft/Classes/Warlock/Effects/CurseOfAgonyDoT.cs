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

        public int? _DMG;
        public int DMG
        {
            get
            {
                if (!_DMG.HasValue)
                {
                    if (Player.Level >= 58) _DMG = 1044;
                    else if (Player.Level >= 48) _DMG = 780;
                    else if (Player.Level >= 38) _DMG = 504;
                    else if (Player.Level >= 28) _DMG = 324;
                    else if (Player.Level >= 18) _DMG = 180;
                    else if (Player.Level >= 8) _DMG = 84;
                    else _DMG = 0;
                }
                return _DMG.Value;
            }
        }

        public CurseOfAgonyDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, 2)
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("ICA"))
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * (1 + 0.15 * Player.GetTalentPoints("DS"))
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

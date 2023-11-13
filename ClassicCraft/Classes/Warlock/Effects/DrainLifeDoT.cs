using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DrainLifeDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Drain Life";

        public static double DURATION = 15;
        public static double RATIO = 1;
        public static int TICK_DELAY = 1;
        public static int NB_TICKS = (int)(DURATION / TICK_DELAY);

        public int? _DMG;
        public int DMG
        {
            get
            {
                if (!_DMG.HasValue)
                {
                    if (Player.Level >= 54) _DMG = 71 * 5;
                    else if (Player.Level >= 46) _DMG = 55 * 5;
                    else if (Player.Level >= 38) _DMG = 41 * 5;
                    else if (Player.Level >= 30) _DMG = 29 * 5;
                    else if (Player.Level >= 22) _DMG = 17 * 5;
                    else if (Player.Level >= 14) _DMG = 10 * 5;
                    else _DMG = 0;
                }
                return _DMG.Value;
            }
        }

        public DrainLifeDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, TICK_DELAY)
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("IDL"))
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * (1 + 0.15 * Player.GetTalentPoints("DS"))
                * (1 + 0.03 * Player.GetTalentPoints("SL"))
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

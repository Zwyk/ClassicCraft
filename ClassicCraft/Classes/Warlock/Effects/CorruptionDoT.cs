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
        public override string ToString() { return NAME; } public static new string NAME = "Corruption";

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

        public int DMG(int level)
        {
            if (level >= 54) return 71 * 5;
            else if (level >= 46) return 55 * 5;
            else if (level >= 38) return 41 * 5;
            else if (level >= 30) return 29 * 5;
            else if (level >= 22) return 17 * 5;
            else if (level >= 14) return 10 * 5;
            else return 0;
        }

        public CorruptionDoT(Player p, Entity target)
            : base(p, target, false, DURATION(p.Level))
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG(Player.Level) + Player.SP * RATIO) / NB_TICKS(Player.Level)
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * Math.Max(Player.Tanking ? 0 : (1 + 0.15 * Player.GetTalentPoints("DS")), 1 + 0.02 * Player.GetTalentPoints("MD") * (1 + 0.03 * Player.GetTalentPoints("SL")))
                * Player.DamageMod
                );
        }

        public override void ApplyTick(int damage)
        {
            base.ApplyTick(damage);

            if(Player.GetTalentPoints("NF") > 0)
            {
                ShadowTrance.CheckProc(Player);
            }
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

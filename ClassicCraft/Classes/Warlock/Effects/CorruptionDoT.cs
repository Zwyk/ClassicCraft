using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class CorruptionDoT : EffectOnTime
    {
        public override string ToString() { return NAME; } public static new string NAME = "Corruption";

        public static double DURATION = 18;
        public static double RATIO = 1;
        public static int NB_TICKS = (int)(DURATION / 3);

        public static double DMG = 666;

        public CorruptionDoT(Player p, Entity target)
            : base(p, target, false, DURATION)
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * (1 + 0.15 * Player.GetTalentPoints("DS"))
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
            return base.GetExternalModifiers() * (Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Target.Effects[ShadowVulnerability.NAME]).Modifier : 1);
        }
    }
}

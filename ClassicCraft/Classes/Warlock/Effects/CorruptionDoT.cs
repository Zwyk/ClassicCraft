﻿using System;
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
        public int NB_TICKS;

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

        public CorruptionDoT(Player p, Entity target)
            : base(p, target, false, 0)
        {
            Duration = DURATION(p.Level);
            NB_TICKS = (int)(Duration / 3);
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
            return base.GetExternalModifiers()
                * (Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Target.Effects[ShadowVulnerability.NAME]).Modifier : 1
                * (Target.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                );
        }
    }
}

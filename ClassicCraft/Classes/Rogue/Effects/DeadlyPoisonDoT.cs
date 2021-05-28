using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeadlyPoisonDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Deadly Poison";

        public static double DURATION = 12;
        public static int NB_TICKS = (int)(DURATION / 3);

        public static double DMG = 180;

        public DeadlyPoisonDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, 3, 5)
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round(DMG * CurrentStacks / NB_TICKS
                * Player.DamageMod
                * (1 + 0.01 * Player.GetTalentPoints("Murder"))
                * (1 + 0.04 * Player.GetTalentPoints("VP"))
                );
        }

        public override double GetExternalModifiers()
        {
            return (base.GetExternalModifiers()
                );
        }
    }
}

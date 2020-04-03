using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DevouringPlagueDoT : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Devouring Plague";

        public static double DURATION = 24;
        public static double RATIO = 1;
        public static int NB_TICKS = (int)(DURATION / 3);

        public static double DMG = 904;

        public DevouringPlagueDoT(Player p, Entity target)
            : base(p, target, false, DURATION)
        {
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("Darkness"))
                * 1.15 // shadow weaving
                * 1.15 // shadow form
                * Player.DamageMod
                );
        }
    }
}

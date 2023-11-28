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
        public static int TICK_DELAY = 3;
        public static int MAX_STACKS = 5;

        public static int RATIO = 0;

        public override double BaseDmg()
        {
            return DMG(Player.Level);
        }

        public static int DMG(int level)
        {
            return 180;
        }

        public DeadlyPoisonDoT(Player p, Entity target)
            : base(p, target, false, DURATION, 1, RATIO, TICK_DELAY, MAX_STACKS)
        {
        }
    }
}

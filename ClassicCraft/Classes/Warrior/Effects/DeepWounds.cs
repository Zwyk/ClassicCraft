using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeepWounds : EffectOnTime
    {
        public override string ToString() { return NAME; } public static new string NAME = "Deep Wounds";

        public override double BaseDmg()
        {
            int minDmg = (int)Math.Round(Player.MH.DamageMin + Player.MH.Speed * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Player.MH.Speed * Player.AP / 14);

            return (minDmg + maxDmg) / 2.0;
        }

        public static double DURATION = 12;

        public static int TICK_DELAY = 3;

        public static double RATIO = 1.0 / 14;

        public DeepWounds(Player p, int points, Entity target)
            : base(p, target, false, DURATION, 1, RATIO, TICK_DELAY, 1, School.Physical)
        {
            Ratio = 0.2 * points;
        }

        public static void CheckProc(Player p, ResultType type, int points)
        {
            if (type == ResultType.Crit)
            {
                if (p.Target.Effects.ContainsKey(NAME))
                {
                    p.Target.Effects[NAME].Refresh();
                }
                else
                {
                    new DeepWounds(p, points, p.Target).StartEffect();
                }
            }
        }
    }
}

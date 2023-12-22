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

        public static double RATIO = 1;

        public DeepWounds(Player p, Entity target)
            : base(p, target, false, DURATION, 1, RATIO * 0.2 * p.GetTalentPoints("DW"), TICK_DELAY, 1, School.Physical)
        {
        }

        public static void CheckProc(Player p, ResultType type)
        {
            if (type == ResultType.Crit && p.GetTalentPoints("DW") > 0)
            {
                if (p.Target.Effects.ContainsKey(NAME))
                {
                    p.Target.Effects[NAME].Refresh();
                }
                else
                {
                    new DeepWounds(p, p.Target).StartEffect();
                }
            }
        }
    }
}

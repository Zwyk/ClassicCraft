using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClassicCraft
{
    class ConsumedByRage : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Consumed by Rage";

        public static double DURATION = 12;

        public static double DAMAGE_MOD = 1.2;

        public ConsumedByRage(Player p)
            : base(p, p, true, DURATION, 12, 12)
        {
        }

        public static void OnHit(Player p, ResultType res)
        {
            if (p.Effects.ContainsKey(NAME)
                && !new List<ResultType>() { ResultType.Miss, ResultType.Parry, ResultType.Dodge }.Contains(res))
            {
                p.Effects[NAME].StackRemove();
            }
        }

        public static void CheckProc(Player p, int oldRage, int newRage)
        {
            if (oldRage < 80 && newRage >= 80)
            {
                if (p.Effects.ContainsKey(NAME))
                {
                    p.Effects[NAME].Refresh();
                }
                else
                {
                    new ConsumedByRage(p).StartEffect();
                }
            }
        }
    }
}

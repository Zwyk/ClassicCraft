using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SavageRoarBuff : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Savage Roar";

        public static double[] DURATION =
        {
            14,
            19,
            24,
            29,
            34
        };

        public static double DurationCalc(Player p)
        {
            return DURATION[p.Combo - 1];
        }

        public SavageRoarBuff(Player p, double bonusPct = 30)
            : base(p, p, true, DurationCalc(p), 1)
        {
        }

        public override double CustomDuration()
        {
            return DurationCalc(Player);
        }
    }
}

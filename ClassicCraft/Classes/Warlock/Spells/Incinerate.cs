using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Incinerate : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Incinerate";

        public static Effect NewEffect(Player p, Entity t)
        {
            return new CustomEffect(p, t, NAME, false, 15);
        }

        public static int BASE_COST(int level)
        {
            return 0;
        }

        public static int CD = 0;
        public static double CAST_TIME = 2.25;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public static double THREAT_RATIO = 1;

        public static int MAX_TARGETS = 1;

        public static int MIN_DMG(int level)
        {
            return 222;
        }

        public static int MAX_DMG(int level)
        {
            return 258;
        }

        public Incinerate(Player p)
            : base(p, CD, (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, true, School.Fire, CAST_TIME, MAX_TARGETS, THREAT_RATIO, new EndDmg(MIN_DMG(p.Level), MAX_DMG(p.Level), RATIO), new EndEffect(NAME), null)
        {
        }
    }
}

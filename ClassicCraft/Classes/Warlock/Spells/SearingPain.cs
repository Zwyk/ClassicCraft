using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SearingPain : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Searing Pain";

        public static int BASE_COST(int level)
        {
            if (level >= 60) return 168;
            else if (level >= 50) return 141;
            else if (level >= 42) return 118;
            else if (level >= 34) return 91;
            else if (level >= 26) return 68;
            else if (level >= 18) return 45;
            else return 0;
        }

        public static int CD = 0;
        public static double CAST_TIME = 1.5;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public static double THREAT_RATIO = 2;

        public static int MAX_TARGETS = 1;

        public static int MIN_DMG(int level)
        {
            if (level >= 60) return 208;
            else if (level >= 50) return 168;
            else if (level >= 42) return 131;
            else if (level >= 34) return 93;
            else if (level >= 26) return 65;
            else if (level >= 18) return 38;
            else return 0;
        }

        public static int MAX_DMG(int level)
        {
            if (level >= 60) return 244;
            else if (level >= 50) return 199;
            else if (level >= 42) return 155;
            else if (level >= 34) return 112;
            else if (level >= 26) return 77;
            else if (level >= 18) return 47;
            else return 0;
        }

        public SearingPain(Player p)
            : base(p, CD, (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, true, School.Fire, CAST_TIME, MAX_TARGETS, THREAT_RATIO, new EndDmg(MIN_DMG(p.Level), MAX_DMG(p.Level), RATIO), null, null)
        {
        }

        public override void Cast(Entity t)
        {
            Cast(t, Player.Form == Player.Forms.Metamorphosis, false);
        }
    }
}

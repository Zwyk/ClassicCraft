using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowCleave : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shadow Cleave";

        public static int CD = 6;

        public static double CAST_TIME = 0;

        public static double BASE_RATIO = Math.Max(1.5, CAST_TIME) / 3.5;
        public static double RATIO = 2.0/3.0;

        public static int BASE_COST(int level)
        {
            if (level >= 60) return 190;
            //else if (level >= 60) return 185; // Rank 9
            else if (level >= 52) return 157;
            else if (level >= 44) return 132;
            else if (level >= 36) return 105;
            else if (level >= 28) return 80;
            else if (level >= 20) return 55;
            else if (level >= 12) return 35;
            else if (level >= 6) return 20;
            else return 12;
        }

        public static double SB_CAST_TIME(int level)
        {
            if (level >= 20) return 3;
            else if (level >= 12) return 2.8;
            else if (level >= 6) return 2.2;
            else return 1.7;
        }

        public static int MIN_DMG(int level)
        {
            if (level >= 60) return 136;
            //else if (level >= 60) return 128; // Rank 9
            else if (level >= 52) return 105;
            else if (level >= 44) return 82;
            else if (level >= 36) return 60;
            else if (level >= 28) return 42;
            else if (level >= 20) return 26;
            else if (level >= 12) return 14;
            else if (level >= 6) return 7;
            else return 3;
        }

        public static int MAX_DMG(int level)
        {
            if (level >= 60) return 204;
            //else if (level >= 60) return 193; // Rank 9
            else if (level >= 52) return 158;
            else if (level >= 44) return 124;
            else if (level >= 36) return 91;
            else if (level >= 28) return 64;
            else if (level >= 20) return 39;
            else if (level >= 12) return 23;
            else if (level >= 6) return 12;
            else return 7;
        }

        public static int MAX_TARGETS = 3;

        public ShadowCleave(Player p)
            : base(p, CD, School.Shadow,
                  new SpellData(SpellType.Magical, (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, CAST_TIME, SMI.Reset, MAX_TARGETS),
                  new EndDmg(MIN_DMG(p.Level), MAX_DMG(p.Level), BASE_RATIO * RATIO, RatioType.SP))
        {
        }
    }
}

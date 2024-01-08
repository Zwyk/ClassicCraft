using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Corruption : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Corruption";

        public static int BASE_COST(int level)
        {
            if (level >= 60) return 340;
            else if (level >= 54) return 290;
            else if (level >= 44) return 225;
            else if (level >= 34) return 160;
            else if (level >= 24) return 100;
            else if (level >= 14) return 55;
            else if (level >= 4) return 35;
            else return 0;
        }
        public static int CD = 0;
        public static double CAST_TIME = 2;

        public Corruption(Player p)
            : base(p, CD, School.Shadow, 
                  new SpellData(SpellType.Magical, BASE_COST(p.Level), true, CAST_TIME - 0.4 * p.GetTalentPoints("IC")),
                  null,
                  new EndEffect(CorruptionDoT.NAME))
        {
        }
    }
}

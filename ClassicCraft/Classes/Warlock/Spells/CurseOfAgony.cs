using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class CurseOfAgony : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Curse of Agony";

        public static int BASE_COST(int level)
        {
            if (level >= 58) return 215;
            else if (level >= 48) return 170;
            else if (level >= 38) return 130;
            else if (level >= 28) return 90;
            else if (level >= 18) return 50;
            else if (level >= 8) return 25;
            else return 0;
        }
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public CurseOfAgony(Player p)
            : base(p, CD, School.Shadow,
                  new SpellData(SpellType.Magical, BASE_COST(p.Level), true, CAST_TIME, SMI.Reset),
                  null,
                  new EndEffect(CurseOfAgonyDoT.NAME), null)
        {
        }
    }
}

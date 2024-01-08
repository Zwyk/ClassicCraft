using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SWP : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "SW:P";

        public static int BASE_COST = 470;
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public SWP(Player p)
            : base(p, CD, School.Shadow,
                  new SpellData(SpellType.Magical, BASE_COST, true, CAST_TIME, SMI.Reset),
                  null, 
                  new EndEffect(SWPDoT.NAME), null)
        {
        }
    }
}

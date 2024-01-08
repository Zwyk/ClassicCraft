using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class InnerFocus : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Inner Focus";

        public static int BASE_COST = 0;
        public static int CD = 180;

        public static double CAST_TIME = 0;

        public InnerFocus(Player p)
            : base(p, CD, School.Magical,
                  new SpellData(SpellType.Magical, BASE_COST, false, CAST_TIME, SMI.Reset),
                  null,
                  new EndEffect(InnerFocusBuff.NAME), null)
        {
        }
    }
}

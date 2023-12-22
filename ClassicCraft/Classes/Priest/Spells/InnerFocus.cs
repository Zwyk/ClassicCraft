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

        public InnerFocus(Player p)
            : base(p, CD, BASE_COST, true, false, School.Magical, 0, 1, 1, null, new EndEffect(InnerFocusBuff.NAME), null)
        {
        }
    }
}

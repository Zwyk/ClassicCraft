using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Innervate : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Innervate";

        public static int CD = 360;

        public Innervate(Player p)
               : base(p, CD, (int)(p.BaseMana * 0.05), true, true, School.Magical, 0, 1, 1, null, new EndEffect(InnervateBuff.NAME), null)
        {
        }
    }
}

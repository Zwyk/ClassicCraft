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

        public static double COST_PCT = 0.05;

        public static int CD = 360;

        public Innervate(Player p)
               : base(p, CD, School.Magical,
                     new SpellData(SpellType.Magical, (int)(p.BaseMana * COST_PCT), true, 0, SMI.Reset),
                     null,
                     new EndEffect(InnervateBuff.NAME))
        {
        }
    }
}

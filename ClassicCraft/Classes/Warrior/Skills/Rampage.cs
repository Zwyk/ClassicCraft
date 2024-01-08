using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rampage : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rampage";

        public static int BASE_COST = 20;
        public static int CD = 0;

        public Rampage(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  null,
                  new EndEffect(RampageBuff.NAME))
        {
        }
    }
}

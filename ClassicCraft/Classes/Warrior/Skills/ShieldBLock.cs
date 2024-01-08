using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShieldBlock : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shield Block";

        public static int CD = 5;
        public static int BASE_COST = 10;

        public ShieldBlock(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, false))
        {
        }
    }
}

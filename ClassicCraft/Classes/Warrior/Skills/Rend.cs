using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rend : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rend";

        public static int BASE_COST = 10;
        public static int CD = 0;

        public Rend(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  null,
                  new EndEffect(RendDoT.NAME), null)
        {
        }
    }
}

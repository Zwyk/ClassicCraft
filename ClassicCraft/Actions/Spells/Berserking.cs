using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Berserking : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Berserking";

        public static int BASE_COST = 5;
        public static int CD = 180;

        public Berserking(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, false),
                  null,
                  new EndEffect(BerserkingBuff.NAME))
        {
        }
    }
}

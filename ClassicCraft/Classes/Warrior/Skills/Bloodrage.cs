using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodrage : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Bloodrage";

        public static int BASE_COST = 0;
        public static int CD = 60;

        public Bloodrage(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, false),
                  null,
                  new EndEffect(BloodrageBuff.NAME))
        {
        }
    }
}

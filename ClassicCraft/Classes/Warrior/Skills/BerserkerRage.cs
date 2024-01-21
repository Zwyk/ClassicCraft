using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BerserkerRage : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Berserker Rage";

        public static int BASE_COST = 0;
        public static int CD = 30;

        public BerserkerRage(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  null,
                  new EndEffect(BerserkerRageBuff.NAME))
        {
        }
    }
}

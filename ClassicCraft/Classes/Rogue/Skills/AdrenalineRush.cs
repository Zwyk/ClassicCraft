using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class AdrenalineRush : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Adrenaline Rush";

        public static int BASE_COST = 0;
        public static int CD = 300;

        public AdrenalineRush(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  null,
                  new EndEffect(AdrenalineRushBuff.NAME))
        {
        }
    }
}

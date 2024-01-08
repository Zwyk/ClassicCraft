using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SweepingStrikes : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Sweeping Strikes";

        public static int BASE_COST = 30;
        public static int CD = 30;

        public SweepingStrikes(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, false),
                  null,
                  new EndEffect(SweepingStrikesBuff.NAME))
        {
        }
    }
}

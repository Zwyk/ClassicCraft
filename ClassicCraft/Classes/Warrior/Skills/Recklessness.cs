using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Recklessness : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Recklessness";

        public static int BASE_COST = 0;
        public static int CD = 300;

        public Recklessness(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  null,
                  new EndEffect(RecklessnessBuff.NAME))
        {
        }
    }
}

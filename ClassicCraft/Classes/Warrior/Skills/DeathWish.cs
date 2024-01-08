using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class DeathWish : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Death Wish";

        public static int BASE_COST = 10;
        public static int CD = 180;

        public DeathWish(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  null,
                  new EndEffect(DeathWishBuff.NAME))
        {
        }
    }
}

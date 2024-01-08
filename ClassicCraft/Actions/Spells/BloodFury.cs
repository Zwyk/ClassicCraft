using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BloodFury : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Blood Fury";

        public static int BASE_COST = 0;
        public static int CD = 120;

        public BloodFury(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, false),
                  null,
                  new EndEffect(BloodFuryBuff.NAME))
        {
        }
    }
}

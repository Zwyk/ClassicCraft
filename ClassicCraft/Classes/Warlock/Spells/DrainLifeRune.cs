using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DrainLifeRune : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Drain Life";

        public static double CAST_TIME = 0;
        public static double RUNE_COST_RATIO = 2;

        public static int CD = 15;

        public DrainLifeRune(Player p)
            : base(p, CD, School.Shadow,
                  new SpellData(SpellType.Magical, (int)(DrainLife.BASE_COST(p.Level) * RUNE_COST_RATIO), true, CAST_TIME, SMI.Reset),
                  null,
                  new EndEffect(DrainLifeDoT.NAME))
        {
        }
    }
}

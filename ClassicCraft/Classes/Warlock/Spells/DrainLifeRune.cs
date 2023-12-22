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
            : base(p, CD, (int)(DrainLife.BASE_COST(p.Level) * RUNE_COST_RATIO), true, true, School.Shadow, CAST_TIME, 1, 1, null, new EndEffect(DrainLifeDoT.NAME), null)
        {
        }
    }
}

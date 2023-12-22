using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DrainLife : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Drain Life";

        public static int CD = 0;
        public static double CAST_TIME = 5;
        public static double TICK_DELAY = 1;
        public static int NB_TICKS = (int)(CAST_TIME / TICK_DELAY);

        public static double RATIO = 0.5;

        public static int BASE_COST(int level)
        {
            if (level >= 54) return 300;
            else if (level >= 46) return 240;
            else if (level >= 38) return 185;
            else if (level >= 30) return 135;
            else if (level >= 22) return 85;
            else if (level >= 14) return 55;
            else return 0;
        }

        public static int DMG(int level)
        {
            if (level >= 54) return (int)(71 * CAST_TIME);
            else if (level >= 46) return (int)(55 * CAST_TIME);
            else if (level >= 38) return (int)(41 * CAST_TIME);
            else if (level >= 30) return (int)(29 * CAST_TIME);
            else if (level >= 22) return (int)(17 * CAST_TIME);
            else if (level >= 14) return (int)(10 * CAST_TIME);
            else return 0;
        }

        public DrainLife(Player p)
            : base(p, CD, BASE_COST(p.Level), true, true, School.Shadow, CAST_TIME, 1, p.Tanking ? 1.5 : 1, null, null, new ChannelDmg(DMG(p.Level), TICK_DELAY, RATIO))
        {
        }
    }
}

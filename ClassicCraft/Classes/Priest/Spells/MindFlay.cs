using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MindFlay : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Mind Flay";

        public static int BASE_COST = 205;
        public static int CD = 0;
        public static double CAST_TIME = 3;
        public static double TICK_DELAY = 1;
        public static int NB_TICKS = (int)(CAST_TIME/TICK_DELAY);

        public static double RATIO = 0.457;

        public static int DMG = 426;

        public MindFlay(Player p)
            : base(p, CD, BASE_COST, true, true, School.Shadow, CAST_TIME, 1, 1, null, null, new ChannelDmg(DMG, TICK_DELAY, RATIO))
        {
        }
    }
}

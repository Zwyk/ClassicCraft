using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MindFlay : ChannelSpell
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
            : base(p, CD, BASE_COST, true, true, School.Shadow, CAST_TIME)
        {
        }

        public override void Cast()
        {
            StartCast();
        }

        public override int GetTickDamage()
        {
            return (int)Math.Round((DMG + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("Darkness"))
                * 1.15 // shadow weaving
                * 1.15 // shadow form
                * Player.DamageMod
                );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MindBlast : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Mind Blast";

        public static int BASE_COST = 350;
        public static int CD = 8;
        public static double CAST_TIME = 1.5;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public static int MIN_DMG = 508;
        public static int MAX_DMG = 537;

        public MindBlast(Player p)
            : base(p, CD - 0.5 * p.GetTalentPoints("IMB"), BASE_COST, true, true, School.Shadow, CAST_TIME, 1, 1, new EndDmg(MIN_DMG, MAX_DMG, RATIO), null, null)
        {
        }
    }
}

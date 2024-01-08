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
            : base(p, CD, School.Shadow,
                  new SpellData(SpellType.Magical, BASE_COST, true, CAST_TIME, SMI.Reset),
                  new EndDmg(MIN_DMG, MAX_DMG, RATIO, RatioType.SP))
        {
        }
    }
}

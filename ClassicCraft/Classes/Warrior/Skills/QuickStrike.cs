using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class QuickStrike : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Quick Strike";

        public static double AP_RATIO_MIN = 0.15;
        public static double AP_RATIO_MAX = 0.25;

        public static int BASE_COST = 30;
        public static int CD = 6;

        public QuickStrike(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") + (p.NbSet("Destroyer") >= 4 ? 5 : 0) : 0)),
                  new EndDmg(0, 0, 0, RatioType.AP))
        {
        }

        public override double GetEndDmgBase(bool mh = true)
        {
            return Randomer.Next(AP_RATIO_MIN, AP_RATIO_MAX + 1) * Player.AP;
        }
    }
}

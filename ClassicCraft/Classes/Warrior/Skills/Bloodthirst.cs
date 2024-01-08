using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodthirst : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Bloodthirst";

        public static double AP_RATIO = 0.45;

        public static int BASE_COST = 30;
        public static int CD = 6;

        public Bloodthirst(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") + (p.NbSet("Destroyer") >= 4 ? 5 : 0) : 0)),
                  new EndDmg(0, 0, AP_RATIO, RatioType.AP))
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Revenge : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Revenge";

        public static int BASE_COST = 5;
        public static int CD = 5;

        public static int DMG_MIN = Program.version == Version.TBC ? 414 : 81;
        public static int DMG_MAX = Program.version == Version.TBC ? 506 : 99;

        public static int BONUS_THREAT = Program.version == Version.TBC ? 200 : 355;

        public Revenge(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), true, 0, SMI.None, 1, 1, BONUS_THREAT),
                  new EndDmg(DMG_MIN + (p.NbSet("Dreadnaught") >= 2 ? 75 : 0), DMG_MAX + (p.NbSet("Dreadnaught") >= 2 ? 75 : 0), 0, RatioType.None)) { }

    }
}

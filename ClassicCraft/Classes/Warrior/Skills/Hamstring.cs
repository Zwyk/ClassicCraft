using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Hamstring : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Hamstring";

        public static int CD = 0;
        public static int BASE_COST = 10;

        public static int DMG = Program.version == Version.TBC ? 63 : 45;

        public static int BONUS_THREAT = 141;

        public Hamstring(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), true, 0, SMI.None, 1, 1, BONUS_THREAT),
                  new EndDmg(DMG, DMG, 0, RatioType.None))
        {
        }
    }
}

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

        public static int DMG(int level)
        {
            if (level >= 70) return 63;
            else if (level >= 54) return 45;
            else if (level >= 32) return 18;
            else if (level >= 8) return 5;
            else return 0;
        }

        public static int BONUS_THREAT(int level)
        {
            if (level >= 70) return 141;
            else if (level >= 54) return 141;
            else return DMG(level) * 3;    // ?
        }

        public Hamstring(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), true, 0, SMI.None, 1, 1, BONUS_THREAT(p.Level)),
                  new EndDmg(DMG(p.Level), DMG(p.Level), 0, RatioType.None))
        {
        }
    }
}

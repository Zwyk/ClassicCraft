using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SunderArmor : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Sunder Armor";

        public static int BASE_COST = 15;
        public static int CD = 0;

        public static int BONUS_THREAT = Program.version == Version.TBC ? 301 : 260;

        public SunderArmor(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("ISA") - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), true, 0, SMI.None, 1, 1, BONUS_THREAT))
        {
        }
    }
}

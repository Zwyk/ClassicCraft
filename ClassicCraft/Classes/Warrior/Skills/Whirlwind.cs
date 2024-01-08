using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Whirlwind : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Whirlwind";

        public static int BASE_COST = 25;
        public static int CD = 10;

        public static int MAX_TARGETS = 4;

        public Whirlwind(Player p)
            : base(p, CD - (Program.version == Version.TBC ? p.GetTalentPoints("IWW") : 0), School.Physical, 
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") + (p.NbSet("Warbringer")>=2?5:0) : 0), true, 0, SMI.None, MAX_TARGETS),
                  new EndDmg(p.MH.DamageMin, p.MH.DamageMax, 1/14.0, Program.version == Version.TBC ? RatioType.WeaponDual : RatioType.WeaponMH), null, null)
        {
        }
    }
}

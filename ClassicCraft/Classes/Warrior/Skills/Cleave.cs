using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Cleave : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Cleave";

        public static int BASE_COST = 20;
        public static int CD = 0;
        
        public static int BONUS_DMG = Program.version == Version.TBC ? 70 : 50;

        public Cleave(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), false, 0, SMI.UseOnNextMHSwing, 2),
                  new EndDmg(p.MH.DamageMin + BONUS_DMG, p.MH.DamageMax + BONUS_DMG, 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

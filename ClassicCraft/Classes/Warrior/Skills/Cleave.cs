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

        public static int BONUS_DMG(int level)
        {
            if (level >= 70) return 70;
            else if (level >= 60) return 50;
            else if (level >= 50) return 32;
            else if (level >= 40) return 18;
            else if (level >= 30) return 10;
            else if (level >= 20) return 5;
            else return 0;
        }

        public Cleave(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), false, 0, SMI.UseOnNextMHSwing, 2),
                  new EndDmg(p.MH.DamageMin + BONUS_DMG(p.Level), p.MH.DamageMax + BONUS_DMG(p.Level), 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HeroicStrike : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Heroic Strike";

        public static int BASE_COST = 15;
        public static int CD = 0;

        public static int BONUS_THREAT = Program.version == Version.TBC ? 196 : 175;
        public static int BONUS_DMG = Program.version == Version.TBC ? 208 : 157;

        public HeroicStrike(Player p)
            : base(p, CD, School.Physical, 
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("IHS") - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), false, 0, SMI.UseOnNextMHSwing, 1, 1, BONUS_THREAT),
                  new EndDmg(p.MH.DamageMin + BONUS_DMG, p.MH.DamageMax + BONUS_DMG, 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

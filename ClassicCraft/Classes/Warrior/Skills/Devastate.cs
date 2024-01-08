using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Devastate : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Devastate";

        public static int BASE_COST = 15;
        public static int CD = 0;

        public static int BONUS_THREAT = 101;

        public static double WEAPON_DMG_RATIO = 0.5;

        public static int BASE_DMG = 35;
        public static int TARGET_SUNDERS = 5;

        public Devastate(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("FR"), true, 0, SMI.None, 1, 1, BONUS_THREAT),
                  new EndDmg(p.MH.DamageMin * 0.5, p.MH.DamageMax * 0.5, 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

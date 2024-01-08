using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShieldSlam : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shield Slam";

        public static int BASE_COST = 20;
        public static int CD = 6;

        public static int BONUS_THREAT = 310;

        public static int DMG_MIN = 420;
        public static int DMG_MAX = 440;

        public ShieldSlam(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("FR"), true, 0, SMI.None, 1, 1, BONUS_THREAT),
                  new EndDmg(DMG_MIN, DMG_MAX, 1, RatioType.Shield))
        {
        }
    }
}

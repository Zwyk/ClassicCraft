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
        public static int BONUS_DMG(int level)
        {
            if (level >= 70) return 208;
            else if (level >= 60) return 157;
            else if (level >= 56) return 138;
            else if (level >= 48) return 111;
            else if (level >= 40) return 80;
            else if (level >= 32) return 58;
            else if (level >= 24) return 44;
            else if (level >= 16) return 32;
            else if (level >= 8) return 21;
            else return 11;
        }
        public static int BONUS_THREAT(int level)
        {
            if (level >= 70) return 196;
            else if (level >= 60) return 175;
            else return (int)(BONUS_DMG(level) * 1.1);   // ?
        }

        public HeroicStrike(Player p)
            : base(p, CD, School.Physical, 
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("IHS") - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), false, 0, SMI.UseOnNextMHSwing, 1, 1, BONUS_THREAT(p.Level)),
                  new EndDmg(p.MH.DamageMin + BONUS_DMG(p.Level), p.MH.DamageMax + BONUS_DMG(p.Level), 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

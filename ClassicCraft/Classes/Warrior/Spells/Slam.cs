using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Slam : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Slam";

        public static int BASE_COST = 15;
        public static int CD = 0;
        public static double CAST_TIME = 1.5;

        public static int BASE_DMG = Program.version == Version.TBC ? 140 : 87;

        public Slam(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), true, CAST_TIME - (Program.version == Version.TBC ? 0.5 : 0.1) * p.GetTalentPoints("IS"), Program.version == Version.TBC ? SMI.None: SMI.Reset),
                  new EndDmg(p.MH.DamageMin + BASE_DMG, p.MH.DamageMax + BASE_DMG, 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

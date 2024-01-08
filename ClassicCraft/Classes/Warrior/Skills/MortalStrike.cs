using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MortalStrike : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Mortal Strike";

        public static int BASE_COST = 30;
        public static int CD = 6;

        public static int BASE_DMG = Program.version == Version.TBC ? 210 : 160;

        public MortalStrike(Player p)
            : base(p, CD - (Program.version == Version.TBC ? 0.2 * p.GetTalentPoints("IMS") - p.GetTalentPoints("FR") : 0), School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  new EndDmg(BASE_DMG + p.MH.DamageMin, BASE_DMG + p.MH.DamageMax, 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

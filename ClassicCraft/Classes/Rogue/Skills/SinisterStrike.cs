using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SinisterStrike : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Sinister Strike";

        public static int BASE_COST = 45;
        public static int CD = 0;

        public static int BASE_DMG = Program.version == Version.Vanilla ? 68 : 98;

        public SinisterStrike(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (p.GetTalentPoints("ISS") > 0 ? (p.GetTalentPoints("ISS") > 1 ? 5 : 3) : 0), true, 0, SMI.None, 1, 1, 0, EnergyType.ComboAward),
                  new EndDmg(p.MH.DamageMin + BASE_DMG, p.MH.DamageMax + BASE_DMG, 1/14.0, RatioType.WeaponMH)) { }
    }
}

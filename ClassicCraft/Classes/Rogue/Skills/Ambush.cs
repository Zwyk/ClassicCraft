using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Ambush : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Ambush";

        public static int BASE_COST = 60;
        public static int CD = 0;

        public static int BASE_DMG = Program.version == Version.Vanilla ? 290 : 335;
        public static double WEAP_RATIO = Program.version == Version.Vanilla ? 2.5 : 2.75;

        public Ambush(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, true, 0, SMI.None, 1, 1, 0, EnergyType.ComboSpend),
                  new EndDmg(BASE_DMG + p.MH.DamageMin * WEAP_RATIO, BASE_DMG + p.MH.DamageMin * WEAP_RATIO, 1 / 14.0, RatioType.WeaponMH))
        {
        }
    }
}

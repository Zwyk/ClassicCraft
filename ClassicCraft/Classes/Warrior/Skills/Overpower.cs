using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Overpower : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Overpower";

        public static int BASE_COST = 5;
        public static int CD = 6;

        public static int DMG = 5;

        public Overpower(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  new EndDmg(p.MH.DamageMin + DMG, p.MH.DamageMax + DMG, 1 / 14.0, RatioType.WeaponMH))
        {
        }
    }
}

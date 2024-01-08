using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shiv : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shiv";

        public static int BASE_COST = 20;
        public static int CD = 0;

        public Shiv(Player p)
            : base(p, CD, School.Physical, 
                  new SpellData(SpellType.Melee, (int)(BASE_COST + Math.Round(p.OH.Speed / 10)), true, 0, SMI.None, 1, 1, 0, EnergyType.ComboAward),
                  new EndDmg(p.MH.DamageMin, p.MH.DamageMax, 1/14.0, RatioType.WeaponMH))
        {
        }
    }
}

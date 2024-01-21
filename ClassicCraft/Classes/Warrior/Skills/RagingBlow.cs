using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class RagingBlow : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Raging Blow";

        public static int BASE_COST = 0;
        public static int CD = 8;

        public RagingBlow(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  new EndDmg(p.MH.DamageMin, p.MH.DamageMax, 1 / 14.0, RatioType.WeaponMH))
        {
        }

        public override bool CanUse()
        {
            return base.CanUse() && new List<string>() { BloodrageBuff.NAME, ConsumedByRage.NAME, "Enrage" }.Any(b => Player.Effects.Keys.Any(e => e == b));
        }
    }
}

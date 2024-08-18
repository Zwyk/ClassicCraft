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

        public static int EFFECT_DURATION = 5;
        public static int DMG(int level)
        {
            if (level >= 60) return 35;
            else if (level >= 44) return 25;
            else if (level >= 28) return 15;
            else return 5;
        }

        public Overpower(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST),
                  new EndDmg(p.MH.DamageMin + DMG(p.Level), p.MH.DamageMax + DMG(p.Level), 1 / 14.0, RatioType.WeaponMH))
        {
        }

        public override bool CanUse()
        {
            return base.CanUse() && Player.Target.Effects.ContainsKey(NAME);
        }
    }
}

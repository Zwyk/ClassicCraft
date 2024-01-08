using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BladeFlurry : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Blade Flurry";

        public static int BASE_COST = 25;
        public static int CD = 120;

        public BladeFlurry(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, true, 0, SMI.None, 1, 1, 0, EnergyType.ComboSpend),
                  null,
                  new EndEffect(BladeFlurryBuff.NAME))
        {
        }
    }
}

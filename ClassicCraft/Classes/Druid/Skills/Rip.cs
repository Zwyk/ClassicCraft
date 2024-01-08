using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rip : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rip";

        public static int BASE_COST = 30;
        public static int CD = 0;

        public Rip(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, true, 0, SMI.None, 1, 1, 0, EnergyType.ComboSpend),
                  null,
                  new EndEffect(RipDoT.NAME))
        {
        }

        public override int CustomCost()
        {
            return Player.Effects.ContainsKey(ClearCasting.NAME) ? 0 : Cost;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class FerociousBite : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Ferocious Bite";

        public static int BASE_COST = 35;
        public static int CD = 0;

        // rank 4
        public static int[] min =
        {
            173,
            301,
            429,
            557,
            685,
        };
        // rank 4
        public static int[] max =
        {
            223,
            351,
            479,
            607,
            735,
        };

        public double ENERGY_BONUS_DMG = 2.7;

        public double AP_RATIO = 0.15;

        public FerociousBite(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, true, 0, SMI.None, 1, 1, 0, EnergyType.ComboSpend),
                  new EndDmg(1, 1, 0.15, RatioType.AP))
        {
        }

        public override double GetEndDmgBase(bool mh = true)
        {
            return Randomer.Next(min[Player.Combo - 1], max[Player.Combo - 1] + 1) + ENERGY_BONUS_DMG * Player.Resource;
        }

        public override void CustomActionAfter()
        {
            Player.Resource = 0;
        }

        public override int CustomCost()
        {
            return Player.Effects.ContainsKey(ClearCasting.NAME) ? 0 : Cost;
        }
    }
}

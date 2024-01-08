using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MangleCat : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Mangle";

        public static int BASE_COST = 40;
        public static int CD = 0;

        public static double RATIO = 3.0;

        public MangleCat(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - 5 * p.GetTalentPoints("Fero"), true, 0, SMI.None, 1, 1, 0, EnergyType.ComboAward),
                  new EndDmg(p.Level * 0.85 * RATIO, p.Level * 1.25 * RATIO, 1/14.0, RatioType.AP),
                  new EndEffect(Mangle.NAME))
        {
        }

        public override int CustomCost()
        {
            return Player.Effects.ContainsKey(ClearCasting.NAME) ? 0 : Cost;
        }
    }
}

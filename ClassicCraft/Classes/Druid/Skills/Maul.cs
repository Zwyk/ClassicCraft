using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Maul : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Maul";

        public static int BASE_COST = 15;
        public static int CD = 0;

        public static double THREAT_MOD = 1.75;

        public static double RATIO = 2.5;

        public static int DMG = 128;

        public Maul(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("Fero"), false, 0, SMI.UseOnNextMHSwing, 1, THREAT_MOD),
                  new EndDmg(p.Level * 0.85 + DMG, p.Level * 1.25 + DMG, RATIO/14, RatioType.AP))
        {
        }

        public override int CustomCost()
        {
            return Player.Effects.ContainsKey(ClearCasting.NAME) ? 0 : Cost;
        }
    }
}

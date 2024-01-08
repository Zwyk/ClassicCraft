using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shred : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shred";

        public static int BASE_COST = 60;
        public static int CD = 0;

        public static int DMG(int level)
        {
            if (level >= 54) return 180;
            else return 54;
        }

        public static double DMG_RATIO = 2.25;

        public Shred(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("IS") * 6, true, 0, SMI.None, 1, 1, 0, EnergyType.ComboAward),
                  new EndDmg(p.Level * 0.85 * DMG_RATIO + DMG(p.Level), p.Level * 1.25 * DMG_RATIO + DMG(p.Level), 1/14.0, RatioType.AP))
        {
        }

        public override int CustomCost()
        {
            return Player.Effects.ContainsKey(ClearCasting.NAME) ? 0 : Cost;
        }
    }
}

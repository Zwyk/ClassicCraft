using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Swipe : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Swipe";

        public static int BASE_COST = 20;
        public static int CD = 0;

        public static int DAMAGE = 83;

        public static double THREAT_MOD = 1.75;

        public static int MAX_TARGETS = 3;

        public Swipe(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("Fero"), true, 0, SMI.None, MAX_TARGETS, THREAT_MOD),
                  new EndDmg(DAMAGE, DAMAGE, 0, RatioType.None))
        {
        }
    }
}

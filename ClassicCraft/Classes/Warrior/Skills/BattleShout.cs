using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BattleShout : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Battle Shout";

        public static int BASE_COST = 10;
        public static int CD = 0;

        public static int THREAT_PER_BUFFED = 70;
        public static int NB_BUFFED = 5;

        public BattleShout(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, true, 0, SMI.None, 1, 1, THREAT_PER_BUFFED * NB_BUFFED))
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BerserkerRageBuff : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Berserker Rage";

        public static int LENGTH = 10;

        public BerserkerRageBuff(Player p)
            : base(p, p, true, LENGTH)
        {
        }
    }
}

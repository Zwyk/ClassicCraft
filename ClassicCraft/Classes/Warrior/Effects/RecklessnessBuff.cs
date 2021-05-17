using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class RecklessnessBuff : Effect
    {
        public static int LENGTH = 15;

        public RecklessnessBuff(Player p)
            : base(p, p, true, LENGTH + 2 * p.GetTalentPoints("ID"), 1)
        {
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Recklessness' Buff";
    }
}

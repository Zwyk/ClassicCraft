using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class RecklessnessBuff : Effect
    {
        public RecklessnessBuff(Entity target, bool friendly = true, double baseLength = 12, int baseStacks = 1)
            : base(target, friendly, baseLength, baseStacks)
        {
        }

        public override string ToString()
        {
            return "RecklessnessBuff";
        }
    }
}

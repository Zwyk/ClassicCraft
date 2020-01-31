using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowTrance : Effect
    {
        public ShadowTrance(Player p, Entity target, bool friendly, double baseLength, int baseStacks = 1)
            : base(p, target, friendly, baseLength, baseStacks)
        {
        }
    }
}

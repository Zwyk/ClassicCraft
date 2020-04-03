using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class CustomEffect : Effect
    {
        public string Name { get; set; }

        public CustomEffect(Player p, Entity target, string name, bool friendly, double baseLength, int baseStacks = 1)
            : base(p, target, friendly, baseLength, baseStacks)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

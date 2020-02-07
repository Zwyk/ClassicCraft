using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class PresenceOfMindEffect : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Presence of Mind";
        
        public static int LENGTH = 10;

        public PresenceOfMindEffect(Player p)
            : base(p, p, true, LENGTH)
        {
        }
    }
}

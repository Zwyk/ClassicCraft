using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Mangle : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Mangle";

        public static int LENGTH = 60;

        public Mangle(Player p, Entity t)
            : base(p, t, false, LENGTH)
        {
        }
    }
}

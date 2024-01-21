using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Enrage : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Enrage";

        public static int LENGTH = 12;

        public static double BONUS_DMG = 1.25;

        public Enrage(Player p)
            : base(p, p, true, LENGTH)
        {
        }
    }
}

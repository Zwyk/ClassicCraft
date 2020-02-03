using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Aura : PlayerObject
    {
        public static string NAME = "Undefined Aura";

        //public Entity Source { get; set; }

        public Aura(Player p)
            : base(p)
        {
        }

        public override string ToString()
        {
            return NAME;
        }
    }
}

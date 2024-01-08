using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MCP : ActiveItemBuff
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "MCP";

        public static int CD = 30;

        public static int LENGTH = 300000000; // 30;

        public MCP(Player p)
            : base(p, CD, LENGTH, NAME, new Dictionary<Attribute, double>() { { Attribute.Haste, 0.5 } })
        {
        }
    }
}

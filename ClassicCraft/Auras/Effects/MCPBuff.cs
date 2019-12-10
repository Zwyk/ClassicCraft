using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MCPBuff : Effect
    {
        //public static int LENGTH = 30;
        public static int LENGTH = 300000000;

        public MCPBuff(Player p, double baseLength = 300000000)
            : base(p, p, true, baseLength, 1)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.HasteMod *= 1.5;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.HasteMod /= 1.5;
        }

        public override string ToString()
        {
            return "MCP Buff";
        }
    }
}

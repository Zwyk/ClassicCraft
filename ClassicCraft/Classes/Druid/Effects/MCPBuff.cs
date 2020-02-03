using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MCPBuff : Effect
    {
        public override string ToString() { return NAME; } public static new string NAME = "MCP Buff";

        //public static int LENGTH = 30;
        public static int LENGTH = 300000000;

        public MCPBuff(Player p, double baseLength = 300000000)
            : base(p, p, true, baseLength, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.HasteMod *= 1.5;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.HasteMod /= 1.5;
        }
    }
}

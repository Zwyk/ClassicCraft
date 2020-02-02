using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MCP : Skill
    {
        public static int CD = 30;

        public MCP(Player p, double baseCD = 30)
            : base(p, baseCD, 0, false)
        {
        }

        public override void Cast()
        {
            DoAction();
            CDAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.Any(e => e is MCPBuff))
            {
                Effect current = Player.Effects.Where(e => e is MCPBuff).First();
                current.Refresh();
            }
            else
            {
                MCPBuff r = new MCPBuff(Player);
                r.StartEffect();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "MCP";
        }
    }
}

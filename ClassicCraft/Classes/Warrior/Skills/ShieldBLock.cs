using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShieldBlock : Skill
    {
        public static int CD = 5;
        public static int BASE_COST = 10;

        public ShieldBlock(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            Player.Resource -= Cost;
            
            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : [{1}] cast ({4} {2}/{3})", Player.Sim.CurrentTime, ToString(), Player.Resource, Player.MaxResource, "rage"));
            }
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Shield Block";
    }
}

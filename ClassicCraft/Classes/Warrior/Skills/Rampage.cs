using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rampage : Skill
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rampage";

        public static int BASE_COST = 20;
        public static int CD = 0;

        public Rampage(Player p)
            : base(p, CD, BASE_COST, true)
        {
        }

        public override void Cast(Entity t)
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            LogAction();

            if (Player.Effects.ContainsKey(RampageBuff.NAME))
            {
                Player.Effects[RampageBuff.NAME].Refresh();
            }
            else
            {
                new RampageBuff(Player).StartEffect();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Recklessness : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Recklessness";

        public static int BASE_COST = 0;
        public static int CD = 300;

        public Recklessness(Player p)
            : base(p, CD, BASE_COST, true)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(RecklessnessBuff.NAME))
            {
                Player.Effects[RecklessnessBuff.NAME].Refresh();
            }
            else
            {
                new RecklessnessBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

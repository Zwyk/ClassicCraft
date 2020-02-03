using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class AdrenalineRush : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Adrenaline Rush";

        public static int BASE_COST = 0;
        public static int CD = 300;

        public AdrenalineRush(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(AdrenalineRushBuff.NAME))
            {
                Player.Effects[AdrenalineRushBuff.NAME].Refresh();
            }
            else
            {
                new AdrenalineRushBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

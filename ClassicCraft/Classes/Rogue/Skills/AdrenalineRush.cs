using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class AdrenalineRush : Skill
    {
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
            if (Player.Effects.Any(e => e is AdrenalineRushBuff))
            {
                Effect current = Player.Effects.Where(e => e is AdrenalineRushBuff).First();
                current.Refresh();
            }
            else
            {
                AdrenalineRushBuff r = new AdrenalineRushBuff(Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Adrenaline Rush";
        }
    }
}

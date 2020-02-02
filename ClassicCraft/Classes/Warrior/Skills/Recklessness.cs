using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Recklessness : Skill
    {
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
            if (Player.Effects.Any(e => e is RecklessnessBuff))
            {
                Effect current = Player.Effects.Where(e => e is RecklessnessBuff).First();
                current.Refresh();
            }
            else
            {
                RecklessnessBuff r = new RecklessnessBuff(Player);
                r.StartEffect();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Recklesness";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SliceAndDice : Skill
    {
        public static int BASE_COST = 25;
        public static int CD = 0;

        public SliceAndDice(Player p)
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
            if (Player.Effects.Any(e => e is SliceAndDiceBuff))
            {
                Effect current = Player.Effects.Where(e => e is SliceAndDiceBuff).First();
                current.Refresh();
            }
            else
            {
                SliceAndDiceBuff r = new SliceAndDiceBuff(Player);
                r.StartEffect();
            }

            Player.Combo = 0;

            LogAction();
        }

        public override string ToString()
        {
            return "Slice And Dice";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SliceAndDice : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Slice And Dice";

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
            if (Player.Effects.ContainsKey(SliceAndDiceBuff.NAME))
            {
                Player.Effects[SliceAndDiceBuff.NAME].Refresh();
            }
            else
            {
                new SliceAndDiceBuff(Player).StartEffect();
            }

            Player.Combo = 0;

            LogAction();
        }
    }
}

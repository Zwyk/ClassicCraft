using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BattleShout : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Battle Shout";

        public static int BASE_COST = 10;
        public static int CD = 0;

        public BattleShout(Player p)
            : base(p, CD, BASE_COST, true)
        {
        }

        public override void Cast()
        {
            DoAction();
            CommonRessourceSkill();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(BattleShoutBuff.NAME))
            {
                Player.Effects[BattleShoutBuff.NAME].Refresh();
            }
            else
            {
                new BattleShoutBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

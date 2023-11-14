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

        public static int THREAT_PER_BUFFED = 70;

        public int NbBuffed { get; set; }

        public BattleShout(Player p, int nbBuffed = 5)
            : base(p, CD, BASE_COST, true)
        {
            NbBuffed = nbBuffed;
        }

        public override void Cast(Entity t)
        {
            DoAction();
            CommonRessourceSkill();
        }

        public override void DoAction()
        {
            /*
            if (Player.Effects.ContainsKey(BattleShoutBuff.NAME))
            {
                Player.Effects[BattleShoutBuff.NAME].Refresh();
            }
            else
            {
                new BattleShoutBuff(Player).StartEffect();
            }
            */
            LogAction(THREAT_PER_BUFFED * NbBuffed);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SweepingStrikes : Skill
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Sweeping Strikes";

        public static int BASE_COST = 30;
        public static int CD = 30;

        public SweepingStrikes(Player p)
            : base(p, CD, BASE_COST, false)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(SweepingStrikesBuff.NAME))
            {
                Player.Effects[SweepingStrikesBuff.NAME].Refresh();
            }
            else
            {
                new SweepingStrikesBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

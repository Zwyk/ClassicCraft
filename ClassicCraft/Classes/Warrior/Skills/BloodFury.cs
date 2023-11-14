using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BloodFury : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Blood Fury";

        public static int BASE_COST = 0;
        public static int CD = 120;

        public BloodFury(Player p)
            : base(p, CD, BASE_COST, false)
        {
        }

        public override void Cast(Entity t)
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(BloodFuryBuff.NAME))
            {
                Player.Effects[BloodFuryBuff.NAME].Refresh();
            }
            else
            {
                new BloodFuryBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

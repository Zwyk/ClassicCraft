using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BladeFlurry : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Blade Flurry";

        public static int BASE_COST = 25;
        public static int CD = 120;

        public BladeFlurry(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void Cast(Entity t)
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(BladeFlurryBuff.NAME))
            {
                Player.Effects[BladeFlurryBuff.NAME].Refresh();
            }
            else
            {
                new BladeFlurryBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

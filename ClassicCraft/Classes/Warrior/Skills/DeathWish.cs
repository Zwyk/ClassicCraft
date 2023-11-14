using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class DeathWish : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Death Wish";

        public static int BASE_COST = 10;
        public static int CD = 180;

        public DeathWish(Player p)
            : base(p, CD, BASE_COST, true)
        {
        }

        public override void Cast(Entity t)
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(DeathWishBuff.NAME))
            {
                Player.Effects[DeathWishBuff.NAME].Refresh();
            }
            else
            {
                new DeathWishBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

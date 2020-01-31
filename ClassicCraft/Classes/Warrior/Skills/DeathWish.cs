using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class DeathWish : Skill
    {
        public static int BASE_COST = 10;
        public static int CD = 180;

        public DeathWish(Player p)
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
            if (Player.Effects.Any(e => e is DeathWishBuff))
            {
                Effect current = Player.Effects.Where(e => e is DeathWishBuff).First();
                current.Refresh();
            }
            else
            {
                DeathWishBuff dw = new DeathWishBuff(Player);
                dw.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Death Wish";
        }
    }
}

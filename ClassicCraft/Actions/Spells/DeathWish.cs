using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class DeathWish : Spell
    {
        public DeathWish(Player p, double baseCD = 180, int ressourceCost = 10, bool gcd = true)
            : base(p, baseCD, ressourceCost, gcd)
        {
        }

        public override void Cast()
        {
            CommonSpell();
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
                DeathWishBuff dw = new DeathWishBuff(Player, Player);
                dw.StartBuff();
            }
        }
    }
}

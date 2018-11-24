using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class DeathWish : Spell
    {
        public DeathWish(double baseCD = 180, int ressourceCost = 10, bool gcd = true)
            : base(baseCD, ressourceCost, gcd)
        {
        }

        public override void Cast()
        {
            CommonSpell();
            DoAction();
        }

        public override void DoAction()
        {
            if (Program.Player.Effects.Any(e => e is DeathWishBuff))
            {
                Effect current = Program.Player.Effects.Where(e => e is DeathWishBuff).First();
                current.Refresh();
            }
            else
            {
                DeathWishBuff dw = new DeathWishBuff(Program.Player);
                dw.StartBuff();
            }
        }
    }
}

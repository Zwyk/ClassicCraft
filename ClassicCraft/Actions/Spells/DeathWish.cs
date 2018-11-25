using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class DeathWish : Spell
    {
        public DeathWish(Simulation s, double baseCD = 180, int ressourceCost = 10, bool gcd = true)
            : base(s, baseCD, ressourceCost, gcd)
        {
        }

        public override void Cast()
        {
            CommonSpell();
            DoAction();
        }

        public override void DoAction()
        {
            if (Sim.Player.Effects.Any(e => e is DeathWishBuff))
            {
                Effect current = Sim.Player.Effects.Where(e => e is DeathWishBuff).First();
                current.Refresh();
            }
            else
            {
                DeathWishBuff dw = new DeathWishBuff(Sim, Sim.Player);
                dw.StartBuff();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Recklessness : Spell
    {
        public Recklessness(Simulation s, double baseCD = 300, int ressourceCost = 0, bool gcd = true)
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
            if (Sim.Player.Effects.Any(e => e is RecklessnessBuff))
            {
                Effect current = Sim.Player.Effects.Where(e => e is RecklessnessBuff).First();
                current.Refresh();
            }
            else
            {
                RecklessnessBuff r = new RecklessnessBuff(Sim, Sim.Player);
                r.StartBuff();
            }
        }
    }
}

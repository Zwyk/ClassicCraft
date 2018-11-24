using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Recklessness : Spell
    {
        public Recklessness(double baseCD = 300, int ressourceCost = 0, bool gcd = true)
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
            if (Program.Player.Effects.Any(e => e is RecklessnessBuff))
            {
                Effect current = Program.Player.Effects.Where(e => e is RecklessnessBuff).First();
                current.Refresh();
            }
            else
            {
                RecklessnessBuff r = new RecklessnessBuff(Program.Player);
                r.StartBuff();
            }
        }
    }
}

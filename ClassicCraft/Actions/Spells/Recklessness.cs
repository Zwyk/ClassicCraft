using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Recklessness : Spell
    {
        public Recklessness(Player p, double baseCD = 300, int ressourceCost = 0, bool gcd = true)
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
            if (Player.Effects.Any(e => e is RecklessnessBuff))
            {
                Effect current = Player.Effects.Where(e => e is RecklessnessBuff).First();
                current.Refresh();
            }
            else
            {
                RecklessnessBuff r = new RecklessnessBuff(Player, Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Reck";
        }
    }
}

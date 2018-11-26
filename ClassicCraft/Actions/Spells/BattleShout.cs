using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BattleShout : Spell
    {
        public BattleShout(Player p, double baseCD = 0, int ressourceCost = 10, bool gcd = true)
            : base(p, baseCD, ressourceCost, gcd)
        {
        }

        public override void Cast()
        {
            DoAction();
            CommonSpell();
        }

        public override void DoAction()
        {
            if (Player.Effects.Any(e => e is BattleShoutBuff))
            {
                Effect current = Player.Effects.Where(e => e is BattleShoutBuff).First();
                current.Refresh();
            }
            else
            {
                BattleShoutBuff r = new BattleShoutBuff(Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "BattleShout";
        }
    }
}

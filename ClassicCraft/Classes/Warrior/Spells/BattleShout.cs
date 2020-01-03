using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BattleShout : Spell
    {
        public static int BASE_COST = 10;
        public static int CD = 0;

        public BattleShout(Player p)
            : base(p, CD, BASE_COST, true)
        {
        }

        public override void Cast()
        {
            DoAction();
            CommonRessourceSpell();
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
            return "Battle Shout";
        }
    }
}

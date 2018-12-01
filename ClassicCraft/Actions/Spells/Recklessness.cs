using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Recklessness : Spell
    {
        public static int COST = 0;
        public static int CD = 300;

        public Recklessness(Player p)
            : base(p, CD, COST, true)
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
                RecklessnessBuff r = new RecklessnessBuff(Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Recklesness";
        }
    }
}

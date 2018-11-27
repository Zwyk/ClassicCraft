using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BloodFury : Spell
    {
        public BloodFury(Player p, double baseCD = 120, int ressourceCost = 0, bool gcd = false)
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
            if (Player.Effects.Any(e => e is BloodFuryBuff))
            {
                Effect current = Player.Effects.Where(e => e is BloodFuryBuff).First();
                current.Refresh();
            }
            else
            {
                BloodFuryBuff r = new BloodFuryBuff(Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Blood Fury";
        }
    }
}

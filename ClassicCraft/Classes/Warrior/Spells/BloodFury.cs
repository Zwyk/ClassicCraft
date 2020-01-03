using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BloodFury : Spell
    {
        public static int BASE_COST = 0;
        public static int CD = 120;

        public BloodFury(Player p)
            : base(p, CD, BASE_COST, false)
        {
        }

        public override void Cast()
        {
            CommonRessourceSpell();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BladeFlurry : Spell
    {
        public static int BASE_COST = 25;
        public static int CD = 120;

        public BladeFlurry(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void Cast()
        {
            CommonRessourceSpell();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.Any(e => e is BladeFlurryBuff))
            {
                Effect current = Player.Effects.Where(e => e is BladeFlurryBuff).First();
                current.Refresh();
            }
            else
            {
                BladeFlurryBuff r = new BladeFlurryBuff(Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Blade Flurry";
        }
    }
}

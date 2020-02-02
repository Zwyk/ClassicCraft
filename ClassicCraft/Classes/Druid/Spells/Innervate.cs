using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Innervate : Spell
    {
        public static int CD = 360;

        public Innervate(Player p)
               : base(p, CD, (int)(p.BaseMana * 0.05), true, true)
        {
        }

        public override void Cast()
        {
            CommonManaSpell();
            DoAction();
        }

        public override void DoAction()
        {
            base.DoAction();

            if (Player.Effects.Any(e => e is InnervateBuff))
            {
                Effect current = Player.Effects.Where(e => e is InnervateBuff).First();
                current.Refresh();
            }
            else
            {
                InnervateBuff r = new InnervateBuff(Player);
                r.StartEffect();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Innervate";
        }
    }
}

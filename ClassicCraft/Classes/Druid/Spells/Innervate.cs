using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Innervate : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Innervate";

        public static int CD = 360;

        public Innervate(Player p)
               : base(p, CD, (int)(p.BaseMana * 0.05), true, true)
        {
        }

        public override void Cast(Entity t)
        {
            Target = t;
            CommonManaSpell();
            DoAction();
        }

        public override void DoAction()
        {
            base.DoAction();

            if (Player.Effects.ContainsKey(InnervateBuff.NAME))
            {
                Player.Effects[InnervateBuff.NAME].Refresh();
            }
            else
            {
                new InnervateBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

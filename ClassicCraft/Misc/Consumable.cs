using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Consumable : Skill
    {
        public Consumable(Player p, double baseCD)
            : base(p, baseCD, 0, false)
        {
        }

        public override void Cast()
        {
            DoAction();
            CDAction();
        }
    }
}

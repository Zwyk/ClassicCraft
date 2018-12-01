using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Potion : Spell
    {
        public Potion(Player p, double baseCD)
            : base(p, baseCD, 0, false)
        {
        }

        public override void Cast()
        {
            DoAction();
            CDAction();
        }

        public override void DoAction()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "Undefined Potion";
        }
    }
}

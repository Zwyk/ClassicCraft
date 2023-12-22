using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class InnerFocusBuff : Effect
    {
        public static int LENGTH = 10000;

        public double Bonus { get; set; }

        public InnerFocusBuff(Player p)
            : base(p, p, true, LENGTH)
        {
        }

        public override void EndEffect()
        {
            base.EndEffect();
            ((Priest)Player).inner.CDAction();
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Inner Focus";
    }
}

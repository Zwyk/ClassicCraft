using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class AdrenalineRushBuff : Effect
    {
        public static int LENGTH = 15;

        public AdrenalineRushBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();
        }

        public override void EndEffect()
        {
            base.EndEffect();
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Adrenaline Rush's Buff";
    }
}

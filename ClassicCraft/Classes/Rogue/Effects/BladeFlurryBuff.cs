using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BladeFlurryBuff : Effect
    {
        public static int LENGTH = 15;

        public BladeFlurryBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.HasteMod *= 1.2;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.HasteMod /= 1.2;
        }

        public override string ToString()
        {
            return "Blade Flurry's Buff";
        }
    }
}

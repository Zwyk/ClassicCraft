using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class JujuFlurryBuff : Effect
    {
        public override string ToString() { return NAME; } public static new string NAME = "Juju Flurry Buff";

        public static int LENGTH = 20;

        public JujuFlurryBuff(Player p, double baseLength = 20)
            : base(p, p, true, baseLength, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.HasteMod *= 1.03;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.HasteMod /= 1.03;
        }
    }
}

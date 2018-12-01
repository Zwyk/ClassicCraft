using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class JujuFlurryBuff : Effect
    {
        public static int LENGTH = 20;

        public JujuFlurryBuff(Player p, double baseLength = 20)
            : base(p, p, true, baseLength, 1)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.HasteMod *= 1.03;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.HasteMod /= 1.03;
        }

        public override string ToString()
        {
            return "Juju Flurry Buff";
        }
    }
}

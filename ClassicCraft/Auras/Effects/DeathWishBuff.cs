using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeathWishBuff : Effect
    {
        public DeathWishBuff(Simulation s, Entity target, bool friendly = true, double baseLength = 30, int baseStacks = 1)
            : base(s, target, friendly, baseLength, baseStacks)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Sim.Player.DamageMod *= 1.2;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Sim.Player.DamageMod /= 1.2;
        }

        public override string ToString()
        {
            return "DWBuff";
        }
    }
}

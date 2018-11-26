using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BloodFuryBuff : Effect
    {
        public BloodFuryBuff(Player p, bool friendly = true, double baseLength = 15, int baseStacks = 1)
            : base(p, p, friendly, baseLength, baseStacks)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.DamageMod *= 1.25;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.DamageMod /= 1.25;
        }

        public override string ToString()
        {
            return "BFBuff";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeathWishBuff : Effect
    {
        public DeathWishBuff(Entity target, bool friendly = true, double baseLength = 30, int baseStacks = 1)
            : base(target, friendly, baseLength, baseStacks)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Program.Player.DamageMod *= 1.2;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Program.Player.DamageMod /= 1.2;
        }

        public override string ToString()
        {
            return "DWBuff";
        }
    }
}

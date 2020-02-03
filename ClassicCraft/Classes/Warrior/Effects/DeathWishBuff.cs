using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeathWishBuff : Effect
    {
        public static int LENGTH = 30;

        public DeathWishBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.DamageMod *= 1.2;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.DamageMod /= 1.2;
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Death Wish's Buff";
    }
}

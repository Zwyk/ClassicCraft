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

            Player.DamageMod *= 1.2;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.DamageMod /= 1.2;
        }

        public override string ToString()
        {
            return "Adrenaline Rush's Buff";
        }
    }
}

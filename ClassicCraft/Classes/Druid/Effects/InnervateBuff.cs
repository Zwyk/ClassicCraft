using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class InnervateBuff : Effect
    {
        public static int LENGTH = 20;

        public InnervateBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.CastingRegenPct += 1;
            Player.MPTRatio += 4;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.CastingRegenPct -= 1;
            Player.MPTRatio -= 4;
        }

        public override string ToString()
        {
            return "Innervate's Buff";
        }
    }
}

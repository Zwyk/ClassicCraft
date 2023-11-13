using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DemonicGraceBuff : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Demonic Grace's Buff";

        public static int LENGTH = 6;

        public DemonicGraceBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.SpellCritChance += 0.3;
            //Player.DodgeChance += 0.3;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.SpellCritChance -= 0.3;
            //Player.DodgeChance -= 0.3;
        }
    }
}

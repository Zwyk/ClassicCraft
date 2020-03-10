using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HammerOfJusticeDebuff : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Hammer of Justice";

        public static int LENGTH = 6;

        public HammerOfJusticeDebuff(Player p)
            : base(p, p.Sim.Boss, false, LENGTH, 1)
        {
        }

        public static void CheckProc(Player p, Spell s, ResultType res)
        {
            /*  Need to implement Nightfall 2H axe proc
            if (res == ResultType.Crit && s is ShadowBolt)
            {
                if (p.Sim.Boss.Effects.ContainsKey(NAME))
                {
                    p.Sim.Boss.Effects[NAME].Refresh();
                }
                else
                {
                    new SpellVulnerability(p).StartEffect();
                }
            }
            else if(res == ResultType.Hit && p.Sim.Boss.Effects.ContainsKey(NAME))
            {
                p.Sim.Boss.Effects[NAME].StackRemove();
            }
            */
        }
    }
}

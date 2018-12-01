using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MightyRage : Potion
    {
        public MightyRage(Player p, double baseCD = 120)
            : base(p, baseCD)
        {
        }

        public override void DoAction()
        {
            if (Player.Effects.Any(e => e is MightyRageBuff))
            {
                Effect current = Player.Effects.Where(e => e is MightyRageBuff).First();
                current.Refresh();
            }
            else
            {
                MightyRageBuff r = new MightyRageBuff(Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Juju Flurry Potion";
        }
    }
}

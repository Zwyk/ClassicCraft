using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MightyRage : Potion
    {
        public static int CD = 120;

        public MightyRage(Player p)
            : base(p, CD)
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

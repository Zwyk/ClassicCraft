using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JujuFlurry : Potion
    {
        public static int CD = 60;

        public JujuFlurry(Player p)
            : base(p, CD)
        {
        }

        public override void DoAction()
        {
            if (Player.Effects.Any(e => e is JujuFlurryBuff))
            {
                Effect current = Player.Effects.Where(e => e is JujuFlurryBuff).First();
                current.Refresh();
            }
            else
            {
                JujuFlurryBuff r = new JujuFlurryBuff(Player);
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

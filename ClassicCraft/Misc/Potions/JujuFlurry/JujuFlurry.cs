using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JujuFlurry : Potion
    {
        public override string ToString() { return NAME; } public static new string NAME = "Juju Flurry Potion";

        public static int CD = 60;

        public JujuFlurry(Player p)
            : base(p, CD)
        {
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(JujuFlurryBuff.NAME))
            {
                Player.Effects[JujuFlurryBuff.NAME].Refresh();
            }
            else
            {
                new JujuFlurryBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

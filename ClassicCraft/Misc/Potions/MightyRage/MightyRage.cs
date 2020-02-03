using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MightyRage : Potion
    {
        public override string ToString() { return NAME; } public static new string NAME = "Mighty Rage Potion";

        public static int CD = 120;

        public MightyRage(Player p)
            : base(p, CD)
        {
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(MightyRageBuff.NAME))
            {
                Player.Effects[MightyRageBuff.NAME].Refresh();
            }
            else
            {
                new MightyRageBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

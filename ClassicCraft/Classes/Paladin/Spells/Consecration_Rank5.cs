using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
	public class Consecration_Rank5 : Consecration_Base
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Consecration (Rank 5)";

        public Consecration_Rank5(Player p)
               : base(p, 565)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            if (Player.Sim.Boss.Effects.ContainsKey(ConsecrationDoT_Rank5.NAME))
            {
                Player.Sim.Boss.Effects[ConsecrationDoT_Rank5.NAME].Refresh();
            }
            else
            {
                new ConsecrationDoT_Rank5(Player, Player.Sim.Boss).StartEffect();
            }
        }
    }
}

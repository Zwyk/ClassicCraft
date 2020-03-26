using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
	public class Consecration_Rank1 : Consecration_Base
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Consecration (Rank 1)";

        public Consecration_Rank1(Player p)
               : base(p, 135)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            if (Player.Sim.Boss.Effects.ContainsKey(ConsecrationDoT_Rank1.NAME))
            {
                Player.Sim.Boss.Effects[ConsecrationDoT_Rank1.NAME].Refresh();
            }
            else
            {
                new ConsecrationDoT_Rank1(Player, Player.Sim.Boss).StartEffect();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClassicCraft
{
    public class BloodrageBuff : EffectOnTime
    {
        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Bloodrage Buff";

        public static int TICK_DELAY = 1;
        public static int DURATION = 10;

        public static int BASE_RAGE = 10;

        public BloodrageBuff(Player p)
            : base(p, p, true, DURATION, 1, 0, TICK_DELAY, 1, School.Physical)
        {
        }

        public override void ApplyTick(int damage)
        {
            Player.Resource += 1;

            if(Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} for {2} rage (rage {3}/{4})", Player.Sim.CurrentTime, ToString(), 1, Player.Resource, Player.MaxResource));
            }
        }

        public override void WhenApplied()
        {
            Player.Resource += 10 + (Player.GetTalentPoints("IBR") > 0 ? (Player.GetTalentPoints("IBR") == 1 ? 2 : 5) : 0);
        }
    }
}

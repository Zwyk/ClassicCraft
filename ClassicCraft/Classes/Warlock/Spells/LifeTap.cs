using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class LifeTap : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Life Tap";

        public static int BASE_COST = 424;
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public LifeTap(Player p)
            : base(p, CD, BASE_COST, false, true, School.Shadow, CAST_TIME)
        {
        }

        public override bool CanUse()
        {
            return //Player.Life >= Cost && 
                Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public int LifeCost()
        {
            return (int)(BASE_COST + 0.8 * Player.AP);
        }

        public int ManaGain()
        {
            return (int)(LifeCost() * 1 + (0.1 * Player.GetTalentPoints("ILT")));
        }

        public override void DoAction()
        {
            base.DoAction();
            CDAction();

            //Player.Life -= LifeCost();
            Player.Mana += ManaGain();

            LogAction();
        }
    }
}

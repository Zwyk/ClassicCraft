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

        public int BASE_COST(int level)
        {
            if (level >= 56) return 424;
            else if (level >= 46) return 310;
            else if (level >= 36) return 220;
            else if (level >= 26) return 140;
            else if (level >= 16) return 75;
            else if (level >= 6) return 30;
            else return 0;
        }
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public LifeTap(Player p)
            : base(p, CD, 0, false, true, School.Shadow, CAST_TIME)
        {
            Cost = BASE_COST(p.Level);
        }

        public override bool CanUse()
        {
            return /*Player.Life >= Cost && */Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public int LifeCost()
        {
            return (int)(BASE_COST(Player.Level) + 0.8 * Player.AP);
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

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

        public int? _BASE_COST = null;
        public int BASE_COST
        {
            get
            {
                if (!_BASE_COST.HasValue)
                {
                    if (Player.Level >= 56) _BASE_COST = 424;
                    else if (Player.Level >= 46) _BASE_COST = 310;
                    else if (Player.Level >= 36) _BASE_COST = 220;
                    else if (Player.Level >= 26) _BASE_COST = 140;
                    else if (Player.Level >= 16) _BASE_COST = 75;
                    else if (Player.Level >= 6) _BASE_COST = 30;
                    else _BASE_COST = 0;
                }
                return _BASE_COST.Value;
            }
        }
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public LifeTap(Player p)
            : base(p, CD, 0, false, true, School.Shadow, CAST_TIME)
        {
            Cost = BASE_COST;
        }

        public override bool CanUse()
        {
            return /*Player.Life >= Cost && */Available() && (AffectedByGCD ? Player.HasGCD() : true);
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

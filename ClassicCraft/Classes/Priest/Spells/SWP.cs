using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SWP : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "SW:P";

        public static int BASE_COST = 470;
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public SWP(Player p)
            : base(p, CD, BASE_COST, true, true, School.Shadow, CAST_TIME)
        {
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();
            
            ResultType res = Simulation.MagicMitigationBinary(Player.Sim.Boss.MagicResist[School]);

            if (res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Player.Sim.Boss, false, 0.02 * Player.GetTalentPoints("SF"));
            }

            if (res == ResultType.Hit)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0), Player.Sim.CurrentTime));
                if (Player.Sim.Boss.Effects.ContainsKey(SWPDoT.NAME))
                {
                    Player.Sim.Boss.Effects[SWPDoT.NAME].Refresh();
                }
                else
                {
                    new SWPDoT(Player, Player.Sim.Boss).StartEffect();
                }
            }
            else
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Resist, 0), Player.Sim.CurrentTime));
            }
        }
    }
}

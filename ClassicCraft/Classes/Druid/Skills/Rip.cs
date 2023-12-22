using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rip : Skill
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rip";

        public static int BASE_COST = 30;
        public static int CD = 0;

        public Rip(Player p)
            : base(p, CD, BASE_COST) { }

        public override bool CanUse()
        {
            return (Player.Effects.ContainsKey(ClearCasting.NAME) || Player.Resource >= Cost) && Available() && (!AffectedByGCD || Player.HasGCD()) && Player.Combo > 0;
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Target);

            int cost = Cost;
            if (Player.Effects.ContainsKey(ClearCasting.NAME))
            {
                cost = 0;
                Player.Effects[ClearCasting.NAME].StackRemove();
            }

            CommonAction();
            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= cost / 2;
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(res, 0, 0), Player.Sim.CurrentTime));
            }
            else
            {
                if (Target.Effects.ContainsKey(RipDoT.NAME))
                {
                    Target.Effects[RipDoT.NAME].Refresh();
                }
                else
                {
                    new RipDoT(Player, Target).StartEffect();
                }
                Player.Resource -= cost;
                Player.Combo = 0;
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
            }

            //Player.CheckOnHits(true, false, res);
        }
    }
}

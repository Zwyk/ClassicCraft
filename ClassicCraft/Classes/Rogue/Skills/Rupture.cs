using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rupture : Skill
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rupture";

        public static int BASE_COST = 25;
        public static int CD = 0;

        public Rupture(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void DoAction()
        {
            CommonAction();

            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Player.Effects.ContainsKey("CdG") ? 0 : Cost / 2;
                if (Player.Effects.ContainsKey("CdG")) Player.Effects["CdG"].EndEffect();

                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(res, 0, 0), Player.Sim.CurrentTime));
            }
            else
            {
                Player.Resource -= Player.Effects.ContainsKey("CdG") ? 0 : Cost;
                if (Player.Effects.ContainsKey("CdG")) Player.Effects["CdG"].EndEffect();

                if (Player.GetTalentPoints("RS") > 0 && Randomer.NextDouble() < 0.2 * Player.Combo)
                {
                    Player.Resource += 25;
                }

                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                if (Player.Sim.Boss.Effects.ContainsKey(RuptureDoT.NAME))
                {
                    Player.Sim.Boss.Effects[RuptureDoT.NAME].EndEffect();
                }

                new RuptureDoT(Player, Player.Sim.Boss).StartEffect();

                Player.Combo = 0;

                if (Randomer.NextDouble() < 0.2 * Player.GetTalentPoints("Ruth"))
                {
                    Player.Combo++;
                }
                if (Player.NbSet("Netherblade") >= 4 && Randomer.NextDouble() < 0.15)
                {
                    Player.Combo++;
                }
            }

            Player.CheckOnHits(true, false, res);
        }
    }
}

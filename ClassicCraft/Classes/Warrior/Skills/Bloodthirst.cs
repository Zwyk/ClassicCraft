using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodthirst : Skill
    {
        public static int BASE_COST = 30;
        public static int CD = 6;

        public Bloodthirst(Player p)
            : base(p, CD, BASE_COST) {}

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);
            
            int damage = (int)Math.Round(0.45 * Player.AP
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0 + (0.1 * Player.GetTalentPoints("Impale")) : 0))
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                );

            CommonAction();
            if(res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Cost / 2;
            }
            else
            {
                Player.Resource -= Cost;
            }

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Bloodthirst";
    }
}

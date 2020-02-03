using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Hamstring : Skill
    {
        public static int CD = 0;
        public static int BASE_COST = 10;

        public Hamstring(Player p)
            : base(p, 0, 10, true)
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int damage = (int)Math.Round(45
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0 + (0.1 * Player.GetTalentPoints("Impale")) : 0))
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            CommonAction();
            if (res != ResultType.Parry && res != ResultType.Dodge)
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
        public static new string NAME = "Hamstring";
    }
}

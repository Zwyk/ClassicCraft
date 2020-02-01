using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Whirlwind : Skill
    {
        public static int BASE_COST = 25;
        public static int CD = 10;

        public Whirlwind(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            CommonAction();
            Player.Resource -= Cost;

            int minDmg = (int)Math.Round(Player.MH.DamageMin + Simulation.Normalization(Player.MH) * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Simulation.Normalization(Player.MH) * Player.AP / 14);

            int damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1 )
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return "Whirlwind";
        }
    }
}

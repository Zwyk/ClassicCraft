using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Slam : Spell
    {
        public static int BASE_COST = 15;
        public static int CD = 0;
        public static double CAST_TIME = 1.5;

        public Slam(Player p)
            : base(p, CD, BASE_COST, false, true, School.Physical, CAST_TIME - 0.1 * p.GetTalentPoints("IS"))
        {
        }

        public override void DoAction()
        {
            base.DoAction();

            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            Player.Resource -= Cost;

            int minDmg = (int)Math.Round(Player.MH.DamageMin + Player.MH.Speed * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Player.MH.Speed * Player.AP / 14);

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + 87)
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0 + (0.1 * Player.GetTalentPoints("Impale")) : 0))
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return "Slam";
        }
    }
}

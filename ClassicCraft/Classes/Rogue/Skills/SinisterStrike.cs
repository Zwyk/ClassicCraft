using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SinisterStrike : Skill
    {
        public static int BASE_COST = 45;
        public static int CD = 0;

        public SinisterStrike(Player p)
            : base(p, CD, BASE_COST - (p.GetTalentPoints("ISS") > 0 ? (p.GetTalentPoints("ISS") > 1 ? 5 : 3) : 0)) { }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            Weapon weapon = Player.MH;

            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + Simulation.Normalization(weapon) * (Player.AP + Player.nextAABonus) / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + Simulation.Normalization(weapon) * (Player.AP + Player.nextAABonus) / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + 68)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor)
                * (1 + (0.02 * Player.GetTalentPoints("Agg")))
                * (res == ResultType.Crit ? 1 + (0.06 * Player.GetTalentPoints("Letha")) : 1)
                * (1 + (0.01 * Player.GetTalentPoints("Murder")))
                * Player.DamageMod
                );

            CommonAction();

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Cost / 2;
            }
            else
            {
                Player.Resource -= Cost;
            }

            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                Player.Combo++;
            }

            /*
            if (res == ResultType.Crit && Randomer.NextDouble() < 0.2 * Player.GetTalentPoints("SF"))
            {
                Player.Combo++;
            }
            */

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Sinister Strike";
    }
}

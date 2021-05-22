using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Eviscerate : Skill
    {
        public static int BASE_COST = 35;
        public static int CD = 0;

        // rank 8
        public static int[] min =
        {
            199,
            350,
            501,
            652,
            803,
        };
        // rank 8
        public static int[] max =
        {
            295,
            446,
            597,
            748,
            899,
        };

        public Eviscerate(Player p)
            : base(p, CD, BASE_COST) { }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = min[Player.Combo - 1];
            int maxDmg = max[Player.Combo - 1];

            int damage = (int)Math.Round(
                (Randomer.Next(minDmg, maxDmg + 1) + Player.AP * 0.15)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (1 + (0.02 * Player.GetTalentPoints("Agg")))
                * (1 + (0.05 * Player.GetTalentPoints("IE")))
                * (1 + (0.01 * Player.GetTalentPoints("Murder")))
                * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless") || b.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
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

                if (Player.GetTalentPoints("RS") > 0 && Randomer.NextDouble() < 0.2 * Player.Combo)
                {
                    Player.Resource += 25;
                }

                Player.Combo = 0;

                if (Randomer.NextDouble() < 0.2 * Player.GetTalentPoints("Ruth"))
                {
                    Player.Combo++;
                }
            }

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Eviscerate";
    }
}

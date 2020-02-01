using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shred : Skill
    {
        public static int BASE_COST = 60;
        public static int CD = 0;

        public Shred(Player p)
            : base(p, CD, BASE_COST - p.GetTalentPoints("IS") * 6) { }

        public override void Cast()
        {
            DoAction();
        }

        public override bool CanUse()
        {
            return (Player.Effects.Any(e => e is ClearCasting) || Player.Resource >= Cost) && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(Player.Level * 0.85 + Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.Level * 1.25 + Player.AP / 14);

            int damage = (int)Math.Round(
                (Randomer.Next(minDmg, maxDmg + 1) * 2.25 + 180)
                * (1 + Player.GetTalentPoints("NW") * 0.02)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod);

            CommonAction();

            int cost = Cost;
            if(Player.Effects.Any(e => e is ClearCasting))
            {
                cost = 0;
                Player.Effects.Where(e => e is ClearCasting).First().StackRemove();
            }

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= cost / 2;
            }
            else
            {
                Player.Resource -= cost;
            }

            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                Player.Combo++;
            }

            if (res == ResultType.Crit && Randomer.NextDouble() < 0.5 * Player.GetTalentPoints("BF"))
            {
                Player.Combo++;
            }

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return "Shred";
        }
    }
}

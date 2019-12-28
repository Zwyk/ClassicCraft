using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class FerociousBite : Spell
    {
        public static int COST = 35;
        public static int CD = 0;

        // rank 4
        public static int[] min =
        {
            173,
            301,
            429,
            557,
            685,
        };
        // rank 4
        public static int[] max =
        {
            223,
            351,
            479,
            607,
            735,
        };

        static Random random = new Random();

        public FerociousBite(Player p)
            : base(p, CD, COST) { }

        public override void Cast()
        {
            DoAction();
        }

        public override bool CanUse()
        {
            return (Player.Effects.Any(e => e is ClearCasting) || Player.Resource >= Cost) && Available() && (AffectedByGCD ? Player.HasGCD() : true) && Player.Combo > 0;
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = min[Player.Combo - 1];
            int maxDmg = max[Player.Combo - 1];


            int cost = Cost;
            if (Player.Effects.Any(e => e is ClearCasting))
            {
                cost = 0;
                Player.Effects.Where(e => e is ClearCasting).First().StackRemove();
            }

            int damage = (int)Math.Round(
                (Randomer.Next(minDmg, maxDmg + 1) + Player.AP * 0.15 + 2.5 * (Player.Resource - cost))
                * (1 + Player.GetTalentPoints("FA") * 0.03)
                * Player.Sim.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor));

            CommonAction();
            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource = cost / 2;
            }
            else
            {
                Player.Resource = 0;
                Player.Combo = 0;
            }

            RegisterDamage(new ActionResult(res, damage));
            
            Player.CheckOnHits(true, res);
        }

        public override string ToString()
        {
            return "Ferocious Bite";
        }
    }
}

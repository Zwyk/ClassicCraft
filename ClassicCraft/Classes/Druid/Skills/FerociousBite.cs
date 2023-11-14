using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class FerociousBite : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Ferocious Bite";

        public static int BASE_COST = 35;
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

        public FerociousBite(Player p)
            : base(p, CD, BASE_COST) { }

        public override bool CanUse()
        {
            return (Player.Effects.ContainsKey(ClearCasting.NAME) || Player.Resource >= Cost) && Available() && (AffectedByGCD ? Player.HasGCD() : true) && Player.Combo > 0;
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Target);

            int minDmg = min[Player.Combo - 1];
            int maxDmg = max[Player.Combo - 1];


            int cost = Cost;
            if (Player.Effects.ContainsKey(ClearCasting.NAME))
            {
                cost = 0;
                Player.Effects[ClearCasting.NAME].StackRemove();
            }

            int damage = (int)Math.Round(
                (Randomer.Next(minDmg, maxDmg + 1) + Player.AP * 0.15 + 2.5 * (Player.Resource - cost))
                * (1 + Player.GetTalentPoints("FA") * 0.03)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod);

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

            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
            
            Player.CheckOnHits(true, false, res);
        }
    }
}

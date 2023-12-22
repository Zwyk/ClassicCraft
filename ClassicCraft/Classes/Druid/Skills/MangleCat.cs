using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MangleCat : Skill
    {
        public static int BASE_COST = 40;
        public static int CD = 0;

        public static Effect NewEffect(Player p, Entity t)
        {
            return new CustomEffect(p, t, NAME, false, 60);
        }

        public MangleCat(Player p)
            : base(p, CD, BASE_COST - 5 * p.GetTalentPoints("Fero")) { }

        public override bool CanUse()
        {
            return (Player.Effects.ContainsKey(ClearCasting.NAME) || Player.Resource >= Cost) && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Target);

            int minDmg = (int)Math.Round(Player.Level * 0.85 + Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.Level * 1.25 + Player.AP / 14);

            int damage = (int)Math.Round(
                (Randomer.Next(minDmg, maxDmg + 1) * 3.0)
                * Player.Sim.DamageMod(res, School)
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * Player.SelfModifiers(NAME, Target, School, res));

            CommonAction();

            int cost = Cost;
            if (Player.Effects.ContainsKey(ClearCasting.NAME))
            {
                cost = 0;
                Player.Effects[ClearCasting.NAME].StackRemove();
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

            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString() { return NAME; }
        public static new string NAME = "Mangle";
    }
}

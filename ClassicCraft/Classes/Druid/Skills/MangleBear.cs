using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MangleBear : Skill
    {
        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Mangle";

        public static Effect NewEffect(Player p, Entity t)
        {
            return new CustomEffect(p, t, NAME, false, 60);
        }

        public static int BASE_COST = 20;
        public static int CD = 6;

        public MangleBear(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            Player.applyAtNextAA = null;

            Weapon weapon = Player.MH;

            LockedUntil = Player.Sim.CurrentTime + weapon.Speed / Player.HasteMod;

            ResultType res = Player.YellowAttackEnemy(Target);

            int minDmg = (int)Math.Round(Player.Level * 0.85 + 2.5 * (Player.AP + Player.nextAABonus) / 14);
            int maxDmg = (int)Math.Round(Player.Level * 1.25 + 2.5 * (Player.AP + Player.nextAABonus) / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) * 1.6)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.SelfModifiers(NAME, Target, School.Physical, res)
                * Player.DamageMod
                );

            int threat = (int)Math.Round(damage * Player.ThreatMod);

            int cost = Cost;
            if (Player.Effects.ContainsKey(ClearCasting.NAME))
            {
                cost = 0;
                Player.Effects[ClearCasting.NAME].StackRemove();
            }

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                Player.Resource -= (int)(cost * 0.2);
            }
            else
            {
                Player.Resource -= cost;
            }

            RegisterDamage(new ActionResult(res, damage, threat));

            Player.CheckOnHits(true, false, res);
        }
    }
}

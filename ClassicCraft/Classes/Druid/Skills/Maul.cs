using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Maul : Skill
    {
        public static int BASE_COST = 15;
        public static int CD = 0;

        public static double THREAT_MOD = 1.75;

        public Maul(Player p)
            : base(p, CD, BASE_COST - p.GetTalentPoints("Fero"), true)
        {
        }

        public override bool CanUse()
        {
            return Player.Effects.ContainsKey(ClearCasting.NAME) || Player.Resource >= Cost;
        }

        public override void Cast()
        {
            Player.applyAtNextAA = this;
        }

        public override void DoAction()
        {
            Player.applyAtNextAA = null;

            Weapon weapon = Player.MH;

            LockedUntil = Player.Sim.CurrentTime + weapon.Speed / Player.HasteMod;

            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(Player.Level * 0.85 + 2.5 * (Player.AP + Player.nextAABonus) / 14);
            int maxDmg = (int)Math.Round(Player.Level * 1.25 + 2.5 * (Player.AP + Player.nextAABonus) / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + 101)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * (1 + Player.GetTalentPoints("SF") * 0.1)
                * Player.DamageMod
                );

            int threat = (int)Math.Round(damage * THREAT_MOD * Player.ThreatMod);

            int cost = Cost;
            if (Player.Effects.ContainsKey(ClearCasting.NAME))
            {
                cost = 0;
                Player.Effects[ClearCasting.NAME].StackRemove();
            }

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                Player.Resource -= (int)(Cost*0.2);
            }
            else
            {
                Player.Resource -= Cost;
            }

            RegisterDamage(new ActionResult(res, damage, threat));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Maul";
    }
}

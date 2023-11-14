using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Devastate : Skill
    {
        public static int BASE_COST = 15;
        public static int CD = 0;

        public static int BONUS_THREAT = 101;

        public Devastate(Player p)
            : base(p, CD, BASE_COST - p.GetTalentPoints("FR"))
        {
        }

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Target);

            int minDmg = (int)Math.Round(Player.MH.DamageMin + Simulation.Normalization(Player.MH) * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Simulation.Normalization(Player.MH) * Player.AP / 14);

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) * 0.5 + (35 * 5))
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                * (Target.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                * (Player.Effects.ContainsKey("T4 4P") ? 1.1 : 1)
                );

            int threat = (int)Math.Round((damage + BONUS_THREAT) * Player.ThreatMod);   // TODO + SunderArmor.BONUS_THREAT if <5 stacks

            CommonAction();
            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                Player.Resource -= (int)(Cost * 0.2);
            }
            else
            {
                Player.Resource -= Cost;
            }

            RegisterDamage(new ActionResult(res, damage, threat));

            if (Player.Effects.ContainsKey("T4 4P")) Player.Effects["T4 4P"].EndEffect();

            Player.CheckOnHits(true, false, res);

            SweepingStrikesBuff.CheckProc(Player, damage, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Devastate";
    }
}

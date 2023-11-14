using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Whirlwind : Skill
    {
        public static int BASE_COST = 25;
        public static int CD = 10;

        public static int MAX_TARGETS = 4;

        public Whirlwind(Player p)
            : base(p, CD - (Program.version == Version.TBC ? p.GetTalentPoints("IWW") : 0), BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") + (p.NbSet("Warbringer")>=2?5:0) : 0))
        {
        }

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            CommonAction();
            Player.Resource -= Cost;

            int firstDamage = 0;
            ResultType firstRes = ResultType.Hit;

            Entity t = Target;
            for (int i = 0; i < Math.Min(MAX_TARGETS, Player.Sim.NbTargets); i++)
            {
                Target = Player.Sim.Boss[i];
                ResultType res = Player.YellowAttackEnemy(Target);

                int minDmg = (int)Math.Round(Player.MH.DamageMin + Simulation.Normalization(Player.MH) * Player.AP / 14);
                int maxDmg = (int)Math.Round(Player.MH.DamageMax + Simulation.Normalization(Player.MH) * Player.AP / 14);

                int damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                    * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                    * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                    * Player.DamageMod
                    * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                    * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                    * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                    * (Target.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                    * (Player.Effects.ContainsKey("T4 4P") ? 1.1 : 1)
                    );

                RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));

                Player.CheckOnHits(true, false, res);

                if(i == 1)
                {
                    firstDamage = damage;
                    firstRes = res;
                }

                if (Program.version == Version.TBC && Player.DualWielding && Player.OH != null)
                {
                    minDmg = (int)Math.Round(Player.OH.DamageMin + Simulation.Normalization(Player.OH) * Player.AP / 14);
                    maxDmg = (int)Math.Round(Player.OH.DamageMax + Simulation.Normalization(Player.OH) * Player.AP / 14);

                    damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                        * 0.5 * (1 + 0.05 * Player.GetTalentPoints("DWS"))
                        * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                        * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                        * Player.DamageMod
                        * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                        * (Target.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                        * (Player.Effects.ContainsKey("T4 4P") ? 1.1 : 1)
                        );

                    RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));

                    Player.CheckOnHits(false, false, res);
                }
            }
            Target = t;

            if(Player.Effects.ContainsKey("T4 4P")) Player.Effects["T4 4P"].EndEffect();

            SweepingStrikesBuff.CheckProc(Player, firstDamage, firstRes);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Whirlwind";
    }
}

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

        public Whirlwind(Player p)
            : base(p, CD - (Program.version == Version.TBC ? p.GetTalentPoints("IWW") : 0), BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") + 5*p.Sets["Warbringer"] : 0))
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            CommonAction();
            Player.Resource -= Cost;

            int minDmg = (int)Math.Round(Player.MH.DamageMin + Simulation.Normalization(Player.MH) * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Simulation.Normalization(Player.MH) * Player.AP / 14);

            int damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless")) ? 1.03 : 1)
                );

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);

            if (Program.version == Version.TBC && Player.DualWielding && Player.OH != null)
            {
                minDmg = (int)Math.Round(Player.MH.DamageMin + Simulation.Normalization(Player.MH) * Player.AP / 14);
                maxDmg = (int)Math.Round(Player.MH.DamageMax + Simulation.Normalization(Player.MH) * Player.AP / 14);

                damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                    * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                    * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                    * Player.DamageMod
                    * 0.5 * (1 + 0.05 * Player.GetTalentPoints("DWS"))
                    * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless")) ? 1.03 : 1)
                    );

                RegisterDamage(new ActionResult(res, damage));

                Player.CheckOnHits(false, false, res);
            }
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Whirlwind";
    }
}

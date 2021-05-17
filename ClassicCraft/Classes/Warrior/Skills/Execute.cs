using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Execute : Skill
    {
        public static int BASE_COST = 15;
        public static int CD = 0;

        public Execute(Player p)
            : base(p, CD, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0) - (p.Sets["Onslaught"]>=2?3:0), true)
        {
            switch (Player.GetTalentPoints("IE"))
            {
                case 2: Cost -= 5; break;
                case 1: Cost -= 2; break;
            }
        }

        public override bool CanUse()
        {
            return Player.Sim.Boss.LifePct <= 0.2 && base.CanUse();
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);
            
            int damage = (int)Math.Round(((Program.version == Version.TBC ? 925 : 600) + (Player.Resource - Cost) * (Program.version == Version.TBC ? 21 : 15))
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless")) ? 1.03 : 1)
                );

            CommonAction();
            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource = Cost / 2;
            }
            else
            {
                Player.Resource = 0;
            }

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Execute";
    }
}

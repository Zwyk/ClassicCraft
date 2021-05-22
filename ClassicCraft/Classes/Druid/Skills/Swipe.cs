using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Swipe : Skill
    {
        public static int BASE_COST = 20;
        public static int CD = 0;

        public static int DAMAGE = 83;

        public static double THREAT_MOD = 1.75;

        public Swipe(Player p)
            : base(p, CD, BASE_COST - p.GetTalentPoints("Fero")) { }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int damage = (int)Math.Round(DAMAGE
                * Player.Sim.DamageMod(res)
                * (1 + Player.GetTalentPoints("SF") * 0.1)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                );

            int threat = (int)Math.Round(damage * THREAT_MOD * Player.ThreatMod);

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

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Swipe";
    }
}

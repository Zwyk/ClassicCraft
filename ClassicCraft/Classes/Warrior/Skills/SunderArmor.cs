using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SunderArmor : Skill
    {
        public static int BASE_COST = 15;
        public static int CD = 0;

        public static int BONUS_THREAT = Program.version == Version.TBC ? 301 : 260;

        public SunderArmor(Player p)
            : base(p, CD, BASE_COST - p.GetTalentPoints("ISA") - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0)) { }

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Target);

            int damage = 0;

            int threat = (int)Math.Round((damage + BONUS_THREAT) * Player.ThreatMod);

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
        public static new string NAME = "Sunder Armor";
    }
}

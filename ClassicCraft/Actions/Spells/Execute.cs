using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Execute : Spell
    {
        public Execute(Player p, double baseCD = 0, int ressourceCost = 15, bool gcd = true)
            : base(p, baseCD, ressourceCost, gcd)
        {

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

            int reducedCost;
            switch(Player.GetTalentPoints("IE"))
            {
                case 2: reducedCost = 5; break;
                case 1: reducedCost = 2; break;
                default: reducedCost = 0; break;
            }

            int damage = (int)Math.Round((600 + (Player.Ressource - (15 - reducedCost)) * 15)
                * Player.Sim.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1)
                * (Player.DualWielding() ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            CommonAction();
            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Player.Ressource = 0;
            }

            RegisterDamage(new ActionResult(res, damage));

            if (Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Player, res, Player.GetTalentPoints("DW"));
            }
            if (Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Player, res, Player.GetTalentPoints("Flurry"));
            }
        }

        public override string ToString()
        {
            return "Execute";
        }
    }
}

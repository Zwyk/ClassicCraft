using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Execute : Spell
    {
        public static int COST = 15;
        public static int CD = 0;

        public Execute(Player p)
            : base(p, CD, COST, true)
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

            int damage = (int)Math.Round((600 + (Player.Resource - (15 - reducedCost)) * 15)
                * Player.Sim.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1)
                * (Player.DualWielding() ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

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

            if (Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Player, res, Player.GetTalentPoints("DW"));
            }
            if (Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Player, res, Player.GetTalentPoints("Flurry"));
            }
            if (Player.MH.Enchantment != null && Player.MH.Enchantment.Name == "Crusader")
            {
                Crusader.CheckProc(Player, res, Player.MH.Speed);
            }
        }

        public override string ToString()
        {
            return "Execute";
        }
    }
}

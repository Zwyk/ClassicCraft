using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Execute : Spell
    {
        public Execute(double baseCD = 0, int ressourceCost = 15, bool gcd = true)
            : base(baseCD, ressourceCost, gcd)
        {

        }

        public override bool CanUse()
        {
            return Program.Boss.LifePct <= 0.2 && Program.Player.Ressource >= RessourceCost && Available() && (AffectedByGCD ? Program.Player.HasGCD() : true);
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Program.Player.YellowAttackEnemy(Program.Boss);

            int reducedCost;
            switch(Program.Player.GetTalentPoints("IE"))
            {
                case 2: reducedCost = 5; break;
                case 1: reducedCost = 2; break;
                default: reducedCost = 0; break;
            }

            int damage = (int)Math.Round((600 + (Program.Player.Ressource - (15 - reducedCost)) * 15)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Program.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Program.Player.GetTalentPoints("Impale")) : 1)
                * (Program.Player.DualWielding() ? 1 : (1 + 0.01 * Program.Player.GetTalentPoints("2HS"))));

            CommonAction();
            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Program.Player.Ressource = 0;
            }

            if (Program.Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(res, Program.Player.GetTalentPoints("DW"));
            }
            if (Program.Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(res, Program.Player.GetTalentPoints("Flurry"));
            }

            RegisterDamage(new ActionResult(res, damage));
        }

        public override string ToString()
        {
            return "Exec";
        }
    }
}

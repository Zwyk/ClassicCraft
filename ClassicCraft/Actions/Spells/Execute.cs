using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Execute : Spell
    {
        public Execute(Simulation s, double baseCD = 0, int ressourceCost = 15, bool gcd = true)
            : base(s, baseCD, ressourceCost, gcd)
        {

        }

        public override bool CanUse()
        {
            return Sim.Boss.LifePct <= 0.2 && Sim.Player.Ressource >= RessourceCost && Available() && (AffectedByGCD ? Sim.Player.HasGCD() : true);
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Sim.Player.YellowAttackEnemy(Sim.Boss);

            int reducedCost;
            switch(Sim.Player.GetTalentPoints("IE"))
            {
                case 2: reducedCost = 5; break;
                case 1: reducedCost = 2; break;
                default: reducedCost = 0; break;
            }

            int damage = (int)Math.Round((600 + (Sim.Player.Ressource - (15 - reducedCost)) * 15)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Sim.Player.GetTalentPoints("Impale")) : 1)
                * (Sim.Player.DualWielding() ? 1 : (1 + 0.01 * Sim.Player.GetTalentPoints("2HS"))));

            CommonAction();
            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Sim.Player.Ressource = 0;
            }

            if (Sim.Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Sim, res, Sim.Player.GetTalentPoints("DW"));
            }
            if (Sim.Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Sim, res, Sim.Player.GetTalentPoints("Flurry"));
            }

            RegisterDamage(new ActionResult(res, damage));
        }

        public override string ToString()
        {
            return "Exec";
        }
    }
}

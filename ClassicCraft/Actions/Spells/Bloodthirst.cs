using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodthirst : Spell
    {
        static Random random = new Random();

        public Bloodthirst(Simulation s, double baseCD = 6, int ressourceCost = 30)
            : base(s, baseCD, ressourceCost) {}

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Sim.Player.YellowAttackEnemy(Sim.Boss);

            int damage = (int)Math.Round(0.45 * Sim.Player.AP
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Sim.Player.GetTalentPoints("Impale")) : 1)
                * (Sim.Player.DualWielding() ? 1 : (1 + 0.01 * Sim.Player.GetTalentPoints("2HS"))));

            CommonAction();
            if(res != ResultType.Parry && res != ResultType.Dodge)
            {
                Sim.Player.Ressource -= RessourceCost;
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
            return "BT";
        }
    }
}

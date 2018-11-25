using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Whirlwind : Spell
    {
        static Random random = new Random();

        public Whirlwind(Simulation s, double baseCD = 10, int ressourceCost = 25)
            : base(s, baseCD, ressourceCost)
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Sim.Player.YellowAttackEnemy(Sim.Boss);

            CommonAction();
            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Sim.Player.Ressource -= RessourceCost;
            }

            int minDmg = (int)Math.Round(Sim.Player.MH.DamageMin + Program.Normalization(Sim.Player.MH) * Sim.Player.AP / 14);
            int maxDmg = (int)Math.Round(Sim.Player.MH.DamageMax + Program.Normalization(Sim.Player.MH) * Sim.Player.AP / 14);

            if (Sim.Player.OH != null)
            {
                minDmg += (int)Math.Round(Sim.Player.OH.DamageMin + Program.Normalization(Sim.Player.OH) * Sim.Player.AP / 14);
                maxDmg += (int)Math.Round(Sim.Player.OH.DamageMax + Program.Normalization(Sim.Player.OH) * Sim.Player.AP / 14);
            }

            int damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Sim.Player.GetTalentPoints("Impale")) : 1 )
                * (Sim.Player.DualWielding() ? 1 : (1 + 0.01 * Sim.Player.GetTalentPoints("2HS"))));

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
            return "WW";
        }
    }
}

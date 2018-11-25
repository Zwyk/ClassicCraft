using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HeroicStrike : Spell
    {
        public HeroicStrike(Simulation s, double baseCD = 0, int ressourceCost = 15, bool gcd = true)
            : base(s, baseCD, ressourceCost, gcd)
        {
        }

        public override void Cast()
        {
            Sim.Player.applyAtNextAA = this;
        }

        public override void DoAction()
        {
            Sim.Player.applyAtNextAA = null;

            Random random = new Random();

            Weapon weapon = Sim.Player.MH;

            LockedUntil = Sim.CurrentTime + weapon.Speed / Sim.Player.HasteMod;
            
            ResultType res = Sim.Player.YellowAttackEnemy(Sim.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + weapon.Speed * Sim.Player.AP / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + weapon.Speed * Sim.Player.AP / 14);

            int damage = (int)Math.Round((random.Next(minDmg, maxDmg + 1) + 157)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Sim.Player.GetTalentPoints("Impale")) : 1)
                * (Sim.Player.DualWielding() ? 1 : (1 + 0.01 * Sim.Player.GetTalentPoints("2HS"))));

            if (res != ResultType.Parry && res != ResultType.Dodge)
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
            if (Sim.Player.GetTalentPoints("UW") > 0)
            {
                UnbridledWrath.CheckProc(Sim, res, Sim.Player.GetTalentPoints("UW"));
            }

            RegisterDamage(new ActionResult(res, damage));
        }

        public override string ToString()
        {
            return "HS";
        }
    }
}

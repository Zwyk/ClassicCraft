using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class AutoAttack : Action
    {
        static Random random = new Random();

        public bool MH { get; set; }
        public Weapon Weapon { get; set; }

        public AutoAttack(Simulation s, Weapon weapon, bool mh)
            : base(s, weapon.Speed)
        {
            Weapon = weapon;
            MH = mh;
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            NextAA();

            ResultType res = Sim.Player.WhiteAttackEnemy(Sim.Boss, MH);

            int minDmg = (int)Math.Round(Weapon.DamageMin + Weapon.Speed * Sim.Player.AP / 14);
            int maxDmg = (int)Math.Round(Weapon.DamageMax + Weapon.Speed * Sim.Player.AP / 14);

            int damage = 0;

            damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Sim.Boss.Armor)
                * (Sim.Player.DualWielding() ? (MH ? 1 : 0.5 * (1 + (0.05 * Sim.Player.GetTalentPoints("DWS")))) : (1 + 0.01 * Sim.Player.GetTalentPoints("2HS"))));

            Sim.Player.Ressource += (int)Math.Round(Program.RageGained(damage, Weapon.Speed, res, MH));

            if(Sim.Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Sim, res, Sim.Player.GetTalentPoints("DW"));
            }
            if (Sim.Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Sim, res, Sim.Player.GetTalentPoints("Flurry"));
            }
            if(Sim.Player.GetTalentPoints("UW") > 0)
            {
                UnbridledWrath.CheckProc(Sim, res, Sim.Player.GetTalentPoints("UW"));
            }

            RegisterDamage(new ActionResult(res, damage));
        }

        public void NextAA()
        {
            LockedUntil = Sim.CurrentTime + Weapon.Speed / Sim.Player.HasteMod;
        }

        public override string ToString()
        {
            return "AA " + (MH ? "MH" : "OH");
        }
    }
}

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

        public AutoAttack(Weapon weapon, bool mh)
            : base(weapon.Speed)
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

            ResultType res = Program.Player.WhiteAttackEnemy(Program.Boss, MH);

            int minDmg = (int)Math.Round(Weapon.DamageMin + Weapon.Speed * Program.Player.AP / 14);
            int maxDmg = (int)Math.Round(Weapon.DamageMax + Weapon.Speed * Program.Player.AP / 14);

            int damage = 0;

            damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Program.Boss.Armor)
                * (Program.Player.DualWielding() ? (MH ? 1 : 0.5 * (1 + (0.05 * Program.Player.GetTalentPoints("DWS")))) : (1 + 0.01 * Program.Player.GetTalentPoints("2HS"))));

            Program.Player.Ressource += (int)Math.Round(Program.RageGained(damage, Weapon.Speed, res, MH));

            if(Program.Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(res, Program.Player.GetTalentPoints("DW"));
            }
            if (Program.Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(res, Program.Player.GetTalentPoints("Flurry"));
            }
            if(Program.Player.GetTalentPoints("UW") > 0)
            {
                UnbridledWrath.CheckProc(res, Program.Player.GetTalentPoints("UW"));
            }

            RegisterDamage(new ActionResult(res, damage));
        }

        public void NextAA()
        {
            LockedUntil = Program.currentTime + Weapon.Speed / Program.Player.HasteMod;
        }

        public override string ToString()
        {
            return "AA " + (MH ? "MH" : "OH");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HeroicStrike : Spell
    {
        public HeroicStrike(double baseCD = 0, int ressourceCost = 15, bool gcd = true)
            : base(baseCD, ressourceCost, gcd)
        {
        }

        public override void Cast()
        {
            Program.Player.applyAtNextAA = this;
        }

        public override void DoAction()
        {
            Program.Player.applyAtNextAA = null;

            Random random = new Random();

            Weapon weapon = Program.Player.MH;

            LockedUntil = Program.currentTime + weapon.Speed / Program.Player.HasteMod;
            
            ResultType res = Program.Player.YellowAttackEnemy(Program.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + weapon.Speed * Program.Player.AP / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + weapon.Speed * Program.Player.AP / 14);

            int damage = (int)Math.Round((random.Next(minDmg, maxDmg + 1) + 157)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Program.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Program.Player.GetTalentPoints("Impale")) : 1)
                * (Program.Player.DualWielding() ? 1 : (1 + 0.01 * Program.Player.GetTalentPoints("2HS"))));

            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Program.Player.Ressource -= RessourceCost;
            }

            if (Program.Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(res, Program.Player.GetTalentPoints("DW"));
            }
            if (Program.Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(res, Program.Player.GetTalentPoints("Flurry"));
            }
            if (Program.Player.GetTalentPoints("UW") > 0)
            {
                UnbridledWrath.CheckProc(res, Program.Player.GetTalentPoints("UW"));
            }

            RegisterDamage(new ActionResult(res, damage));
        }

        public override string ToString()
        {
            return "HS";
        }
    }
}

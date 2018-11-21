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
            Player.Instance.applyAtNextAA = this;
        }

        public override void DoAction()
        {
            Player.Instance.applyAtNextAA = null;

            Random random = new Random();

            Weapon weapon = Player.Instance.MH;

            LockedUntil = Program.currentTime + weapon.Speed / Program.speedMod;

            bool crit = random.NextDouble() < Player.Instance.CritChance;

            int minDmg = (int)Math.Round(weapon.DamageMin + weapon.Speed * Player.Instance.AP / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + weapon.Speed * Player.Instance.AP / 14);

            int damage = (int)Math.Round((random.Next(minDmg, maxDmg + 1) + 157)
                * (1 + (crit ? 1.2 : 0))
                * 1.03);

            Player.Instance.Ressource -= RessourceCost;

            RegisterDamage(new ActionResult(crit ? ResultType.Crit : ResultType.Hit, damage));
        }

        public override string ToString()
        {
            return "HS";
        }
    }
}

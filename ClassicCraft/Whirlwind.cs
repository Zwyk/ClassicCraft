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

        public Whirlwind(double baseCD = 10, int ressourceCost = 25)
            : base(baseCD, ressourceCost)
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            CommonSpell();

            bool crit = random.NextDouble() < Player.Instance.CritChance;

            int minDmg = (int)Math.Round(Player.Instance.MH.DamageMin + Program.Normalization(Player.Instance.MH) * Player.Instance.AP / 14);
            int maxDmg = (int)Math.Round(Player.Instance.MH.DamageMax + Program.Normalization(Player.Instance.MH) * Player.Instance.AP / 14);

            if (Player.Instance.OH != null)
            {
                minDmg += (int)Math.Round(Player.Instance.OH.DamageMin + Program.Normalization(Player.Instance.OH) * Player.Instance.AP / 14);
                maxDmg += (int)Math.Round(Player.Instance.OH.DamageMax + Program.Normalization(Player.Instance.OH) * Player.Instance.AP / 14);
            }

            int damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * (1 + (crit ? 1.2 : 0))
                * 1.03);

            RegisterDamage(new ActionResult(crit ? ResultType.Crit : ResultType.Hit, damage));
        }

        public override string ToString()
        {
            return "WW";
        }
    }
}

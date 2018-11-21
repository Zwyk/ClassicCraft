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

        public Bloodthirst(double baseCD = 6, int ressourceCost = 30)
            : base(baseCD, ressourceCost) {}

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            bool crit = random.NextDouble() < Player.Instance.CritChance;

            int damage = (int)Math.Round(0.45 * Player.Instance.AP
                * (1 + (crit ? 1.2 : 0))
                * 1.03);

            CommonAction();
            Player.Instance.Ressource -= RessourceCost;

            RegisterDamage(new ActionResult(crit ? ResultType.Crit : ResultType.Hit, damage));
        }

        public override string ToString()
        {
            return "BT";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowBolt : Spell
    {
        public static int BASE_COST = 370;
        public static int CD = 0;
        public static double CAST_TIME = 3;

        public static int MIN_DMG = 455;
        public static int MAX_DMG = 507;

        public double castTimeKeeper;

        public ShadowBolt(Player p)
            : base(p, CD, (int)(BASE_COST * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, true, CAST_TIME - 0.1 * p.GetTalentPoints("ISB"))
        {
            castTimeKeeper = CastTime;
        }

        public override void Cast()
        {
            if(Player.Effects.Any(e => e is ShadowTrance))
            {
                CastTime = 0;
            }
            else
            {
                CastTime = castTimeKeeper;
            }
            base.Cast();
        }

        public override void DoAction()
        {
            base.DoAction();

            ResultType res = Player.SpellAttackEnemy(Player.Sim.Boss);

            CommonAction();
            Player.Resource -= Cost;

            int minDmg = MIN_DMG;
            int maxDmg = MAX_DMG;

            int damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                * Player.Sim.DamageMod(res)
                * Entity.MagicMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1)
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            RegisterDamage(new ActionResult(res, damage));
        }

        public override string ToString()
        {
            return "Slam";
        }
    }
}

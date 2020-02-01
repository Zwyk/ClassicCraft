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

        public static double RATIO = CAST_TIME / 3.5;

        public static int MIN_DMG = 455;
        public static int MAX_DMG = 507;

        public double castTimeKeeper;

        public ShadowBolt(Player p)
            : base(p, CD, (int)(BASE_COST * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, true, School.Shadow, CAST_TIME - 0.1 * p.GetTalentPoints("ISB"))
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
            
            ResultType res;
            double mitigation = Simulation.MagicMitigation(Player.Sim.Boss.ResistChances[School]);
            if(mitigation == 0)
            {
                res = ResultType.Resist;
            }
            else
            {
                res = Player.SpellAttackEnemy(Player.Sim.Boss);
            }

            CommonManaSpell();

            int minDmg = MIN_DMG;
            int maxDmg = MAX_DMG;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                * Player.Sim.DamageMod(res, School)
                * mitigation
                * Player.DamageMod
                );

            RegisterDamage(new ActionResult(res, damage));
        }

        public override string ToString()
        {
            return "Shadow Bolt";
        }
    }
}

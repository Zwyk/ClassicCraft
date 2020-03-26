using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class HammerOfWrath : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Hammer of Wrath";

        public static int CD = 6;
        private double RATIO = 0.2857;

        public HammerOfWrath(Player p)
            : base(p, CD, 425, true, true, School.Light, 0)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            ResultType res = Player.SpellAttackEnemy(Player.Sim.Boss, false, 0, 0);

            //  Can't glance
            if (res == ResultType.Hit || res == ResultType.Crit)
            {
                int minDmg = 504;
                int maxDmg = 556;

                //  Uses melee crit multiplier
                if (res == ResultType.Crit)
                {
                    minDmg *= 2;
                    maxDmg *= 2;
                }

                //  Include SotC bonus
                int holyBoost = 0;
                if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME))
                {
                    holyBoost = ((JudgementOfTheCrusaderDebuff)Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME]).HolyDmgIncrease;
                }

                int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + ((holyBoost + Player.SP) * RATIO))
                    * Player.Sim.DamageMod(res, School)
                    * (Player.Sim.Boss.Effects.ContainsKey(SpellVulnerability.NAME) ? ((SpellVulnerability)Player.Sim.Boss.Effects[SpellVulnerability.NAME]).Modifier : 1)
                    * Player.DamageMod
                    );

                RegisterDamage(new ActionResult(res, damage));
            }

            //  Does trigger GCD after cast
            CommonManaSpell();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JudgementOfRighteousness : Judgement
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "=== JUDGEMENT === JoR";

        private double RATIO = 0.5;

        public int minDmg = 170;
        public int maxDmg = 187;

        public JudgementOfRighteousness(Player p) : base(p)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            ResultType res = Player.SpellAttackEnemy(Player.Sim.Boss);

            int damage = 0;

            //  Can't glance
            if (res == ResultType.Hit || res == ResultType.Crit)
            {
                //  Include SotC bonus
                int holyBoost = 0;
                if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME)) {
                    holyBoost = ((JudgementOfTheCrusaderDebuff)Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME]).HolyDmgIncrease;
                }

                //  Include spell resistance
                double spellResist = 1.0;

                /*  I don't think this factors in, for some reason, or there is a different dmg *boost* missing to offset this
                spellResist = 1 - Simulation.AverageResistChance(315);
                */

                //  Include talented SoR
                double impSoRMultiplier = 1.0 + 0.03 * Player.GetTalentPoints("ImpSoR");

                damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + ((holyBoost + Player.SP) * RATIO))
                    * Player.Sim.DamageMod(ResultType.Hit, School.Light)
                    * (Player.Sim.Boss.Effects.ContainsKey(SpellVulnerability.NAME) ? ((SpellVulnerability)Player.Sim.Boss.Effects[SpellVulnerability.NAME]).Modifier : 1)
                    * spellResist
                    * Player.DamageMod
                    * impSoRMultiplier
                    );

                //  Turn on Vengeance
                if (Player.GetTalentPoints("Veng") > 0)
                {
                    Vengeance.CheckProc(Player, res);
                }
            }

            RegisterDamage(new ActionResult(res, damage));
        }
    }
}

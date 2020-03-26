using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JudgementOfCommand_Base : Judgement
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "=== JUDGEMENT === JoC";

        private double RATIO = 0.43;

        public int minDmg_Normal = 1;
        public int maxDmg_Normal = 1;
        public int minDmg_Stunned = 1;
        public int maxDmg_Stunned = 1;

        public JudgementOfCommand_Base(Player p) : base(p)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            ResultType res = Player.MeleeSpellAttackEnemy(Player.Sim.Boss);

            int damage = 0;

            //  Can't glance
            if (res == ResultType.Hit || res == ResultType.Crit)
            {
                int minDmg = minDmg_Normal;
                int maxDmg = maxDmg_Normal;

                //  Double dmg if target is stunned
                if (Player.Sim.Boss.Effects.ContainsKey(HammerOfJusticeDebuff.NAME))
                {
                    minDmg = minDmg_Stunned;
                    maxDmg = maxDmg_Stunned;
                }

                //  Include SotC bonus
                int holyBoost = 0;
                if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME)) {
                    holyBoost = ((JudgementOfTheCrusaderDebuff)Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME]).HolyDmgIncrease;
                }

                damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + ((holyBoost + Player.SP) * RATIO))
                    * Player.Sim.DamageMod(res, School.Physical)    //  Crit modifier using Melee multiplier
                    * (Player.Sim.Boss.Effects.ContainsKey(SpellVulnerability.NAME) ? ((SpellVulnerability)Player.Sim.Boss.Effects[SpellVulnerability.NAME]).Modifier : 1)
                    * (1 - Simulation.AverageResistChance(315))
                    * Player.DamageMod
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

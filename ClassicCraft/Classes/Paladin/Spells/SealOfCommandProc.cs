using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SealOfCommandProc : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "<<< PROC >>> SoC";

        private static double RATIO = 0.29;

        public SealOfCommandProc(Player p, Entity target)
            : base(p, 0, 0, false, false, School.Physical)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            //  Find out whether it hits/crits
            ResultType res = Player.WhiteAttackEnemy(Player.Sim.Boss, true);

            int damage = 0;

            //  Can't glance, so treat glance as a hit
            if (res == ResultType.Glance)
            {
                res = ResultType.Hit;
            }

            if (res == ResultType.Hit || res == ResultType.Crit)
            {
                //  Calculate weapon dmg
                double weapMin = Player.MH.DamageMin + Player.MH.Speed * (Player.AP + Player.nextAABonus) / 14;
                double weapMax = Player.MH.DamageMax + Player.MH.Speed * (Player.AP + Player.nextAABonus) / 14;

                //  70% weapon damage
                int minDmg = (int)(0.7 * weapMin);
                int maxDmg = (int)(0.7 * weapMax);

                //  Include SotC bonus
                int holyBoost = 0;
                if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME))
                {
                    holyBoost = ((JudgementOfTheCrusaderDebuff)Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME]).HolyDmgIncrease;
                }

                //  Include spell resistance
                double spellResist = 1.0;

                /*  I don't think this factors in, for some reason, or there is a different dmg *boost* missing to offset this
                spellResist = 1 - Simulation.AverageResistChance(315);
                */

                damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + ((holyBoost + Player.SP) * RATIO))
                    * Player.Sim.DamageMod(res, School.Physical)    //  Crit modifier using Melee multiplier
                    * (Player.Sim.Boss.Effects.ContainsKey(SpellVulnerability.NAME) ? ((SpellVulnerability)Player.Sim.Boss.Effects[SpellVulnerability.NAME]).Modifier : 1)
                    * spellResist
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

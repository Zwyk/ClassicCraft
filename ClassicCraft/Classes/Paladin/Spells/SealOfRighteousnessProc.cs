using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SealOfRighteousnessProc : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "<<< PROC >>> SoR";

        public SealOfRighteousnessProc(Player p, Entity target)
            : base(p, 0, 0, false, false, School.Light)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            //
            //  SoR always hits when melee hits
            //
            int damage = 0;


            //  Include SotC bonus
            int holyBoost = 0;
            if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME))
            {
                holyBoost = ((JudgementOfTheCrusaderDebuff)Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME]).HolyDmgIncrease;
            }

            //  Set coefficient differently for 1H versus 2H
            double coefficient = 0.12;  //  2H coeff.
            int dmgAmt = (int)(33 + 2.3 * Player.MH.Speed * 10 - 34.5);
            if (!Player.MH.TwoHanded)
            {
                coefficient = 0.1;
                dmgAmt = (int)(22 + 2.0 * Player.MH.Speed * 10 - 30);
            }

            //  Include spell resistance
            double spellResist = 1.0;

            /*  I don't think this factors in, for some reason, or there is a different dmg *boost* missing to offset this
            spellResist = 1 - Simulation.AverageResistChance(315);
            */

            //  Include talented SoR
            double impSoRMultiplier = 1.0 + 0.03 * Player.GetTalentPoints("ImpSoR");

            damage = (int)Math.Round((dmgAmt + ((holyBoost + Player.SP) * coefficient))
                * Player.Sim.DamageMod(ResultType.Hit, School.Light)
                * (Player.Sim.Boss.Effects.ContainsKey(SpellVulnerability.NAME) ? ((SpellVulnerability)Player.Sim.Boss.Effects[SpellVulnerability.NAME]).Modifier : 1)
                * spellResist
                * Player.DamageMod
                * impSoRMultiplier
                );

            //  Turn on Vengeance
            if (Player.GetTalentPoints("Veng") > 0)
            {
                Vengeance.CheckProc(Player, ResultType.Hit);
            }

            RegisterDamage(new ActionResult(ResultType.Hit, damage));
        }
    }
}

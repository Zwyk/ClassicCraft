using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ConsecrationDoT_Rank5 : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Consecration (Rank 5) DoT";

        public static double DURATION = 7;
        public static double RATIO = 0.33;
        public static int NB_TICKS = (int)DURATION;

        public static double DMG = 384;

        public ConsecrationDoT_Rank5(Player p, Entity target)
            : base(p, target, false, DURATION, 1, 1)
        {
        }

        public override int GetTickDamage()
        {
            //  Include SotC bonus
            int holyBoost = 0;

            if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME))
            {
                holyBoost = ((JudgementOfTheCrusaderDebuff)Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME]).HolyDmgIncrease;
            }

            int dmg = (int)Math.Round((DMG + (holyBoost + Player.SP) * RATIO) / NB_TICKS
                * Player.DamageMod
                * (1 - Simulation.AverageResistChance(315))
                * (Player.Sim.Boss.Effects.ContainsKey(SpellVulnerability.NAME) ? ((SpellVulnerability)Player.Sim.Boss.Effects[SpellVulnerability.NAME]).Modifier : 1)
                );

            return dmg;
        }

        public override void ApplyTick(int damage)
        {
            //  Apparently ticks can be silently resisted
            ResultType res = Simulation.MagicMitigationBinary(Player.Sim.Boss.MagicResist[School.Light]);
            if (res == ResultType.Hit)
            {
                //  Recalculate tick damage each time.
                int currDmg = GetTickDamage();

                base.ApplyTick(currDmg);
            }
            else
            {
                if (Program.logFight)
                {
                    Program.Log(string.Format("{0:N2} : {1} tick {2}", Player.Sim.CurrentTime, ToString(), res));
                }
            }
        }
    }
}

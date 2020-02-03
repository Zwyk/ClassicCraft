using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SliceAndDiceBuff : Effect
    {
        public static int[] LENGTH =
        {
            9,
            12,
            15,
            18,
            21
        };

        public static double DurationCalc(Player p)
        {
            return p.Combo == 0 ? 0 : LENGTH[p.Combo - 1] * (1 + 0.15 * p.GetTalentPoints("ISD"));
        }

        public double Bonus { get; set; }

        public SliceAndDiceBuff(Player p, double bonusPct = 30)
            : base(p, p, true, DurationCalc(p), 1)
        {
            Bonus = 1 + bonusPct / 100;
        }

        public override void Refresh()
        {
            BaseLength = DurationCalc(Player);
            CurrentStacks = BaseStacks;
            End = Player.Sim.CurrentTime + BaseLength;
            AppliedTimes.Add(Player.Sim.CurrentTime);

            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} refreshed", Player.Sim.CurrentTime, ToString()));
            }
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.HasteMod *= Bonus;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.HasteMod /= Bonus;
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Slice And Dice's Buff";
    }
}

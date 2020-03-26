using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JudgementOfTheCrusaderDebuff : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "--- DEBUFF --- JotC";

        public static int LENGTH = 10;

        public int BaseHolyDmgBonus = 140;
        public int HolyDmgIncrease;

        public JudgementOfTheCrusaderDebuff(Player p)
            : base(p, p.Sim.Boss, false, LENGTH, 1)
        {
            HolyDmgIncrease = BaseHolyDmgBonus + BaseHolyDmgBonus * 5 * p.GetTalentPoints("ImpSotC") / 100;
        }

        //  Possibly move this into a shared class...
        public static void RefreshJudgement(Player p, ResultType res)
        {
            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Glance)
            {
                if (p.Sim.Boss.Effects.ContainsKey(NAME))
                {
                    p.Sim.Boss.Effects[NAME].Refresh();
                }
            }
        }
    }
}


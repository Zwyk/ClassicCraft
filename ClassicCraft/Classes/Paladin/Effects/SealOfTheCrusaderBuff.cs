using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SealOfTheCrusaderBuff : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "--- BUFF --- SotC";

        public static int LENGTH = 30;

        public int BaseAPBonus = 326;
        public double APBonus { get; set; }

        public SealOfTheCrusaderBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();

            APBonus = BaseAPBonus + BaseAPBonus * 5 * Player.GetTalentPoints("ImpSotC") / 100;

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + APBonus);
            Player.HasteMod *= 1.4;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) - APBonus);
            Player.HasteMod /= 1.4;
        }
    }
}

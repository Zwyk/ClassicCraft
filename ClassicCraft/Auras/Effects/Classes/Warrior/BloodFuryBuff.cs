using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BloodFuryBuff : Effect
    {
        public static int LENGTH = 15;

        public double Bonus { get; set; }

        public BloodFuryBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Bonus = Player.AP * 0.25;

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + Bonus);
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) - Bonus);
        }

        public override string ToString()
        {
            return "Blood Fury's Buff";
        }
    }
}

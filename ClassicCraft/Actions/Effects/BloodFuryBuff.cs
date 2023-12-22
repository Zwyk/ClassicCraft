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

        public override void StartEffect()
        {
            base.StartEffect();

            Bonus = Player.AP * 0.25;

            Player.Attributes.AddToValue(Attribute.AP, Bonus * (1 + 0.02 * Player.GetTalentPoints("IBStance")));
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.Attributes.AddToValue(Attribute.AP, -Bonus * (1 + 0.02 * Player.GetTalentPoints("IBStance")));
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Blood Fury's Buff";
    }
}

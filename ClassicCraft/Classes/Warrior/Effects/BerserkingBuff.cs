using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BerserkingBuff : Effect
    {
        public static int LENGTH = 10;

        public double Bonus { get; set; }

        public BerserkingBuff(Player p, double bonusPct = 30)
            : base(p, p, true, LENGTH, 1)
        {
            Bonus = 1 + bonusPct / 100;
        }

        public override void StartBuff()
        {
            base.StartBuff();

            //Bonus = 1.1 + 0.2 * (1 - Player.LifePct);

            Player.HasteMod *= Bonus;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.HasteMod /= Bonus;
        }

        public override string ToString()
        {
            return "Blood Fury's Buff";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Crusader : Effect
    {
        public static int LENGTH = 15;

        public double Bonus { get; set; }

        public Crusader(Player p)
            : base(p, p, true, LENGTH, 1)
        {
            Bonus = 100;
        }

        public static void CheckProc(Player p, ResultType type, double weaponSpeed)
        {
            if (type == ResultType.Hit || type == ResultType.Crit || type == ResultType.Block || type == ResultType.Glancing)
            {
                if (Randomer.NextDouble() < weaponSpeed * 1.82 / 100)
                {
                    if (p.Effects.Any(e => e is Crusader))
                    {
                        Effect current = p.Effects.Where(e => e is Crusader).First();
                        current.Refresh();
                    }
                    else
                    {
                        Crusader flu = new Crusader(p);
                        flu.StartBuff();
                    }
                }
            }
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.BonusAttributes.SetValue(Attribute.Strength, Player.BonusAttributes.GetValue(Attribute.Strength) + Bonus);
            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + Bonus*2);
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.BonusAttributes.SetValue(Attribute.Strength, Player.BonusAttributes.GetValue(Attribute.Strength) - Bonus);
            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) - Bonus*2);
        }

        public override string ToString()
        {
            return "Crusader";
        }
    }
}

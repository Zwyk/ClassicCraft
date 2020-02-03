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

        public static double BONUS = 100;

        public Crusader(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public static void CheckProc(Player p, ResultType type, double weaponSpeed)
        {
            if (type == ResultType.Hit || type == ResultType.Crit || type == ResultType.Block || type == ResultType.Glance)
            {
                if (Randomer.NextDouble() < weaponSpeed / 60)
                {
                    if (p.Effects.ContainsKey(Crusader.NAME))
                    {
                        p.Effects[Crusader.NAME].Refresh();
                    }
                    else
                    {
                        new Crusader(p).StartEffect();
                    }
                }
            }
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.BonusAttributes.SetValue(Attribute.Strength, Player.BonusAttributes.GetValue(Attribute.Strength) + BONUS);
            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + BONUS*2);
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.BonusAttributes.SetValue(Attribute.Strength, Player.BonusAttributes.GetValue(Attribute.Strength) - BONUS);
            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) - BONUS*2);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Crusader";
    }
}

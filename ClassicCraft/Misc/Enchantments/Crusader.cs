using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Crusader : Effect
    {
        // DEPRECATED

        public static int LENGTH = 15;

        public static double BONUS = 100;

        public double Bonus { get; set; }

        public Crusader(Player p)
            : base(p, p, true, LENGTH, 1)
        {
            Bonus = BONUS * (p.Buffs.Any(b => b.Name.ToLower().Contains("blessing of kings")) ? 1.1 : 1) * (1 + 0.02 * p.GetTalentPoints("Vitality"));
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

            Player.Attributes.AddToValue(Attribute.Strength, Bonus);
            Player.Attributes.AddToValue(Attribute.AP, Bonus * 2 * (Program.version == Version.TBC ? 1 + 0.02 * Player.GetTalentPoints("IBStance") : 1));
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.Attributes.AddToValue(Attribute.Strength, -Bonus);
            Player.Attributes.AddToValue(Attribute.AP, -Bonus * 2 * (Program.version == Version.TBC ? 1 + 0.02 * Player.GetTalentPoints("IBStance") : 1));
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Crusader";
    }
}

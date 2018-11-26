using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Flurry : Effect
    {
        public double Haste { get; set; }

        public Flurry(Player p, int points, Entity target, double baseLength = 15, int stacks = 3)
            : base(p, target, true, baseLength, stacks)
        {
            Haste = 1 + 0.1 + ((points - 1) * 0.05);
        }

        public static void CheckProc(Player p, ResultType type, int points)
        {
            if (p.Effects.Any(e => e is Flurry))
            {
                Effect current = p.Effects.Where(e => e is Flurry).First();
                if (type == ResultType.Crit)
                {
                    current.Refresh();
                }
                else
                {
                    current.StackRemove();
                }
            }
            else if (type == ResultType.Crit)
            {
                Flurry flu = new Flurry(p, points, p);
                flu.StartBuff();
            }
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.HasteMod *= Haste;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.HasteMod /= Haste;
        }

        public override string ToString()
        {
            return "Flurry";
        }
    }
}

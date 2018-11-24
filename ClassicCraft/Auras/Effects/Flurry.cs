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

        public Flurry(int points, Entity target, double baseLength = 15, int stacks = 3)
            : base(target, true, baseLength, stacks)
        {
            Haste = 1 + 0.1 + ((points - 1) * 0.05);
        }

        public static void CheckProc(ResultType type, int points)
        {
            if (Program.Player.Effects.Any(e => e is Flurry))
            {
                Effect current = Program.Player.Effects.Where(e => e is Flurry).First();
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
                Flurry flu = new Flurry(points, Program.Player);
                flu.StartBuff();
            }
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Program.Player.HasteMod *= Haste;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Program.Player.HasteMod /= Haste;
        }

        public override string ToString()
        {
            return "Flurry";
        }
    }
}

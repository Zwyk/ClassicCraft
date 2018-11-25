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

        public Flurry(Simulation s, int points, Entity target, double baseLength = 15, int stacks = 3)
            : base(s, target, true, baseLength, stacks)
        {
            Haste = 1 + 0.1 + ((points - 1) * 0.05);
        }

        public static void CheckProc(Simulation sim, ResultType type, int points)
        {
            if (sim.Player.Effects.Any(e => e is Flurry))
            {
                Effect current = sim.Player.Effects.Where(e => e is Flurry).First();
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
                Flurry flu = new Flurry(sim, points, sim.Player);
                flu.StartBuff();
            }
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Sim.Player.HasteMod *= Haste;
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Sim.Player.HasteMod /= Haste;
        }

        public override string ToString()
        {
            return "Flurry";
        }
    }
}

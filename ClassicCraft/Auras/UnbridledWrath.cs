using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class UnbridledWrath : Aura
    {
        public UnbridledWrath(Simulation s, Entity target)
            : base(s, target)
        {
        }

        public static void CheckProc(Simulation sim, ResultType res, int points)
        {
            if(res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block)
            {
                if(Program.random.NextDouble() < (0.08 * points))
                {
                    sim.Player.Ressource += 1;
                }
            }
        }
    }
}
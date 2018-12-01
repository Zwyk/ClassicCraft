using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class UnbridledWrath : Aura
    {
        public UnbridledWrath(Player p, Entity target)
            : base(p, target)
        {
        }

        public static void CheckProc(Player p, ResultType res, int points)
        {
            if(res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block)
            {
                if(p.Sim.random.NextDouble() < (0.08 * points))
                {
                    p.Ressource += 1;
                }
            }
        }
    }
}
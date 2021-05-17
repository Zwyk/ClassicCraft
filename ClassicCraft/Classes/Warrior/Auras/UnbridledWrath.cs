using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class UnbridledWrath : Aura
    {
        public static new string NAME = "Unbridled Wrath";
        public override string ToString()
        {
            return NAME;
        }

        public UnbridledWrath(Player p)
            : base(p)
        {
        }

        public static bool CheckProc(Player p, ResultType res, int points, double speed)
        {
            if(res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                double chance = (Program.version == Version.Vanilla ? 0.08 : speed * 3 / 60) * points;
                if (Randomer.NextDouble() < chance)
                {
                    p.Resource += 1;
                    return true;
                }
            }
            return false;
        }
    }
}
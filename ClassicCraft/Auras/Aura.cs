using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Aura : SimulationObject
    {
        //public Entity Source { get; set; }
        public Entity Target { get; set; }

        public Aura(Simulation s, Entity target)
            : base(s)
        {
            Target = target;
        }
    }
}

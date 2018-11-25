using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SimulationObject
    {
        public Simulation Sim { get; set; }

        public SimulationObject(Simulation sim)
        {
            Sim = sim;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class TheUnstoppableForce : Weapon
    {
        public TheUnstoppableForce()
            : base(175, 292, 3.8, true, WeaponType.Mace,
                  new Attributes(
                      new Dictionary<string, double>()
                      {
                          { "Str", 19 },
                          { "Sta", 15 },
                          { "Crit", 2 },
                      }
                  ),
                  0, "The Unstoppable Force", null, null, null)
        {
        }
    }
}

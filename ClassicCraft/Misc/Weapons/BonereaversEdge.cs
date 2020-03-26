using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class BonereaversEdge : Weapon
    {
        public BonereaversEdge()
            : base(206, 310, 3.4, true, WeaponType.Sword,
                  new Attributes(
                      new Dictionary<string, double>()
                      {
                          { "Sta", 16 },
                          { "Crit", 16 },
                      }
                  ),
                  0, "Bonereaver's Edge", null, null, null)
        {
        }
    }
}

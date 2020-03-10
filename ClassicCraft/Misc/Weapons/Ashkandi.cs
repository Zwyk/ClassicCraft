using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Ashkandi : Weapon
    {
        public Ashkandi()
            : base(229, 344, 3.5, true, WeaponType.Sword,
                  new Attributes(
                      new Dictionary<string, double>()
                      {
                          { "Sta", 33 },
                          { "AP", 86 },
                      }
                  ),
                  0, "Ashkandi, Greatsword of the Brotherhood", null, null, null)
        {
        }
    }
}

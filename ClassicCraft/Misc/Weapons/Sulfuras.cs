using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Sulfuras : Weapon
    {
        public Sulfuras()
            : base(223, 372, 3.7, true, WeaponType.Mace,
                  new Attributes(
                      new Dictionary<string, double>()
                      {
                          { "Str", 12 },
                          { "Sta", 12 },
                      }
                  ),
                  0, "Sulfuras, Hand of Ragnaros", null, null, null)
        {
        }
    }
}

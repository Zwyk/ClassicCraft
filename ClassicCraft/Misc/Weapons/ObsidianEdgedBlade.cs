using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class ObsidianEdgedBlade : Weapon
    {
        public ObsidianEdgedBlade()
            : base(176, 264, 3.4, true, WeaponType.Sword,
                  new Attributes(
                      new Dictionary<string, double>()
                      {
                          { "Str", 42 },
                          { "Sword", 8 },
                      }
                  ),
                  0, "Obsidian Edged Blade", null, null, null)
        {
        }
    }
}

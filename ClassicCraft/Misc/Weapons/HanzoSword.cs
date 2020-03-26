using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class HanzoSword : Weapon
    {
        public HanzoSword()
            : base(38, 71, 1.5, false, WeaponType.Sword, null, 0, "Hanzo Sword", null, null, null)
        {
        }
    }
}

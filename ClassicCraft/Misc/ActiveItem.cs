using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class ActiveItem : Spell
    {
        public ActiveItem(Player p, double baseCD)
            : base(p, baseCD, 0,
                  new SpellData(SpellType.Magical, 0, false))
        {
        }
    }
}

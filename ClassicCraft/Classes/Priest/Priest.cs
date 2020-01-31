using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Priest : Player
    {
        public Priest(Player p)
            : base(p)
        {
        }

        public Priest(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Priest(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, Classes.Shaman, r, level, items, talents, buffs)
        {
        }
    }
}

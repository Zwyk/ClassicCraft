using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Warlock : Player
    {
        public Warlock(Player p)
            : base(p)
        {
        }

        public Warlock(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Warlock(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, Classes.Warlock, r, level, items, talents, buffs)
        {
        }
    }
}

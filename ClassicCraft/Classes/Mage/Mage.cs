using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Mage : Player
    {
        public Mage(Player p)
            : base(p)
        {
        }

        public Mage(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Mage(Simulation s, Races r, int level, Dictionary<Slot, Item> items, Dictionary<string, int> talents, List<Enchantment> buffs, bool tanking, bool facing, List<string> cooldowns, List<string> runes, string prepull)
            : base(s, Classes.Mage, r, level, items, talents, buffs, tanking, facing, cooldowns, runes, null, prepull)
        {
        }

        public override void Rota()
        {
            throw new NotImplementedException();
        }

        public static Dictionary<string, int> TalentsFromString(string ptal)
        {
            throw new NotImplementedException();
        }
    }
}

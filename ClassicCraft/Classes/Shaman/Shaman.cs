using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shaman : Player
    {
        public Shaman(Player p)
            : base(p)
        {
        }

        public Shaman(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Shaman(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null, bool tanking = false, bool facing = false, List<string> cooldowns = null, List<string> runes = null)
            : base(s, Classes.Shaman, r, level, items, talents, buffs, tanking, facing, cooldowns, runes)
        {
        }

        public override void Rota()
        {
            throw new NotImplementedException();
        }

        public override void SetupTalents(string ptal)
        {
            throw new NotImplementedException();
        }
    }
}

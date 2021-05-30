using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Paladin : Player
    {
        public Paladin(Player p)
            : base(p)
        {
        }

        public Paladin(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Paladin(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null, bool tanking = false, bool facing = false)
            : base(s, Classes.Paladin, r, level, items, talents, buffs, tanking, facing)
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

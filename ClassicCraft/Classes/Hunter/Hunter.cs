using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Hunter : Player
    {
        public Hunter(Player p)
            : base(p)
        {
        }

        public Hunter(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Hunter(Simulation s, Races r, int level, Dictionary<Slot, Item> items, Dictionary<string, int> talents, List<Enchantment> buffs, bool tanking, bool facing, List<string> cooldowns, List<string> runes, Entity pet, string prepull, double startResourcePct)
            : base(s, Classes.Hunter, r, level, items, talents, buffs, tanking, facing, cooldowns, runes, pet, prepull, startResourcePct)
        {
        }

        public override void Rota()
        {
            throw new NotImplementedException();
        }

        #region Talents

        public static Dictionary<string, int> TalentsFromString(string ptal)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

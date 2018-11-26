using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Item
    {
        public delegate void ItemEffect();

        public Player Player { get; set; }

        public Slot Slot { get; set; }

        public Attributes Attributes { get; set; }

        public ItemEffect SpecialEffect { get; set; }

        public Item(Player p, Slot slot, Attributes attributes = null, ItemEffect effect = null)
        {
            Player = p;
            Slot = slot;
            Attributes = new Attributes(attributes);
            SpecialEffect = effect;
        }
    }
}

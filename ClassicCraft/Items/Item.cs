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

        public int Id { get; set; }

        public string Name { get; set; }

        public Item(Player p = null, Slot slot = Slot.Any, Attributes attributes = null, int id = 0, string name = "New Item", ItemEffect effect = null)
        {
            Player = p;
            Slot = slot;
            Attributes = new Attributes(attributes);
            Id = id;
            Name = name;
            SpecialEffect = effect;
        }

        public override string ToString()
        {
            string attributes = "";
            foreach(Attribute a in Attributes.Values.Keys)
            {
                attributes += "[" + a + ":" + Attributes.Values[a] + "]";
            }

            return String.Format("[{0}] ({1}) {2} : {3}", Slot, Id, Name, attributes);
        }
    }
}

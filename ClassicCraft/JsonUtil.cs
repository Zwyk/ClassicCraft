using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JsonUtil
    {
        public class JsonItem
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Slot { get; set; }
            public Dictionary<string, double> Stats { get; set; }

            public JsonItem(int id = 0, string name = "New Item", string slot = "Any", Dictionary<string, double> attributes = null)
            {
                Name = name;
                Id = id;
                Slot = slot;
                Stats = attributes;
            }

            public static Item ToItem(JsonItem ji)
            {
                // TODO : Item Effect from ID/Name
                return new Item(null, SlotUtil.FromString(ji.Slot), new Attributes(ji.Stats), ji.Id, ji.Name, null);
            }

            public static JsonItem FromItem(Item i)
            {
                return new JsonItem(i.Id, i.Name, SlotUtil.ToString(i.Slot), Attributes.ToStringDic(i.Attributes));
            }
        }

        public class JsonPlayer
        {
            // TODO LATER Spells level, Level
            // TODO Race, Class, Talents
        }
    }
}

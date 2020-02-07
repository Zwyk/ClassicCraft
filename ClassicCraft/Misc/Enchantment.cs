using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Enchantment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Attributes Attributes { get; set; }

        public Enchantment(int id = 0, string name = "Enchantment", Attributes attributes = null)
        {
            Id = id;
            Name = name;
            Attributes = attributes;
        }

        public static Enchantment ToEnchantment(JsonUtil.JsonEnchantment json)
        {
            if (json == null) return null;
            return new Enchantment(json.Id, json.Name, new Attributes(json.Stats));
        }

        public static JsonUtil.JsonEnchantment FromEnchantment(Enchantment enchant)
        {
            if (enchant == null) return null;
            return new JsonUtil.JsonEnchantment(enchant.Id, enchant.Name, Attributes.ToStringDic(enchant.Attributes));
        }
    }
}

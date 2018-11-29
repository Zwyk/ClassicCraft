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
            public JsonEnchantment Enchantment { get; set; }

            public JsonItem(int id = 0, string name = "New Item", string slot = "Any", Dictionary<string, double> attributes = null, JsonEnchantment enchantment = null)
            {
                Name = name;
                Id = id;
                Slot = slot;
                Stats = attributes;
                Enchantment = enchantment;
            }

            public static Item ToItem(JsonItem ji)
            {
                // TODO : Item Effect from ID/Name
                if (ji == null) return null;
                else
                {
                    Item res = new Item(null, SlotUtil.FromString(ji.Slot), new Attributes(ji.Stats), ji.Id, ji.Name, ClassicCraft.Enchantment.ToEnchantment(ji.Enchantment), null);
                    res.Attributes.SetValue(Attribute.CritChance, res.Attributes.GetValue(Attribute.CritChance) / 100);
                    res.Attributes.SetValue(Attribute.HitChance, res.Attributes.GetValue(Attribute.HitChance) / 100);
                    res.Attributes.SetValue(Attribute.AS, res.Attributes.GetValue(Attribute.AS) / 100);
                    return res;
                }
            }

            public static JsonItem FromItem(Item i)
            {
                if (i == null) return null;
                else
                {
                    JsonItem res = new JsonItem(i.Id, i.Name, SlotUtil.ToString(i.Slot), Attributes.ToStringDic(i.Attributes), ClassicCraft.Enchantment.FromEnchantment(i.Enchantment));
                    if (res.Stats.ContainsKey("CritChance")) res.Stats["CritChance"] *= 100;
                    if (res.Stats.ContainsKey("HitChance")) res.Stats["HitChance"] *= 100;
                    if (res.Stats.ContainsKey("AS")) res.Stats["AS"] *= 100;
                    return res;
                }
            }
        }

        public class JsonWeapon : JsonItem
        {
            public int DamageMin { get; set; }
            public int DamageMax { get; set; }
            public double Speed { get; set; }
            public bool TwoHanded { get; set; }
            public string Type { get; set; }

            public JsonWeapon(int damageMin = 1, int damageMax = 2, double speed = 1, bool twoHanded = true, string type = "Axe", int id = 0, string name = "New Item", Dictionary<string, double> attributes = null, JsonEnchantment enchantment = null)
                : base(id, name, "Weapon", attributes, enchantment)
            {
                DamageMin = damageMin;
                DamageMax = damageMax;
                Speed = speed;
                TwoHanded = twoHanded;
                Type = type;
            }

            public static Weapon ToWeapon(JsonWeapon jw)
            {
                // TODO : Item Effect from ID/Name
                if (jw == null) return null;
                else
                {
                    Weapon res = new Weapon(null, jw.DamageMin, jw.DamageMax, jw.Speed, jw.TwoHanded, Weapon.StringToType(jw.Type), new Attributes(jw.Stats), jw.Id, jw.Name, ClassicCraft.Enchantment.ToEnchantment(jw.Enchantment), null);
                    res.Attributes.SetValue(Attribute.CritChance, res.Attributes.GetValue(Attribute.CritChance) / 100);
                    res.Attributes.SetValue(Attribute.HitChance, res.Attributes.GetValue(Attribute.HitChance) / 100);
                    res.Attributes.SetValue(Attribute.AS, res.Attributes.GetValue(Attribute.AS) / 100);
                    return res;
                }
            }

            public static JsonWeapon FromWeapon(Weapon w)
            {
                if (w == null) return null;
                else
                {
                    JsonWeapon res = new JsonWeapon(w.DamageMin, w.DamageMax, w.Speed, w.TwoHanded, Weapon.TypeToString(w.Type), w.Id, w.Name, Attributes.ToStringDic(w.Attributes), ClassicCraft.Enchantment.FromEnchantment(w.Enchantment));
                    if (res.Stats.ContainsKey("CritChance")) res.Stats["CritChance"] *= 100;
                    if (res.Stats.ContainsKey("HitChance")) res.Stats["HitChance"] *= 100;
                    if (res.Stats.ContainsKey("AS")) res.Stats["AS"] *= 100;
                    return res;
                }
            }
        }

        public static Dictionary<Player.Slot, Item> ToEquipment(JsonWeapon mh, JsonWeapon oh, JsonWeapon ranged, Dictionary<string, JsonItem> je)
        {
            Dictionary<Player.Slot, Item> res = new Dictionary<Player.Slot, Item>();

            foreach (Player.Slot slot in (Player.Slot[])Enum.GetValues(typeof(Player.Slot)))
            {
                if (slot == Player.Slot.MH)
                {
                    res.Add(slot, JsonWeapon.ToWeapon(mh));
                }
                else if (slot == Player.Slot.OH)
                {
                    res.Add(slot, JsonWeapon.ToWeapon(oh));
                }
                else if (slot == Player.Slot.Ranged)
                {
                    res.Add(slot, JsonWeapon.ToWeapon(ranged));
                }
                else
                {
                    Item i = null;
                    string s = Player.FromSlot(slot);

                    if (je.ContainsKey(s))
                    {
                        i = JsonItem.ToItem(je[s]);
                    }

                    res.Add(slot, JsonItem.ToItem(je[Player.FromSlot(slot)]));
                }
            }

            return res;
        }

        public static Dictionary<string, JsonItem> FromEquipement(Dictionary<Player.Slot, Item> eq)
        {
            Dictionary<string, JsonItem> res = new Dictionary<string, JsonItem>();

            foreach (Player.Slot s in eq.Keys)
            {
                if(s != Player.Slot.MH && s != Player.Slot.OH && s != Player.Slot.Ranged)
                {
                    res.Add(Player.FromSlot(s), JsonItem.FromItem(eq[s]));
                }
            }

            return res;
        }

        public class JsonPlayer
        {
            // TODO LATER Spells level

            public string Class { get; set; }
            public int Level { get; set; }
            public string Race { get; set; }
            public string Talents { get; set; }
            public JsonWeapon MH { get; set; }
            public JsonWeapon OH { get; set; }
            public JsonWeapon Ranged { get; set; }
            public Dictionary<string, JsonItem> Equipment { get; set; }

            public JsonPlayer(JsonWeapon mh = null, JsonWeapon oh = null, JsonWeapon ranged = null, Dictionary<string, JsonItem> equipment = null, string @class = "Warrior", int level = 60, string race = "Orc", string talents = "")
            {
                MH = mh;
                OH = oh;
                Ranged = ranged;
                Class = @class;
                Level = level;
                Race = race;
                Talents = talents;
                Equipment = equipment;
            }
        }

        public class JsonBoss
        {
            public int Level { get; set; }
            public int Armor { get; set; }

            public JsonBoss(int level = 63, int armor = 4400)
            {
                Level = level;
                Armor = armor;
            }
        }

        public class JsonSim
        {
            public double FightLength { get; set; }
            public double FightLengthMod { get; set; }
            public int NbSim { get; set; }
            public double TargetErrorPct { get; set; }
            public bool TargetError { get; set; }
            public bool LogFight { get; set; }
            public List<string> SimStatWeight { get; set; }
            public JsonBoss Boss { get; set; }
            public JsonPlayer Player { get; set; }

            public JsonSim(JsonPlayer player = null, JsonBoss boss = null, double fightLength = 300, double fightLengthMod = 0.2, int nbSim = 1000, double targetErrorPct = 0.5, bool targetError = true, bool logFight = false, List<string> simStatWeight = null)
            {
                Player = player;
                Boss = boss;
                FightLength = fightLength;
                FightLengthMod = fightLengthMod;
                NbSim = nbSim;
                TargetErrorPct = targetErrorPct;
                TargetError = targetError;
                LogFight = logFight;
                SimStatWeight = simStatWeight;
            }
        }

        public static List<Attribute> ToAttributes(List<string> attributes)
        {
            if (attributes == null) return null;

            List<Attribute> res = new List<Attribute>();
            foreach(string s in attributes)
            {
                res.Add(AttributeUtil.FromString(s));
            }
            return res;
        }

        public class JsonEnchantment
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Dictionary<string, double> Stats { get; set; }

            public JsonEnchantment(int id = 0, string name = "Enchantment", Dictionary<string, double> stats = null)
            {
                Id = id;
                Name = name;
                Stats = stats;
            }
        }
    }
}

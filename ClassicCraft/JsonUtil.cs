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
                if (ji == null) return null;
                else return new Item(null, SlotUtil.FromString(ji.Slot), new Attributes(ji.Stats), ji.Id, ji.Name, null);
            }

            public static JsonItem FromItem(Item i)
            {
                if (i == null) return null;
                else return new JsonItem(i.Id, i.Name, SlotUtil.ToString(i.Slot), Attributes.ToStringDic(i.Attributes));
            }
        }

        public class JsonWeapon : JsonItem
        {
            public int DamageMin { get; set; }
            public int DamageMax { get; set; }
            public double Speed { get; set; }
            public bool TwoHanded { get; set; }
            public string Type { get; set; }

            public JsonWeapon(int damageMin = 1, int damageMax = 2, double speed = 1, bool twoHanded = true, string type = "Axe", int id = 0, string name = "New Item", Dictionary<string, double> attributes = null)
                : base(id, name, "Weapon", attributes)
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
                else return new Weapon(null, jw.DamageMin, jw.DamageMax, jw.Speed, jw.TwoHanded, Weapon.StringToType(jw.Type), new Attributes(jw.Stats), jw.Id, jw.Name, null);
            }

            public static JsonWeapon FromWeapon(Weapon w)
            {
                if (w == null) return null;
                else return new JsonWeapon(w.DamageMin, w.DamageMax, w.Speed, w.TwoHanded, Weapon.TypeToString(w.Type), w.Id, w.Name, Attributes.ToStringDic(w.Attributes));
            }
        }

        public static Dictionary<Player.Slot, Item> ToEquipment(JsonWeapon mh, JsonWeapon oh, Dictionary<string, JsonItem> je)
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
                if(s != Player.Slot.MH && s != Player.Slot.OH)
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
            public Dictionary<string, JsonItem> Equipment { get; set; }

            public JsonPlayer(JsonWeapon mh = null, JsonWeapon oh = null, Dictionary<string, JsonItem> equipment = null, string @class = "Warrior", int level = 60, string race = "Orc", string talents = "")
            {
                MH = mh;
                OH = oh;
                Class = @class;
                Level = level;
                Race = race;
                Talents = talents;
                Equipment = equipment;
            }
        }

        public class JsonSim
        {
            public double FightLength { get; set; }
            public int NbSim { get; set; }
            public double TargetErrorPct { get; set; }
            public bool TargetError { get; set; }
            public bool LogFight { get; set; }
            public JsonPlayer Player { get; set; }

            public JsonSim(JsonPlayer player = null, double fightLength = 300, int nbSim = 1000, double targetErrorPct = 0.5, bool targetError = true, bool logFight = false)
            {
                Player = player;
                FightLength = fightLength;
                NbSim = nbSim;
                TargetErrorPct = targetErrorPct;
                TargetError = targetError;
                LogFight = logFight;
            }
        }
    }
}

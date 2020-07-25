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

                if (Stats == null) Stats = new Dictionary<string, double>();
                if (Enchantment == null) Enchantment = new JsonEnchantment();
            }

            public static Item ToItem(JsonItem ji)
            {
                // TODO : Item Effect from ID/Name
                if (ji == null) return null;
                else
                {
                    Item res = new Item(SlotUtil.FromString(ji.Slot), new Attributes(ji.Stats), ji.Id, ji.Name, ClassicCraft.Enchantment.ToEnchantment(ji.Enchantment), null);
                    if(res.Attributes != null)
                    {
                        res.Attributes.SetValue(Attribute.CritChance, res.Attributes.GetValue(Attribute.CritChance) / 100);
                        res.Attributes.SetValue(Attribute.HitChance, res.Attributes.GetValue(Attribute.HitChance) / 100);
                        res.Attributes.SetValue(Attribute.Haste, res.Attributes.GetValue(Attribute.Haste) / 100);
                        res.Attributes.SetValue(Attribute.SpellHitChance, res.Attributes.GetValue(Attribute.SpellHitChance) / 100);
                        res.Attributes.SetValue(Attribute.SpellCritChance, res.Attributes.GetValue(Attribute.SpellCritChance) / 100);
                    }
                    if (res.Enchantment != null)
                    {
                        res.Enchantment.Attributes.SetValue(Attribute.CritChance, res.Enchantment.Attributes.GetValue(Attribute.CritChance) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.HitChance, res.Enchantment.Attributes.GetValue(Attribute.HitChance) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.Haste, res.Enchantment.Attributes.GetValue(Attribute.Haste) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.SpellHitChance, res.Enchantment.Attributes.GetValue(Attribute.SpellHitChance) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.SpellCritChance, res.Enchantment.Attributes.GetValue(Attribute.SpellCritChance) / 100);
                    }
                    return res;
                }
            }

            public static JsonItem FromItem(Item i)
            {
                if (i == null) return null;
                else
                {
                    JsonItem res = new JsonItem(i.Id, i.Name, SlotUtil.ToString(i.Slot), Attributes.ToStringDic(i.Attributes), ClassicCraft.Enchantment.FromEnchantment(i.Enchantment));
                    if(res.Stats != null)
                    {
                        if (res.Stats.ContainsKey("Crit")) res.Stats["Crit"] *= 100;
                        if (res.Stats.ContainsKey("Hit")) res.Stats["Hit"] *= 100;
                        if (res.Stats.ContainsKey("AS")) res.Stats["AS"] *= 100;
                        if (res.Stats.ContainsKey("SpellHit")) res.Stats["SpellHit"] *= 100;
                        if (res.Stats.ContainsKey("SpellCrit")) res.Stats["SpellCrit"] *= 100;
                    }
                    if(res.Enchantment != null)
                    {
                        if (res.Enchantment.Stats.ContainsKey("Crit")) res.Enchantment.Stats["Crit"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("Hit")) res.Enchantment.Stats["Hit"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("AS")) res.Enchantment.Stats["AS"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("SpellHit")) res.Enchantment.Stats["SpellHit"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("SpellCrit")) res.Enchantment.Stats["SpellCrit"] *= 100;
                    }
                    return res;
                }
            }
        }

        public class JsonWeapon : JsonItem
        {
            public double DamageMin { get; set; }
            public double DamageMax { get; set; }
            public double Speed { get; set; }
            public bool TwoHanded { get; set; }
            public string Type { get; set; }
            public JsonEnchantment Buff { get; set; }
            public string School { get; set; }

            public JsonWeapon(double damageMin = 0, double damageMax = 0, double speed = 0, bool twoHanded = true, string type = "Axe", int id = 0, string name = "New Item", Dictionary<string, double> attributes = null, JsonEnchantment enchantment = null, JsonEnchantment buffs = null, string school = "Physical")
                : base(id, name, IsRangedWeapon(type) ? "Ranged" : "Weapon", attributes, enchantment)
            {
                DamageMin = damageMin;
                DamageMax = damageMax;
                Speed = speed;
                TwoHanded = twoHanded;
                Type = type;
                Buff = buffs;
                School = school;
            }

            public static bool IsRangedWeapon(string type)
            {
                return type == "Bow" || type == "Crossbow" || type == "Gun" || type == "Wand";
            }

            public static Weapon ToWeapon(JsonWeapon jw)
            {
                // TODO : Item Effect from ID/Name
                if (jw == null) return null;
                else
                {
                    Weapon res = new Weapon(jw.DamageMin, jw.DamageMax, jw.Speed, jw.TwoHanded, Weapon.StringToType(jw.Type), new Attributes(jw.Stats), jw.Id, jw.Name, ClassicCraft.Enchantment.ToEnchantment(jw.Enchantment), ClassicCraft.Enchantment.ToEnchantment(jw.Buff), null, SchoolFromString(jw.School));
                    if(res.Attributes != null)
                    {
                        res.Attributes.SetValue(Attribute.CritChance, res.Attributes.GetValue(Attribute.CritChance) / 100);
                        res.Attributes.SetValue(Attribute.HitChance, res.Attributes.GetValue(Attribute.HitChance) / 100);
                        res.Attributes.SetValue(Attribute.Haste, res.Attributes.GetValue(Attribute.Haste) / 100);
                        res.Attributes.SetValue(Attribute.SpellHitChance, res.Attributes.GetValue(Attribute.SpellHitChance) / 100);
                        res.Attributes.SetValue(Attribute.SpellCritChance, res.Attributes.GetValue(Attribute.SpellCritChance) / 100);
                    }
                    if(res.Enchantment != null)
                    {
                        res.Enchantment.Attributes.SetValue(Attribute.CritChance, res.Enchantment.Attributes.GetValue(Attribute.CritChance) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.HitChance, res.Enchantment.Attributes.GetValue(Attribute.HitChance) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.Haste, res.Enchantment.Attributes.GetValue(Attribute.Haste) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.SpellHitChance, res.Enchantment.Attributes.GetValue(Attribute.SpellHitChance) / 100);
                        res.Enchantment.Attributes.SetValue(Attribute.SpellCritChance, res.Enchantment.Attributes.GetValue(Attribute.SpellCritChance) / 100);
                    }
                    if(res.Buff != null)
                    {
                        res.Buff.Attributes.SetValue(Attribute.CritChance, res.Buff.Attributes.GetValue(Attribute.CritChance) / 100);
                        res.Buff.Attributes.SetValue(Attribute.HitChance, res.Buff.Attributes.GetValue(Attribute.HitChance) / 100);
                        res.Buff.Attributes.SetValue(Attribute.Haste, res.Buff.Attributes.GetValue(Attribute.Haste) / 100);
                        res.Buff.Attributes.SetValue(Attribute.SpellHitChance, res.Buff.Attributes.GetValue(Attribute.SpellHitChance) / 100);
                        res.Buff.Attributes.SetValue(Attribute.SpellCritChance, res.Buff.Attributes.GetValue(Attribute.SpellCritChance) / 100);
                    }
                    return res;
                }
            }

            public static JsonWeapon FromWeapon(Weapon w)
            {
                if (w == null) return null;
                else
                {
                    JsonWeapon res = new JsonWeapon(w.DamageMin, w.DamageMax, w.Speed, w.TwoHanded, Weapon.TypeToString(w.Type), w.Id, w.Name, Attributes.ToStringDic(w.Attributes), ClassicCraft.Enchantment.FromEnchantment(w.Enchantment), ClassicCraft.Enchantment.FromEnchantment(w.Buff), SchoolToString(w.School));
                    if(res.Stats != null)
                    {
                        if (res.Stats.ContainsKey("Crit")) res.Stats["Crit"] *= 100;
                        if (res.Stats.ContainsKey("Hit")) res.Stats["Hit"] *= 100;
                        if (res.Stats.ContainsKey("AS")) res.Stats["AS"] *= 100;
                        if (res.Stats.ContainsKey("SpellHit")) res.Stats["SpellHit"] *= 100;
                        if (res.Stats.ContainsKey("SpellCrit")) res.Stats["SpellCrit"] *= 100;
                    }
                    if(res.Enchantment != null)
                    {
                        if (res.Enchantment.Stats.ContainsKey("Crit")) res.Enchantment.Stats["Crit"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("Hit")) res.Enchantment.Stats["Hit"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("AS")) res.Enchantment.Stats["AS"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("SpellHit")) res.Enchantment.Stats["SpellHit"] *= 100;
                        if (res.Enchantment.Stats.ContainsKey("SpellCrit")) res.Enchantment.Stats["SpellCrit"] *= 100;
                    }
                    if(res.Buff != null)
                    {
                        if (res.Buff.Stats.ContainsKey("Crit")) res.Buff.Stats["Crit"] *= 100;
                        if (res.Buff.Stats.ContainsKey("Hit")) res.Buff.Stats["Hit"] *= 100;
                        if (res.Buff.Stats.ContainsKey("AS")) res.Buff.Stats["AS"] *= 100;
                        if (res.Buff.Stats.ContainsKey("SpellHit")) res.Buff.Stats["SpellHit"] *= 100;
                        if (res.Buff.Stats.ContainsKey("SpellCrit")) res.Buff.Stats["SpellCrit"] *= 100;
                    }
                    return res;
                }
            }
        }

        public static Dictionary<Player.Slot, Item> ToEquipment(Dictionary<string, JsonWeapon> jw, Dictionary<string, JsonItem> je)
        {
            Dictionary<Player.Slot, Item> res = new Dictionary<Player.Slot, Item>();

            foreach (Player.Slot slot in (Player.Slot[])Enum.GetValues(typeof(Player.Slot)))
            {
                if (slot == Player.Slot.MH || slot == Player.Slot.OH || slot == Player.Slot.Ranged)
                {
                    string s = Player.FromSlot(slot);
                    res.Add(slot, jw.ContainsKey(s) ? JsonWeapon.ToWeapon(jw[s]) : null);
                }
                else
                {
                    string s = Player.FromSlot(slot);
                    res.Add(slot, je.ContainsKey(s) ? JsonItem.ToItem(je[s]) : null);
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
            public Dictionary<string, bool> Cooldowns { get; set; }
            public List<JsonEnchantment> Buffs { get; set; }
            public Dictionary<string, JsonWeapon> Weapons { get; set; }
            public Dictionary<string, JsonItem> Equipment { get; set; }

            public JsonPlayer(Dictionary<string, JsonWeapon> weapons = null, Dictionary<string, JsonItem> equipment = null, string @class = "Warrior", int level = 60, string race = "Orc", string talents = "", List<JsonEnchantment> buffs = null, Dictionary<string, bool> cooldowns = null)
            {
                Class = @class;
                Level = level;
                Race = race;
                Talents = talents;
                Weapons = weapons;
                Equipment = equipment;
                Buffs = buffs;
                Cooldowns = cooldowns;
            }

            public static Player ToPlayer(JsonPlayer jp, bool tanking = false)
            {
                List<Enchantment> buffs = new List<Enchantment>();
                if(jp.Buffs != null)
                {
                    foreach (JsonEnchantment je in jp.Buffs.Where(v => v != null))
                    {
                        Enchantment e = Enchantment.ToEnchantment(je);
                        if (e != null)
                        {
                            if (e.Attributes != null)
                            {
                                e.Attributes.SetValue(Attribute.CritChance, e.Attributes.GetValue(Attribute.CritChance) / 100);
                                e.Attributes.SetValue(Attribute.HitChance, e.Attributes.GetValue(Attribute.HitChance) / 100);
                                e.Attributes.SetValue(Attribute.Haste, e.Attributes.GetValue(Attribute.Haste) / 100);
                                e.Attributes.SetValue(Attribute.SpellHitChance, e.Attributes.GetValue(Attribute.SpellHitChance) / 100);
                                e.Attributes.SetValue(Attribute.SpellCritChance, e.Attributes.GetValue(Attribute.SpellCritChance) / 100);
                            }

                            buffs.Add(e);
                        }
                    }
                }

                bool windfurytotem = jp.Buffs != null && jp.Buffs.Any(b => b.Name.ToLower().Contains("windfury totem"));
                List<string> cooldowns = jp.Cooldowns.Where(v => v.Value == true).Select(c => c.Key).ToList();
                
                switch(ToClass(jp.Class))
                {
                    case Player.Classes.Druid:
                        return new Druid(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    case Player.Classes.Hunter:
                        return new Hunter(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    case Player.Classes.Paladin:
                        return new Paladin(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    case Player.Classes.Priest:
                        return new Priest(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    case Player.Classes.Rogue:
                        return new Rogue(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    case Player.Classes.Shaman:
                        return new Shaman(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    case Player.Classes.Warlock:
                        return new Warlock(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    case Player.Classes.Warrior:
                        return new Warrior(null, ToRace(jp.Race), 60, ToEquipment(jp.Weapons, jp.Equipment), null, buffs, tanking)
                        {
                            WindfuryTotem = windfurytotem,
                            Cooldowns = cooldowns
                        };
                    default:
                        throw new NotImplementedException("This class isn't supported yet : " + ToClass(jp.Class));
                }
            }

            public static Player.Races ToRace(string s)
            {
                switch (s)
                {
                    case "Dwarf": return Player.Races.Dwarf;
                    case "Gnome": return Player.Races.Gnome;
                    case "Human": return Player.Races.Human;
                    case "Night Elf": return Player.Races.NightElf;
                    case "Orc": return Player.Races.Orc;
                    case "Troll": return Player.Races.Troll;
                    case "Tauren": return Player.Races.Tauren;
                    case "Undead": return Player.Races.Undead;
                    default: throw new Exception("Race not found : " + s);
                }
            }

            public static Player.Classes ToClass(string s)
            {
                switch (s)
                {
                    case "Druid": return Player.Classes.Druid;
                    case "Hunter": return Player.Classes.Hunter;
                    case "Mage": return Player.Classes.Mage;
                    case "Paladin": return Player.Classes.Paladin;
                    case "Priest": return Player.Classes.Priest;
                    case "Rogue": return Player.Classes.Rogue;
                    case "Shaman": return Player.Classes.Shaman ;
                    case "Warlock": return Player.Classes.Warlock;
                    case "Warrior": return Player.Classes.Warrior;
                    default: throw new Exception("Class not found : " + s);
                }
            }
        }

        public class JsonBoss
        {
            public static School StringToSchool(string s)
            {
                switch (s)
                {
                    case "Physical": return School.Physical;
                    case "Magical": return School.Magical;
                    case "All": return School.Magical;
                    case "Any": return School.Magical;
                    case "Fire": return School.Fire;
                    case "Ice": return School.Frost;
                    case "Shadow": return School.Shadow;
                    case "Light": return School.Light;
                    case "Arcane": return School.Arcane;
                    case "Nature": return School.Nature;
                    default: throw new NotImplementedException("School type unknown : " + s);
                }
            }

            public int Level { get; set; }
            public string Type { get; set; }
            public int Armor { get; set; }
            public List<string> Debuffs { get; set; }
            public Dictionary<string, int> SchoolResists { get; set; }

            public JsonBoss(int level = 63, string type = "Humanoid", int armor = 4400, Dictionary<string, int> schoolResists = null, List<string> debuffs = null)
            {
                Level = level;
                Type = type;
                Armor = armor;
                SchoolResists = schoolResists;
                Debuffs = debuffs;
            }

            public static Boss ToBoss(JsonBoss jb, double armorPen = 0)
            {
                int armor = jb.Armor - (int)Math.Round(armorPen);

                Dictionary<School, int> magicResist = null;
                if (jb.SchoolResists != null)
                {
                    magicResist = new Dictionary<School, int>();
                    foreach (string sr in jb.SchoolResists.Keys)
                    {
                        magicResist.Add(StringToSchool(sr), jb.SchoolResists[sr]);
                    }
                }

                List<string> debuffsList = jb.Debuffs;
                Dictionary<string, Effect> debuffs = null;

                if (debuffsList != null && debuffsList.Count > 0)
                {
                    armor -= (debuffsList.Any(d => d.ToLower().Contains("expose armor")) ? 2550 : (debuffsList.Any(d => d.ToLower().Contains("sunder armor")) ? 2250 : 0))
                        + (debuffsList.Any(d => d.ToLower().Contains("curse of recklessness")) ? 640 : 0)
                        + (debuffsList.Any(d => d.ToLower().Contains("faerie fire")) ? 505 : 0)
                        + (debuffsList.Any(d => d.ToLower().Contains("annihilator")) ? 600 : 0);

                    debuffs = new Dictionary<string, Effect>();
                    foreach (string s in debuffsList)
                    {
                        debuffs.Add(s, new CustomEffect(null, null, s, false, -1));
                    }
                }


                return new Boss(ToType(jb.Type), jb.Level, Math.Max(0, armor), magicResist, debuffs);
            }

            public static Entity.MobType ToType(string s)
            {
                switch(s)
                {
                    case "Humanoid": return Entity.MobType.Humanoid;
                    case "Dragonkin": return Entity.MobType.Dragonkin;
                    case "Beast": return Entity.MobType.Beast;
                    case "Giant": return Entity.MobType.Giant;
                    case "Undead": return Entity.MobType.Undead;
                    default: return Entity.MobType.Other;
                }
            }
        }

        public class JsonSim
        {
            public double FightLength { get; set; }
            public double FightLengthMod { get; set; }
            public int NbSim { get; set; }
            public bool StatsWeights { get; set; }
            public double TargetErrorPct { get; set; }
            public bool TargetError { get; set; }
            public bool LogFight { get; set; }
            public bool BossAutoLife { get; set; }
            public double BossLowLifeTime { get; set; }
            public JsonBoss Boss { get; set; }
            public bool UnlimitedMana { get; set; }
            public bool UnlimitedResource { get; set; }
            public bool Tanking { get; set; }
            public double TankHitEvery { get; set; }
            public double TankHitRage { get; set; }

            public JsonSim(JsonBoss boss = null, double fightLength = 300, double fightLengthMod = 0.2, int nbSim = 1000, double targetErrorPct = 0.5, bool targetError = true, bool logFight = false, bool statsWeights = false, bool bossAutoLife = true, double bossLowLifeTime = 0, bool unlimitedMana = false, bool unlimitedResource = false, bool tanking = false, double tankHitEvery = 1, double tankHitRage = 25)
            {
                Boss = boss;
                FightLength = fightLength;
                FightLengthMod = fightLengthMod;
                NbSim = nbSim;
                TargetErrorPct = targetErrorPct;
                TargetError = targetError;
                LogFight = logFight;
                StatsWeights = statsWeights;
                BossAutoLife = bossAutoLife;
                BossLowLifeTime = bossLowLifeTime;
                UnlimitedMana = unlimitedMana;
                UnlimitedResource = unlimitedResource;
                Tanking = tanking;
                TankHitEvery = tankHitEvery;
                TankHitRage = tankHitRage;
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
            public string Slot { get; set; }
            public Dictionary<string, double> Stats { get; set; }

            public JsonEnchantment(int id = 0, string name = "Enchantment", string slot = "Any", Dictionary<string, double> stats = null)
            {
                Id = id;
                Name = name;
                Slot = slot;
                Stats = stats;

                if (Stats == null) Stats = new Dictionary<string, double>();
            }
        }

        public static School SchoolFromString(string s)
        {
            switch (s)
            {
                case "Magical": return School.Magical;
                case "Fire": return School.Fire;
                case "Arcane": return School.Arcane;
                case "Frost": return School.Frost;
                case "Shadow": return School.Shadow;
                case "Nature": return School.Nature;
                case "Light": return School.Light;
                default: return School.Physical;
            }
        }

        public static string SchoolToString(School s)
        {
            switch (s)
            {
                case School.Magical: return  "Magical";
                case School.Fire: return "Fire";
                case School.Arcane: return "Arcane";
                case School.Frost: return "Frost";
                case School.Shadow: return "Shadow";
                case School.Nature: return "Nature";
                case School.Light: return  "Light";
                default: return "Physical";
            }
        }
    }
}

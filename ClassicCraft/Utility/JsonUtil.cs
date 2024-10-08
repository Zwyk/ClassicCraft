﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            public JsonEnchantment Gems { get; set; }

            public JsonItem(int id = 0, string name = "New Item", string slot = "Any", Dictionary<string, double> attributes = null, JsonEnchantment enchantment = null, JsonEnchantment gems = null)
            {
                Name = name;
                Id = id;
                Slot = slot;
                Stats = attributes;
                Enchantment = enchantment;
                Gems = gems;

                if (Stats == null) Stats = new Dictionary<string, double>();
            }

            public static Item ToItem(JsonItem ji)
            {
                if (ji == null) return null;
                else
                {
                    Item res = new Item(SlotUtil.FromString(ji.Slot), new Attributes(ji.Stats), ji.Id, ji.Name, ClassicCraft.Enchantment.ToEnchantment(ji.Enchantment, ji.Gems), null);
                    if(res.Attributes != null)
                    {
                        res.Attributes.SetValue(Attribute.CritChance, res.Attributes.GetValue(Attribute.CritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1));
                        res.Attributes.SetValue(Attribute.HitChance, res.Attributes.GetValue(Attribute.HitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1));
                        res.Attributes.SetValue(Attribute.Haste, res.Attributes.GetValue(Attribute.Haste) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1));
                        res.Attributes.SetValue(Attribute.SpellHitChance, res.Attributes.GetValue(Attribute.SpellHitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                        res.Attributes.SetValue(Attribute.SpellCritChance, res.Attributes.GetValue(Attribute.SpellCritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                        res.Attributes.SetValue(Attribute.Expertise, res.Attributes.GetValue(Attribute.Expertise) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1));
                    }
                    if (res.Enchantment != null)
                    {
                        res.Enchantment.Attributes.SetValue(Attribute.CritChance, res.Enchantment.Attributes.GetValue(Attribute.CritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.HitChance, res.Enchantment.Attributes.GetValue(Attribute.HitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.Haste, res.Enchantment.Attributes.GetValue(Attribute.Haste) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.SpellHitChance, res.Enchantment.Attributes.GetValue(Attribute.SpellHitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.SpellCritChance, res.Enchantment.Attributes.GetValue(Attribute.SpellCritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.Expertise, res.Enchantment.Attributes.GetValue(Attribute.Expertise) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1));
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
                        if (res.Stats.ContainsKey("Crit")) res.Stats["Crit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);
                        if (res.Stats.ContainsKey("Hit")) res.Stats["Hit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1);
                        if (res.Stats.ContainsKey("Haste")) res.Stats["Haste"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1);
                        if (res.Stats.ContainsKey("SpellHit")) res.Stats["SpellHit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1);
                        if (res.Stats.ContainsKey("SpellCrit")) res.Stats["SpellCrit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1);
                        if (res.Stats.ContainsKey("Expertise")) res.Stats["Expertise"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1);
                    }
                    if(res.Enchantment != null)
                    {
                        if (res.Enchantment.Stats.ContainsKey("Crit")) res.Enchantment.Stats["Crit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("Hit")) res.Enchantment.Stats["Hit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("Haste")) res.Enchantment.Stats["Haste"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1);
                        if (res.Enchantment.Stats.ContainsKey("SpellHit")) res.Enchantment.Stats["SpellHit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("SpellCrit")) res.Enchantment.Stats["SpellCrit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("Expertise")) res.Enchantment.Stats["Expertise"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1);
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
                if (jw == null) return null;
                else
                {
                    Weapon res = new Weapon(jw.DamageMin, jw.DamageMax, jw.Speed, jw.TwoHanded, Weapon.StringToType(jw.Type), new Attributes(jw.Stats), jw.Id, jw.Name, ClassicCraft.Enchantment.ToEnchantment(jw.Enchantment, jw.Gems), ClassicCraft.Enchantment.ToEnchantment(jw.Buff), null, SchoolFromString(jw.School));
                    if(res.Attributes != null)
                    {
                        res.Attributes.SetValue(Attribute.CritChance, res.Attributes.GetValue(Attribute.CritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1));
                        res.Attributes.SetValue(Attribute.HitChance, res.Attributes.GetValue(Attribute.HitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1));
                        res.Attributes.SetValue(Attribute.Haste, res.Attributes.GetValue(Attribute.Haste) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1));
                        res.Attributes.SetValue(Attribute.SpellHitChance, res.Attributes.GetValue(Attribute.SpellHitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                        res.Attributes.SetValue(Attribute.SpellCritChance, res.Attributes.GetValue(Attribute.SpellCritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                        res.Attributes.SetValue(Attribute.Expertise, res.Attributes.GetValue(Attribute.Expertise) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1));
                    }
                    if(res.Enchantment != null)
                    {
                        res.Enchantment.Attributes.SetValue(Attribute.CritChance, res.Enchantment.Attributes.GetValue(Attribute.CritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.HitChance, res.Enchantment.Attributes.GetValue(Attribute.HitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.Haste, res.Enchantment.Attributes.GetValue(Attribute.Haste) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.SpellHitChance, res.Enchantment.Attributes.GetValue(Attribute.SpellHitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.SpellCritChance, res.Enchantment.Attributes.GetValue(Attribute.SpellCritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                        res.Enchantment.Attributes.SetValue(Attribute.Expertise, res.Enchantment.Attributes.GetValue(Attribute.Expertise) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1));
                    }
                    if(res.Buff != null)
                    {
                        res.Buff.Attributes.SetValue(Attribute.CritChance, res.Buff.Attributes.GetValue(Attribute.CritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1));
                        res.Buff.Attributes.SetValue(Attribute.HitChance, res.Buff.Attributes.GetValue(Attribute.HitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1));
                        res.Buff.Attributes.SetValue(Attribute.Haste, res.Buff.Attributes.GetValue(Attribute.Haste) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1));
                        res.Buff.Attributes.SetValue(Attribute.SpellHitChance, res.Buff.Attributes.GetValue(Attribute.SpellHitChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                        res.Buff.Attributes.SetValue(Attribute.SpellCritChance, res.Buff.Attributes.GetValue(Attribute.SpellCritChance) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                        res.Buff.Attributes.SetValue(Attribute.Expertise, res.Buff.Attributes.GetValue(Attribute.Expertise) / 100 / (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1));
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
                        if (res.Stats.ContainsKey("Crit")) res.Stats["Crit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);
                        if (res.Stats.ContainsKey("Hit")) res.Stats["Hit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1);
                        if (res.Stats.ContainsKey("Haste")) res.Stats["Haste"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1);
                        if (res.Stats.ContainsKey("SpellHit")) res.Stats["SpellHit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1);
                        if (res.Stats.ContainsKey("SpellCrit")) res.Stats["SpellCrit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1);
                        if (res.Stats.ContainsKey("Expertise")) res.Stats["Expertise"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1);
                    }
                    if(res.Enchantment != null)
                    {
                        if (res.Enchantment.Stats.ContainsKey("Crit")) res.Enchantment.Stats["Crit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("Hit")) res.Enchantment.Stats["Hit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("Haste")) res.Enchantment.Stats["Haste"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1);
                        if (res.Enchantment.Stats.ContainsKey("SpellHit")) res.Enchantment.Stats["SpellHit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("SpellCrit")) res.Enchantment.Stats["SpellCrit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1);
                        if (res.Enchantment.Stats.ContainsKey("Expertise")) res.Enchantment.Stats["Expertise"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1);
                    }
                    if(res.Buff != null)
                    {
                        if (res.Buff.Stats.ContainsKey("Crit")) res.Buff.Stats["Crit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);
                        if (res.Buff.Stats.ContainsKey("Hit")) res.Buff.Stats["Hit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1);
                        if (res.Buff.Stats.ContainsKey("Haste")) res.Buff.Stats["Haste"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1);
                        if (res.Buff.Stats.ContainsKey("SpellHit")) res.Buff.Stats["SpellHit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1);
                        if (res.Buff.Stats.ContainsKey("SpellCrit")) res.Buff.Stats["SpellCrit"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1);
                        if (res.Buff.Stats.ContainsKey("Expertise")) res.Buff.Stats["Expertise"] *= 100 * (Program.version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1);
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
                    if (jw.ContainsKey(s) && jw[s] != null)
                    {
                        res.Add(slot, jw.ContainsKey(s) ? JsonWeapon.ToWeapon(jw[s]) : null);
                    }
                }
                else
                {
                    string s = Player.FromSlot(slot);
                    if (je.ContainsKey(s) && je[s] != null)
                    {
                        res.Add(slot, je.ContainsKey(s) ? JsonItem.ToItem(je[s]) : null);
                    }
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
            public string Version { get; set; }
            public string Class { get; set; }
            public int Level { get; set; }
            public string Race { get; set; }
            public string Talents { get; set; }
            public string Pet { get; set; }
            public List<String> Runes { get; set; }
            public string PrePull { get; set; }
            public double StartResourcePct { get; set; }
            public Dictionary<string, bool> Cooldowns { get; set; }
            public List<JsonEnchantment> Buffs { get; set; }
            public Dictionary<string, JsonWeapon> Weapons { get; set; }
            public Dictionary<string, JsonItem> Equipment { get; set; }

            public JsonPlayer(Dictionary<string, JsonWeapon> weapons = null, Dictionary<string, JsonItem> equipment = null, string @class = "Warrior", int level = 60, string race = "Orc", string talents = "", List<JsonEnchantment> buffs = null, Dictionary<string, bool> cooldowns = null, List<string> runes = null, string version = null, string pet = null, string prePull = null, double startResourcePct = 100)
            {
                Class = @class;
                Level = level;
                Race = race;
                Talents = talents;
                Weapons = weapons;
                Equipment = equipment;
                Buffs = buffs;
                Cooldowns = cooldowns;
                Runes = runes;
                Version = version;
                Pet = pet;
                PrePull = prePull;
                StartResourcePct = startResourcePct;
            }

            public static Player ToPlayer(JsonPlayer jp, bool tanking = false, bool facing = false)
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
                                e.Attributes.SetValue(Attribute.CritChance, e.Attributes.GetValue(Attribute.CritChance) / 100 / (Program.version == ClassicCraft.Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1));
                                e.Attributes.SetValue(Attribute.HitChance, e.Attributes.GetValue(Attribute.HitChance) / 100 / (Program.version == ClassicCraft.Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1));
                                e.Attributes.SetValue(Attribute.Haste, e.Attributes.GetValue(Attribute.Haste) / 100 / (Program.version == ClassicCraft.Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1));
                                e.Attributes.SetValue(Attribute.SpellHitChance, e.Attributes.GetValue(Attribute.SpellHitChance) / 100 / (Program.version == ClassicCraft.Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                                e.Attributes.SetValue(Attribute.SpellCritChance, e.Attributes.GetValue(Attribute.SpellCritChance) / 100 / (Program.version == ClassicCraft.Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                                e.Attributes.SetValue(Attribute.Expertise, e.Attributes.GetValue(Attribute.Expertise) / 100 / (Program.version == ClassicCraft.Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1));
                            }

                            buffs.Add(e);
                        }
                    }
                }
                JsonEnchantment headGems = jp.Equipment["Head"].Gems;
                if (headGems != null)
                {
                    buffs.Add(new Enchantment(0, headGems.Name, null));
                }

                List<string> cooldowns = jp.Cooldowns.Where(v => v.Value == true).Select(c => c.Key).ToList();
                
                switch(Player.ToClass(jp.Class))
                {
                    case Player.Classes.Druid:
                        return new Druid(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Druid.TalentsFromString(jp.Talents), buffs, tanking, facing, cooldowns, jp.Runes, jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    case Player.Classes.Hunter:
                        return new Hunter(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Hunter.TalentsFromString(jp.Talents), buffs, tanking, facing, cooldowns, jp.Runes, new Entity(jp.Pet, null, Entity.MobType.Demon, jp.Level, 0, 0, null, null), jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    case Player.Classes.Paladin:
                        return new Paladin(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Paladin.TalentsFromString(jp.Talents), buffs, tanking, facing, cooldowns, jp.Runes, jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    case Player.Classes.Priest:
                        return new Priest(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Priest.TalentsFromString(jp.Talents), buffs, tanking, facing, cooldowns, jp.Runes, jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    case Player.Classes.Rogue:
                        return new Rogue(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Rogue.TalentsFromString(jp.Talents, Weapon.StringToType(jp.Weapons["isMH"]?.Type)), buffs, tanking, facing, cooldowns, jp.Runes, jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    case Player.Classes.Shaman:
                        return new Shaman(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Shaman.TalentsFromString(jp.Talents), buffs, tanking, facing, cooldowns, jp.Runes, jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    case Player.Classes.Warlock:
                        return new Warlock(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Warlock.TalentsFromString(jp.Talents), buffs, tanking, facing, cooldowns, jp.Runes, new Entity(jp.Pet, null, Entity.MobType.Demon, jp.Level, 0, 0, null, null), jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    case Player.Classes.Warrior:
                        return new Warrior(null, Player.ToRace(jp.Race), jp.Level, ToEquipment(jp.Weapons, jp.Equipment), Warrior.TalentsFromString(jp.Talents, jp.Weapons.ContainsKey("isMH") && jp.Weapons["isMH"].TwoHanded), buffs, tanking, facing, cooldowns, jp.Runes, jp.PrePull, jp.StartResourcePct) { TalentsStr = jp.Talents };
                    default:
                        throw new NotImplementedException("This class isn't supported yet : " + Player.ToClass(jp.Class));
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
                    case "Frost": return School.Frost;
                    case "Shadow": return School.Shadow;
                    case "Holy": return School.Holy;
                    case "Arcane": return School.Arcane;
                    case "Nature": return School.Nature;
                    default: throw new NotImplementedException("School type unknown : " + s);
                }
            }

            public int Level { get; set; }
            public string Type { get; set; }
            public int Armor { get; set; }
            public Dictionary<string, bool> Debuffs { get; set; }
            public Dictionary<string, int> SchoolResists { get; set; }

            public JsonBoss(int level = 63, string type = "Humanoid", int armor = 4400, Dictionary<string, int> schoolResists = null, Dictionary<string, bool> debuffs = null)
            {
                Level = level;
                Type = type;
                Armor = armor;
                SchoolResists = schoolResists;
                Debuffs = debuffs;
            }

            public static Boss ToBoss(JsonBoss jb)
            {
                int armor = jb.Armor;

                Dictionary<School, int> magicResist = null;
                if (jb.SchoolResists != null)
                {
                    magicResist = new Dictionary<School, int>();
                    foreach (string sr in jb.SchoolResists.Keys)
                    {
                        magicResist.Add(StringToSchool(sr), jb.SchoolResists[sr]);
                    }
                }

                Dictionary<string, bool> debuffsList = jb.Debuffs;
                Dictionary<string, Effect> debuffs = null;

                if (debuffsList != null && debuffsList.Count > 0)
                {
                    if (debuffsList["Improved Expose Armor"]) armor -= Program.version == Version.TBC ? 3075 : 2550;
                    else if (debuffsList["Expose Armor"]) armor -= Program.version == Version.TBC ? 2050 : 1700;
                    else if (debuffsList["Sunder Armor"]) armor -= Program.version == Version.TBC ? 2600 : 2250;
                    if (debuffsList["Curse of Recklessness"]) armor -= Program.version == Version.TBC ? 800 : 640;
                    if (debuffsList["Faerie Fire"]) armor -= Program.version == Version.TBC ? 610 : 505;
                    if (debuffsList["Annihilator"]) armor -= 600;

                    debuffs = new Dictionary<string, Effect>();
                    foreach (string s in debuffsList.Keys)
                    {
                        if(debuffsList[s]) debuffs.Add(s, new CustomEffect(null, null, s, false, -1));
                    }
                }


                return new Boss(ToType(jb.Type), jb.Level, Math.Max(0, armor), magicResist, debuffs);
            }

            public static Entity.MobType ToType(string s)
            {
                switch (s)
                {
                    case "Humanoid": return Entity.MobType.Humanoid;
                    case "Demon": return Entity.MobType.Demon;
                    case "Dragonkin": return Entity.MobType.Dragonkin;
                    case "Beast": return Entity.MobType.Beast;
                    case "Giant": return Entity.MobType.Giant;
                    case "Undead": return Entity.MobType.Undead;
                    default: return Entity.MobType.Other;
                }
            }

            public static string ToMobType(Entity.MobType mt)
            {
                switch (mt)
                {
                    case Entity.MobType.Humanoid: return "Humanoid";
                    case Entity.MobType.Demon: return "Demon";
                    case Entity.MobType.Dragonkin: return "Dragonkin";
                    case Entity.MobType.Beast: return "Beast";
                    case Entity.MobType.Giant: return "Giant";
                    case Entity.MobType.Undead: return "Undead";
                    default: return "Other";
                }
            }
        }

        public class JsonSim
        {
            public bool DoCompare {  get; set; }
            public List<string> Compare { get; set; }
            public double FightLength { get; set; }
            public double FightLengthMod { get; set; }
            public int NbSim { get; set; }
            public bool Threat { get; set; }
            public bool StatsWeights { get; set; }
            public double TargetErrorPct { get; set; }
            public bool TargetError { get; set; }
            public bool LogFight { get; set; }
            public bool BossAutoLife { get; set; }
            public double BossLowLifeTime { get; set; }
            public JsonBoss Boss { get; set; }
            public bool UnlimitedMana { get; set; }
            public bool UnlimitedResource { get; set; }
            public bool Facing { get; set; }
            public bool Tanking { get; set; }
            public double TankHitEvery { get; set; }
            public double TankHitRage { get; set; }
            public int NbTargets { get; set; }

            public JsonSim(JsonBoss boss = null, double fightLength = 300, double fightLengthMod = 0.2, int nbSim = 1000, double targetErrorPct = 0.5, bool targetError = true, bool logFight = false, bool statsWeights = false, bool bossAutoLife = true, double bossLowLifeTime = 0, bool unlimitedMana = false, bool unlimitedResource = false, bool tanking = false, double tankHitEvery = 1, double tankHitRage = 25, int nbTargets = 1, bool threat = false, bool facing = false, bool doCompare = false, List<string> compare = null)
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
                NbTargets = nbTargets;
                Facing = facing;
                Threat = threat;
                DoCompare = doCompare;
                Compare = compare;
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
                case "Holy": return School.Holy;
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
                case School.Holy: return "Holy";
                default: return "Physical";
            }
        }

        public class Config
        {
            public string Player { get; set; }
            public string Sim { get; set; }

            public Config(string player, string sim)
            {
                Player = player;
                Sim = sim;
            }
        }
    }
}

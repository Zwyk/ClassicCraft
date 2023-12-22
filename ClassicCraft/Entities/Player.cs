using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ClassicCraft
{
    public abstract class Player : Entity
    {
        protected ManaPotion pot;
        protected ManaRune rune;

        #region Util

        public class HitChances
        {
            public HitChances(Dictionary<ResultType, double> whiteHitChancesMH, Dictionary<ResultType, double> yellowHitChances, Dictionary<ResultType, double> whiteHitChancesOH = null)
            {
                WhiteHitChancesMH = whiteHitChancesMH;
                WhiteHitChancesOH = whiteHitChancesOH;
                YellowHitChances = yellowHitChances;
            }

            public Dictionary<ResultType, double> WhiteHitChancesMH { get; set; }
            public Dictionary<ResultType, double> WhiteHitChancesOH { get; set; }
            public Dictionary<ResultType, double> YellowHitChances { get; set; }
        }

        public enum Races
        {
            Human,
            Dwarf,
            NightElf,
            Gnome,
            Orc,
            Undead,
            Tauren,
            Troll,
            BloodElf,
            Draenei
        }

        public enum Classes
        {
            Priest,
            Rogue,
            Warrior,
            Mage,
            Druid,
            Hunter,
            Warlock,
            Shaman,
            Paladin
        }

        public enum Slot
        {
            Head,
            Neck,
            Shoulders,
            Back,
            Chest,
            Wrists,
            Hands,
            Waist,
            Legs,
            Feet,
            Finger1,
            Finger2,
            Trinket1,
            Trinket2,
            MH,
            OH,
            Ranged
        }

        public enum Forms
        {
            Humanoid,
            Bear,
            Cat,
            Metamorphosis,
        }

        public static Races ToRace(string s)
        {
            switch (s)
            {
                case "Blood Elf": return Races.BloodElf;
                case "Draenei": return Races.Draenei;
                case "Dwarf": return Races.Dwarf;
                case "Gnome": return Races.Gnome;
                case "Human": return Races.Human;
                case "Night Elf": return Races.NightElf;
                case "Orc": return Races.Orc;
                case "Troll": return Races.Troll;
                case "Tauren": return Races.Tauren;
                case "Undead": return Races.Undead;
                default: throw new Exception("Race not found : " + s);
            }
        }

        public static string FromRace(Races r)
        {
            switch (r)
            {
                case Races.BloodElf: return "Blood Elf";
                case Races.Draenei: return "Draenei";
                case Races.Dwarf: return "Dwarf";
                case Races.Gnome: return "Gnome";
                case Races.Human: return "Human";
                case Races.NightElf: return "NightElf";
                case Races.Orc: return "Orc";
                case Races.Tauren: return "Tauren";
                case Races.Troll: return "Troll";
                case Races.Undead: return "Undead";
                default: throw new Exception("Race not found");
            }
        }

        public static Classes ToClass(string s)
        {
            switch(s)
            {
                case "Priest": return Classes.Priest;
                case "Rogue": return Classes.Rogue;
                case "Warrior": return Classes.Warrior;
                case "Mage": return Classes.Mage;
                case "Druid": return Classes.Druid;
                case "Hunter": return Classes.Hunter;
                case "Warlock": return Classes.Warlock;
                case "Shaman": return Classes.Shaman;
                case "Paladin": return Classes.Paladin;
                default: throw new Exception("Class not found");
            }
        }

        public static string FromClass(Classes c)
        {
            switch(c)
            {
                case Classes.Priest: return "Priest";
                case Classes.Rogue: return "Rogue";
                case Classes.Warrior: return "Warrior";
                case Classes.Mage: return "Mage";
                case Classes.Druid: return "Druid";
                case Classes.Hunter: return "Hunter";
                case Classes.Warlock: return "Warlock";
                case Classes.Shaman: return "Shaman";
                case Classes.Paladin: return "Paladin";
                default: throw new Exception("Class not found");
            }
        }

        public static Slot ToSlot(string s)
        {
            switch(s)
            {
                case "Head": return Slot.Head;
                case "Neck": return Slot.Neck;
                case "Shoulders": return Slot.Shoulders;
                case "Back": return Slot.Back;
                case "Chest": return Slot.Chest;
                case "Wrists": return Slot.Wrists;
                case "Hands": return Slot.Hands;
                case "Waist": return Slot.Waist;
                case "Legs": return Slot.Legs;
                case "Feet": return Slot.Feet;
                case "Finger1": return Slot.Finger1;
                case "Finger2": return Slot.Finger2;
                case "Trinket1": return Slot.Trinket1;
                case "Trinket2": return Slot.Trinket2;
                case "MH": return Slot.MH;
                case "OH": return Slot.OH;
                case "Ranged": return Slot.Ranged;
                default: throw new Exception("Slot not found");
            }
        }

        public static string FromSlot(Slot s)
        {
            switch(s)
            {
                case Slot.Head: return "Head";
                case Slot.Neck: return "Neck";
                case Slot.Shoulders: return "Shoulders";
                case Slot.Back: return "Back";
                case Slot.Chest: return "Chest";
                case Slot.Wrists: return "Wrists";
                case Slot.Hands: return "Hands";
                case Slot.Waist: return "Waist";
                case Slot.Legs: return "Legs";
                case Slot.Feet: return "Feet";
                case Slot.Finger1: return "Finger1";
                case Slot.Finger2: return "Finger2";
                case Slot.Trinket1: return "Trinket1";
                case Slot.Trinket2: return "Trinket2";
                case Slot.MH: return "MH";
                case Slot.OH: return "OH";
                case Slot.Ranged: return "Ranged";
                default: throw new Exception("Slot not found");
            }
        }
        
        public static double StrToAPRatio(Classes c)
        {
            return (c == Classes.Druid || c == Classes.Warrior || c == Classes.Shaman || c == Classes.Paladin) ? 2 : 1;
        }

        public static double BonusStrRatio(Player p)
        {
            return 1
                * ((p.Class == Classes.Druid ? (1 + 0.04 * p.GetTalentPoints("HW")) : 1)
                * (p.Buffs.Any(b => b.Name.ToLower().Equals("blessing of kings")) ? 1.1 : 1)
                );
        }

        public static double BonusAgiRatio(Player p)
        {
            return 1
                * ((p.Buffs.Any(b => b.Name.ToLower().Equals("blessing of kings")) ? 1.1 : 1)
                * (p.Class == Classes.Rogue ? (1 + 0.01 * p.GetTalentPoints("Vitality")) : 1)
                );
        }

        public static int AgiToRangedAPRatio(Classes c)
        {
            return (c == Classes.Warrior || c == Classes.Hunter || c == Classes.Rogue) ? 2 : 1;
        }

        public static int AgiToAPRatio(Player p)
        {
            return (p.Form == Forms.Cat || p.Class == Classes.Hunter || p.Class == Classes.Rogue) ? 1 : 0;
        }

        public static double StrToBlockValueRatio()
        {
            return 0.05;
        }

        public static double AgiToDodgeRatio(Classes c, int level)
        {
            if (level < 60 && c == Classes.Warlock)
            {
                return (0.0446055 * Math.Pow(level, 1.39285) + 6.64614) / 100; // Approx
            }
            else
            {
                switch (c)
                {
                    case Classes.Hunter: return 1.0 / 100 / (Program.version == Version.TBC ? 40 : 26);     // TBC ?
                    case Classes.Rogue: return 1.0 / 100 / (Program.version == Version.TBC ? 40 : 14.5);    // TBC ?
                    default: return 1.0 / 100 / (Program.version == Version.TBC ? 25 : 20);                 // TBC ?
                }
            }
        }

        public static double AgiToCritRatio(Classes c, int level)
        {
            if(level < 60 && c == Classes.Warlock)
            {
                return (-0.026779 * Math.Pow(level, 0.330417) + 0.153575) /100; // Approx
            }
            else
            {
                switch (c)
                {
                    case Classes.Hunter: return 1.0 / 100 / (Program.version == Version.TBC ? 40 : 53);
                    case Classes.Rogue: return 1.0 / 100 / (Program.version == Version.TBC ? 40 : 29);
                    case Classes.Warrior: return 1.0 / 100 / (Program.version == Version.TBC ? 33 : 20);
                    default: return 1.0 / 100 / (Program.version == Version.TBC ? 25 : 20);
                }
            }
        }

        public static double IntToCritRatio(Classes c, int level)
        {
            if (level < 60 && c == Classes.Warlock)
            {
                return (-0.435224 * Math.Pow(level, 0.0574586) + 0.565842) / 100; // Approx
            }
            else
            {
                switch (c)
                {
                    case Classes.Druid: return 1.0 / 100 / (Program.version == Version.TBC ? 80 : 60);
                    case Classes.Mage: return 1.0 / 100 / (Program.version == Version.TBC ? 80 : 59.50);
                    case Classes.Paladin: return 1.0 / 100 / (Program.version == Version.TBC ? 80.05 : 54);
                    case Classes.Priest: return 1.0 / 100 / (Program.version == Version.TBC ? 80 : 59.20);
                    case Classes.Shaman: return 1.0 / 100 / (Program.version == Version.TBC ? 80 : 59.50);
                    case Classes.Warlock: return 1.0 / 100 / (Program.version == Version.TBC ? 81.92 : 60.60);
                    default: return 0;
                }
            }
        }

        public static double BaseSpellCrit(Classes c, int level)
        {
            switch (c)
            {
                case Classes.Druid: return 0.0185;
                case Classes.Hunter: return 0.036;
                case Classes.Mage: return 0.0091;
                case Classes.Paladin: return 0.03336;
                case Classes.Priest: return 0.0124;
                case Classes.Shaman: return 0.022;
                case Classes.Warlock: return 0.01701;
                default: return 0;
            }
        }

        public static Attributes BaseAttributes(Classes c, Races r, int level)
        {
            // TODO : by level, TBC, BF/Dra

            Attributes res = new Attributes();

            res.Values.Add(Attribute.CritChance, 0.025);

            switch (c)
            {
                case Classes.Druid:
                    res.Values.Add(Attribute.Stamina, 69);  // lvl 60 but doesn't matter
                    res.Values.Add(Attribute.Health, 1483); // lvl 60 but doesn't matter
                    // TAUREN BASES
                    if (level <= 25)
                    {
                        res.Values.Add(Attribute.Strength, 40 - 5);
                        res.Values.Add(Attribute.Agility, 28 + 5);
                        res.Values.Add(Attribute.Intellect, 42 + 5);
                        res.Values.Add(Attribute.Spirit, 52 - 1);
                        res.Values.Add(Attribute.Mana, 923 - (20 + 15 * (res.Values[Attribute.Intellect] - 20)));
                    }
                    else
                    {
                        res.Values.Add(Attribute.Strength, 70 - 5);
                        res.Values.Add(Attribute.Agility, 55 + 5);
                        res.Values.Add(Attribute.Intellect, 95 + 5);
                        res.Values.Add(Attribute.Spirit, 112 - 1);
                        res.Values.Add(Attribute.Mana, 1244);
                    }
                    break;
                case Classes.Hunter:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 57);
                            res.Values.Add(Attribute.Agility, 121);
                            res.Values.Add(Attribute.Stamina, 93);
                            res.Values.Add(Attribute.Intellect, 64);
                            res.Values.Add(Attribute.Spirit, 69);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 52);
                            res.Values.Add(Attribute.Agility, 130);
                            res.Values.Add(Attribute.Stamina, 89);
                            res.Values.Add(Attribute.Intellect, 65);
                            res.Values.Add(Attribute.Spirit, 70);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 58);
                            res.Values.Add(Attribute.Agility, 122);
                            res.Values.Add(Attribute.Stamina, 92);
                            res.Values.Add(Attribute.Intellect, 62);
                            res.Values.Add(Attribute.Spirit, 73);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 60);
                            res.Values.Add(Attribute.Agility, 120);
                            res.Values.Add(Attribute.Stamina, 92);
                            res.Values.Add(Attribute.Intellect, 60);
                            res.Values.Add(Attribute.Spirit, 72);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 56);
                            res.Values.Add(Attribute.Agility, 127);
                            res.Values.Add(Attribute.Stamina, 91);
                            res.Values.Add(Attribute.Intellect, 61);
                            res.Values.Add(Attribute.Spirit, 71);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Mage:
                    switch (r)
                    {
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 25);
                            res.Values.Add(Attribute.Agility, 38);
                            res.Values.Add(Attribute.Stamina, 44);
                            res.Values.Add(Attribute.Intellect, 133);
                            res.Values.Add(Attribute.Spirit, 120);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 30);
                            res.Values.Add(Attribute.Agility, 35);
                            res.Values.Add(Attribute.Stamina, 45);
                            res.Values.Add(Attribute.Intellect, 125);
                            res.Values.Add(Attribute.Spirit, 126);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 31);
                            res.Values.Add(Attribute.Agility, 37);
                            res.Values.Add(Attribute.Stamina, 46);
                            res.Values.Add(Attribute.Intellect, 121);
                            res.Values.Add(Attribute.Spirit, 121);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 29);
                            res.Values.Add(Attribute.Agility, 33);
                            res.Values.Add(Attribute.Stamina, 46);
                            res.Values.Add(Attribute.Intellect, 123);
                            res.Values.Add(Attribute.Spirit, 125);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Paladin:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 107);
                            res.Values.Add(Attribute.Agility, 61);
                            res.Values.Add(Attribute.Stamina, 103);
                            res.Values.Add(Attribute.Intellect, 69);
                            res.Values.Add(Attribute.Spirit, 74);
                            res.Values.Add(Attribute.Health, 1381);
                            res.Values.Add(Attribute.Mana, 1512);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 105);
                            res.Values.Add(Attribute.Agility, 65);
                            res.Values.Add(Attribute.Stamina, 100);
                            res.Values.Add(Attribute.Intellect, 70);
                            res.Values.Add(Attribute.Spirit, 78);
                            res.Values.Add(Attribute.Health, 1381);
                            res.Values.Add(Attribute.Mana, 1510);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Priest:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 37);
                            res.Values.Add(Attribute.Agility, 36);
                            res.Values.Add(Attribute.Stamina, 53);
                            res.Values.Add(Attribute.Intellect, 119);
                            res.Values.Add(Attribute.Spirit, 124);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 35);
                            res.Values.Add(Attribute.Agility, 40);
                            res.Values.Add(Attribute.Stamina, 50);
                            res.Values.Add(Attribute.Intellect, 120);
                            res.Values.Add(Attribute.Spirit, 131);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 32);
                            res.Values.Add(Attribute.Agility, 45);
                            res.Values.Add(Attribute.Stamina, 49);
                            res.Values.Add(Attribute.Intellect, 120);
                            res.Values.Add(Attribute.Spirit, 125);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 36);
                            res.Values.Add(Attribute.Agility, 42);
                            res.Values.Add(Attribute.Stamina, 51);
                            res.Values.Add(Attribute.Intellect, 116);
                            res.Values.Add(Attribute.Spirit, 126);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 34);
                            res.Values.Add(Attribute.Agility, 38);
                            res.Values.Add(Attribute.Stamina, 51);
                            res.Values.Add(Attribute.Intellect, 118);
                            res.Values.Add(Attribute.Spirit, 130);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            res.Values.Add(Attribute.CritChance, 0.03);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Rogue:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, level == 70 ? 97 : 82);
                            res.Values.Add(Attribute.Agility, level == 70 ? 154 : 126);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 92 : 78);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 38 : 34);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 57 : 49);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, level == 70 ? 90 : 75);
                            res.Values.Add(Attribute.Agility, level == 70 ? 161 : 133);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 88 : 74);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 45 : 40);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 58 : 50);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, level == 70 ? 95 : 80);
                            res.Values.Add(Attribute.Agility, level == 70 ? 158 : 130);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 89 : 75);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 39 : 35);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 63 : 52);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, level == 70 ? 92 : 77);
                            res.Values.Add(Attribute.Agility, level == 70 ? 163 : 135);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 88 : 74);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 39 : 35);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 58 : 50);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, level == 70 ? 98 : 83);
                            res.Values.Add(Attribute.Agility, level == 70 ? 155 : 127);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 91 : 77);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 36 : 32);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 61 : 53);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, level == 70 ? 96 : 81);
                            res.Values.Add(Attribute.Agility, level == 70 ? 160 : 132);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 90 : 76);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 35 : 31);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 59 : 51);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, level == 70 ? 94 : 79);
                            res.Values.Add(Attribute.Agility, level == 70 ? 156 : 128);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 90 : 76);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 37 : 33);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 63 : 55);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.BloodElf:
                            res.Values.Add(Attribute.Strength, level == 70 ? 92 : 79);
                            res.Values.Add(Attribute.Agility, level == 70 ? 160 : 136);
                            res.Values.Add(Attribute.Stamina, level == 70 ? 88 : 76);
                            res.Values.Add(Attribute.Intellect, level == 70 ? 43 : 39);
                            res.Values.Add(Attribute.Spirit, level == 70 ? 57 : 50);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Shaman:
                    switch (r)
                    {
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 88);
                            res.Values.Add(Attribute.Agility, 52);
                            res.Values.Add(Attribute.Stamina, 97);
                            res.Values.Add(Attribute.Intellect, 87);
                            res.Values.Add(Attribute.Spirit, 103);
                            res.Values.Add(Attribute.Health, 1280);
                            res.Values.Add(Attribute.Mana, 1520);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 90);
                            res.Values.Add(Attribute.Agility, 50);
                            res.Values.Add(Attribute.Stamina, 97);
                            res.Values.Add(Attribute.Intellect, 85);
                            res.Values.Add(Attribute.Spirit, 102);
                            res.Values.Add(Attribute.Health, 1280);
                            res.Values.Add(Attribute.Mana, 1520);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 86);
                            res.Values.Add(Attribute.Agility, 57);
                            res.Values.Add(Attribute.Stamina, 96);
                            res.Values.Add(Attribute.Intellect, 86);
                            res.Values.Add(Attribute.Spirit, 101);
                            res.Values.Add(Attribute.Health, 1280);
                            res.Values.Add(Attribute.Mana, 1520);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Warlock:
                    res.Values.Add(Attribute.Stamina, 66);  // lvl 60 but doesn't matter
                    res.Values.Add(Attribute.Health, 1414); // lvl 60 but doesn't matter
                    // ORC BASES
                    if (level <= 25)
                    {
                        res.Values.Add(Attribute.Strength, 31 - 3);
                        res.Values.Add(Attribute.Agility, 27 + 3);
                        res.Values.Add(Attribute.Intellect, 47 + 3);
                        res.Values.Add(Attribute.Spirit, 55 - 2);
                        res.Values.Add(Attribute.Mana, 923 - (20 + 15 * (res.Values[Attribute.Intellect] - 20)));
                    }
                    else if (level <= 40)
                    {
                        res.Values.Add(Attribute.Strength, 37 - 3);
                        res.Values.Add(Attribute.Agility, 34 + 3);
                        res.Values.Add(Attribute.Intellect, 69 + 3);
                    }
                    else if (level <= 50)
                    {
                        res.Values.Add(Attribute.Strength, 42 - 3);
                        res.Values.Add(Attribute.Agility, 40 + 3);
                        res.Values.Add(Attribute.Intellect, 87 + 3);
                    }
                    else if (level <= 60)
                    {
                        res.Values.Add(Attribute.Strength, 48 - 3);
                        res.Values.Add(Attribute.Agility, 47 + 3);
                        res.Values.Add(Attribute.Intellect, 107 + 3);
                    }
                    else
                    {
                        res.Values.Add(Attribute.Strength, 40 - 3);
                        res.Values.Add(Attribute.Agility, 53 + 3);
                        res.Values.Add(Attribute.Intellect, 119 + 3);
                    }
                    break;
                case Classes.Warrior:
                    res.Values.Add(Attribute.Stamina, level == 70 ? 136 : 113);
                    res.Values.Add(Attribute.Health, 1689); // lvl 70
                    res.Values.Add(Attribute.Mana, 0);
                    if (level <= 25)
                    {
                        res.Values.Add(Attribute.Strength, 54);
                        res.Values.Add(Attribute.Agility, 39);
                        res.Values.Add(Attribute.Intellect, 23);
                        res.Values.Add(Attribute.Spirit, 30);
                    }
                    else if(level <= 60)
                    {
                        res.Values.Add(Attribute.Strength, 120);
                        res.Values.Add(Attribute.Agility, 80);
                        res.Values.Add(Attribute.Intellect, 30);
                        res.Values.Add(Attribute.Spirit, 47);
                    }
                    else
                    {
                        res.Values.Add(Attribute.Strength, 145);
                        res.Values.Add(Attribute.Agility, 96);
                        res.Values.Add(Attribute.Intellect, 33);
                        res.Values.Add(Attribute.Spirit, 56);
                    }
                    break;
                default: break;
            }

            switch (r)
            {
                case Races.Human:
                    break;
                case Races.Dwarf:
                    res.Values[Attribute.Strength] += 2;
                    res.Values[Attribute.Agility] += -4;
                    res.Values[Attribute.Intellect] += -1;
                    res.Values[Attribute.Spirit] += -2;
                    break;
                case Races.NightElf:
                    res.Values[Attribute.Strength] += -3;
                    res.Values[Attribute.Agility] += 5;
                    res.Values[Attribute.Intellect] += 0;
                    res.Values[Attribute.Spirit] += -1;
                    break;
                case Races.Gnome:
                    res.Values[Attribute.Strength] += -5;
                    res.Values[Attribute.Agility] += 3;
                    res.Values[Attribute.Intellect] += 4;
                    res.Values[Attribute.Spirit] += -1;
                    break;
                case Races.Orc:
                    res.Values[Attribute.Strength] += 3;
                    res.Values[Attribute.Agility] += -3;
                    res.Values[Attribute.Intellect] += -3;
                    res.Values[Attribute.Spirit] += 2;
                    break;
                case Races.Undead:
                    res.Values[Attribute.Strength] -= -1;
                    res.Values[Attribute.Agility] += -2;
                    res.Values[Attribute.Intellect] += -2;
                    res.Values[Attribute.Spirit] += 4;
                    break;
                case Races.Tauren:
                    res.Values[Attribute.Strength] += 5;
                    res.Values[Attribute.Agility] += -5;
                    res.Values[Attribute.Intellect] += -5;
                    res.Values[Attribute.Spirit] += 1;
                    break;
                case Races.Troll:
                    res.Values[Attribute.Strength] += 1;
                    res.Values[Attribute.Agility] += 2;
                    res.Values[Attribute.Intellect] += -4;
                    res.Values[Attribute.Spirit] += 0;
                    break;
                default: break;
            }

            return res;
        }

        public static Attributes BonusAttributesByRace(Races r, Attributes a)
        {
            Attributes res = new Attributes();

            switch (r)
            {
                case Races.Gnome:
                    res.SetValue(Attribute.Intellect, a.GetValue(Attribute.Intellect) * 0.05);
                    break;
                case Races.Human:
                    res.SetValue(Attribute.Spirit, a.GetValue(Attribute.Spirit) * 0.05);
                    break;
                default: break;
            }

            return res;
        }

        public int BaseAP(Classes c, int level)
        {
            switch (c)
            {
                case Classes.Warrior:
                    return level * 3 - 20;
                case Classes.Paladin:
                    return level * 3 - 20;
                case Classes.Mage:
                    return -10;
                case Classes.Priest:
                    return -10;
                case Classes.Warlock:
                    return -10;
                case Classes.Druid:
                    // -20 // Normal
                    // level * 1.5 - 20 // Moonkin
                    return Tanking ?
                        level * 3 - 20 + 180: //Bear
                        level * 2 - 20 + 40; // Cat
                default:
                    return level * 2 - 20;
            }
        }

        public static int BaseRAP(Classes c, int level)
        {
            switch (c)
            {
                case Classes.Hunter:
                    return level * 2 - 10;
                case Classes.Warrior:
                    return level - 10;
                case Classes.Rogue:
                    return level - 10;
                default:
                    return 0;
            }
        }

        public static List<ResultType> ATTACKTABLE_ORDER = new List<ResultType>()
        {
            ResultType.Miss, ResultType.Dodge, ResultType.Parry, ResultType.Glance, ResultType.Block, ResultType.Crit, ResultType.Hit
        };

        public static double CritWithSuppression(double critChance, int level, int enemyLevel)
        {
            int skillDif = level * 5 - enemyLevel * 5;
            return critChance + skillDif * (skillDif >= 0 ? 0.0004 : 0.002);
        }

        public static List<string> SPECIALSETS_LIST = new List<string>()
        {
            "Dreadnaught",
            "Nightslayer", "Shadowcraft", "Darkmantle",
            "Warbringer", "Destroyer", "Onslaught",
            "Netherblade", "Deathmantle", "Slayer's",
            "Assassination",
            "Wastewalker", // 2 = 35 hit
            "Felsteel", // 3 = 25 str
            "Ragesteel", // 2 = 20 hit
            "Doomplate", // 2 = 35 hit
            "Desolation", // 2 = 35 hit
        };

        public static Dictionary<Attribute, double> RatingRatios = new Dictionary<Attribute, double>()
        {
            { Attribute.Expertise, 15.77 },
            { Attribute.HitChance, 15.77 },
            { Attribute.CritChance, 22.08 },
            { Attribute.SpellHitChance, 12.62 },
            { Attribute.SpellCritChance, 22.08 },
            { Attribute.Haste, 15.77 },
            // { Attribute.Defense, 2.37 },
            // { Attribute.Dodge, 18.92 },
            // { Attribute.Parry, 23.65 },
            // { Attribute.Block, 7.88 },
        };

        public bool IsCaster()
        {
            return Class == Classes.Mage || Class == Classes.Paladin || Class == Classes.Priest
                || Class == Classes.Shaman || Class == Classes.Warlock || (Class == Classes.Druid && Form != Forms.Bear && Form != Forms.Cat);
        }

        #endregion

        #region Properties

        public static bool QCS = false;

        public static double GCD = 1.5 + (QCS ? 0.1 : 0);
        public static double GCD_ENERGY = 1;

        public Forms Form { get; set; }
        public bool Stealthed { get; set; }

        public int MaxResource = 100;

        private int resource;
        public int Resource
        {
            get
            {
                return resource;
            }
            set
            {
                if (Sim != null && Sim.UnlimitedResource)
                {
                    resource = MaxResource;
                }
                else
                {
                    if (value > MaxResource)
                    {
                        resource = MaxResource;
                    }
                    else if (value < 0)
                    {
                        resource = 0;
                    }
                    else
                    {
                        resource = value;
                    }
                }
            }
        }

        public double ThreatMod { get; set; }

        public double BaseMana { get; set; }

        public int MaxMana
        {
            get { return (int)Attributes.GetValue(Attribute.Mana); }
        }

        private int mana;
        public int Mana
        {
            get
            {
                return mana;
            }
            set
            {
                if (Sim != null && Sim.UnlimitedMana)
                {
                    mana = MaxMana;
                }
                else
                {
                    if (value < 0)
                    {
                        value = 0;
                    }
                    else if (value > MaxMana)
                    {
                        value = MaxMana;
                    }

                    if (value < mana)
                    {
                        LastManaExpenditure = Sim.CurrentTime;
                    }

                    mana = value;
                }
            }
        }

        public double LastManaExpenditure = -1000;

        private int combo;
        public int Combo
        {
            get
            {
                return combo;
            }
            set
            {
                if (value < 0)
                {
                    combo = 0;
                }
                else if (value > 5)
                {
                    combo = 5;
                }
                else
                {
                    combo = value;
                }
            }
        }


        public Dictionary<Slot, Item> Equipment { get; set; }

        public Entity Pet { get; set; }

        public string TalentsStr { get; set; }
        public Dictionary<string, int> Talents { get; set; }

        public List<string> Runes { get; set; }

        public List<Enchantment> Buffs { get; set; }

        public Weapon MH
        {
            get
            {
                return Equipment.ContainsKey(Slot.MH) ? (Weapon)Equipment[Slot.MH] : null;
            }
            set
            {
                Equipment[Slot.MH] = value;
            }
        }

        public Weapon OH
        {
            get
            {
                if (Equipment.ContainsKey(Slot.OH) && Equipment[Slot.OH] != null)
                {
                    return (Weapon)Equipment[Slot.OH];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Equipment[Slot.OH] = value;
            }
        }

        public Weapon Ranged
        {
            get
            {
                if (Equipment[Slot.Ranged] != null)
                {
                    return (Weapon)Equipment[Slot.Ranged];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Equipment[Slot.Ranged] = value;
            }
        }

        public double GCDUntil { get; set; }
        public double ResourcesTick { get; set; }
        public double SpiritTick { get; set; }
        public double ManaTick { get; set; }

        public double MPTRatio { get; set; }
        public double CastingManaRegenRate { get; set; }

        public double SchoolSP(School school)
        {
            double bonus;
            switch(school)
            {
                case School.Arcane: bonus = Attributes.GetValue(Attribute.SPArcane); break;
                case School.Fire: bonus = Attributes.GetValue(Attribute.SPFire); break;
                case School.Frost: bonus = Attributes.GetValue(Attribute.SPFrost); break;
                case School.Nature: bonus = Attributes.GetValue(Attribute.SPNature); break;
                case School.Shadow: bonus = Attributes.GetValue(Attribute.SPShadow); break;
                default: bonus = 0; break;
            }
            return SP + bonus;
        }

        public double AP
        {
            get
            {
                return Attributes.GetValue(Attribute.AP);
            }
        }

        public double SP
        {
            get
            {
                return Attributes.GetValue(Attribute.SP);
            }
        }

        public double CritChance
        {
            get
            {
                return Attributes.GetValue(Attribute.CritChance);
            }
            set
            {
                Attributes.SetValue(Attribute.CritChance, value);
            }
        }

        public double HitChance
        {
            get
            {
                return Attributes.GetValue(Attribute.HitChance);
            }
            set
            {
                Attributes.SetValue(Attribute.HitChance, value);
            }
        }

        public double ExpertisePercent
        {
            get
            {
                return Attributes.GetValue(Attribute.Expertise);
            }
        }

        public double SpellCritChance
        {
            get
            {
                return Attributes.GetValue(Attribute.SpellCritChance);
            }
            set
            {
                Attributes.SetValue(Attribute.SpellCritChance, value);
            }
        }

        public double SpellHitChance
        {
            get
            {
                return Attributes.GetValue(Attribute.SpellHitChance);
            }
            set
            {
                Attributes.SetValue(Attribute.SpellHitChance, value);
            }
        }

        public double BlockValue
        {
            get
            {
                return Attributes.GetValue(Attribute.BlockValue) * (1 + 0.1 * GetTalentPoints("SM")) + Attributes.GetValue(Attribute.Strength) * StrToBlockValueRatio();
            }
        }

        public bool DualWielding
        {
            get { return MH != null && !MH.TwoHanded && OH != null && OH.Type != Weapon.WeaponType.Offhand && OH.Type != Weapon.WeaponType.Shield; }
        }

        public Attributes Attributes { get; set; }

        public double HasteMod { get; set; }
        public double DamageMod { get; set; }

        public Races Race { get; set; }
        public Classes Class { get; set; }

        public bool Tanking { get; set; }
        public bool Facing { get; set; }

        public Action applyAtNextAA = null;
        public int nextAABonus = 0;

        public Dictionary<Weapon.WeaponType, int> WeaponSkill { get; set; }

        public Dictionary<Entity, HitChances> HitChancesByEnemy { get; set; }

        public bool WindfuryTotem { get; set; }
        public int WindfuryTotemImp = 0;
        public bool WildStrikes { get; set; }
            
        public List<string> Cooldowns { get; set; }
        public string PrePull {  get; set; }

        public Dictionary<string, int> Sets { get; set; }
        public int SetPieces(string set) { return Sets.ContainsKey(set) ? Sets[set] : 0; }

        public Dictionary<string, CustomAction> CustomActions { get; set; }

        public Dictionary<Skill, int> cds = new Dictionary<Skill, int>();
        public Dictionary<string, double> icds = new Dictionary<string, double>();

        public AutoAttack mh = null;
        public AutoAttack oh = null;
        public AutoAttack ranged = null;

        public Spell casting = null;

        public int rota = 0;

        #endregion

        #region Constructors

        public Player(Player p)
            : this(p.Sim, p)
        {
        }

        public Player(Simulation s, Player p)
            : this(s, p.Class, p.Race, p.Level, p.Equipment, p.Talents, p.Buffs, p.Tanking, p.Facing, p.Cooldowns, p.Runes, p.Pet, p.PrePull)
        {
            Attributes.Values = new Dictionary<Attribute, double>(p.Attributes.Values);

            Tanking = p.Tanking;
            Facing = p.Facing;

            DamageMod = p.DamageMod;
            ThreatMod = p.ThreatMod;
            MPTRatio = p.MPTRatio;
            CastingManaRegenRate = p.CastingManaRegenRate;

            WeaponSkill = p.WeaponSkill;
            MH.DamageMin = p.MH.DamageMin;
            MH.DamageMax = p.MH.DamageMax;
            if (DualWielding)
            {
                OH.DamageMin = p.OH.DamageMin;
                OH.DamageMax = p.OH.DamageMax;
            }
            WindfuryTotem = p.WindfuryTotem;
            WindfuryTotemImp = p.WindfuryTotemImp;
            WildStrikes = p.WildStrikes;
            Cooldowns = p.Cooldowns;
            BaseMana = p.BaseMana;

            Sets = p.Sets;
            ApplySets();
        }

        public Player(Simulation s, Classes c, Races r, int level, Dictionary<Slot, Item> items, 
            Dictionary<string, int> talents, List<Enchantment> buffs, bool tanking, bool facing, List<string> cooldowns, List<string> runes, Entity pet, string prepull)
            : base("Player", s, MobType.Humanoid, level, 0, 0, null, null)
        {
            Race = r;
            Class = c;

            Tanking = tanking;
            Facing = facing;

            pot = new ManaPotion(this);
            rune = new ManaRune(this);
            
            Talents = new Dictionary<string, int>(talents != null ? talents : new Dictionary<string, int>());

            Pet = pet;

            Runes = runes;

            Buffs = new List<Enchantment>(buffs != null ? buffs : new List<Enchantment>());

            int baseSkill = Level * 5;
            WeaponSkill = new Dictionary<Weapon.WeaponType, int>();
            foreach (Weapon.WeaponType type in (Weapon.WeaponType[])Enum.GetValues(typeof(Weapon.WeaponType)))
            {
                WeaponSkill[type] = baseSkill;
            }

            if(items == null)
            {
                Equipment = new Dictionary<Slot, Item>();
                foreach (Slot slot in (Slot[])Enum.GetValues(typeof(Slot)))
                {
                    Equipment[slot] = null;
                }
            }
            else
            {
                Equipment = new Dictionary<Slot, Item>(items);
            }

            if (MH == null) MH = new Weapon();

            HitChancesByEnemy = new Dictionary<Entity, HitChances>();

            CustomActions = new Dictionary<string, CustomAction>();

            Cooldowns = cooldowns;
            PrePull = prepull;

            Sets = new Dictionary<string, int>();
            ApplySets();

            Reset();
        }

        #endregion

        #region Rota

        public Boss Target = null;

        public virtual void PrepFight()
        {
            Target = Sim.Boss[0];

            if (MH.Speed > 0)
            {
                mh = new AutoAttack(this, MH, true);
                if (DualWielding)
                {
                    oh = new AutoAttack(this, OH, false);
                }
            }

            foreach (string s in Cooldowns)
            {
                switch (s)
                {
                    case "Bloodlust": cds.Add(new ActiveItemBuff(this, 600, 40, "Bloodlust", new Dictionary<Attribute, double>() { { Attribute.Haste, 0.3 } }), 40); break;
                    case "Haste Potion": cds.Add(new ActiveItemBuff(this, 120, 15, "Haste Pot", new Dictionary<Attribute, double>() { { Attribute.Haste, 400 / RatingRatios[Attribute.Haste] / 100 } }), 15); break;
                    case "Insane Strength Potion": cds.Add(new ActiveItemBuff(this, 120, 15, "Str Pot", new Dictionary<Attribute, double>() { { Attribute.Haste, 120 } }), 15); break;
                    case "Drums of Battle": cds.Add(new ActiveItemBuff(this, 120, 30, "Drums of Battle", new Dictionary<Attribute, double>() { { Attribute.Haste, 80 / RatingRatios[Attribute.Haste] / 100 } }), 30); break;
                    case "Drums of War": cds.Add(new ActiveItemBuff(this, 120, 30, "Drums of Battle", new Dictionary<Attribute, double>() { { Attribute.AP, 60 } }), 30); break;
                    default: break;
                }
            }

            string[] tr = new string[] { Equipment[Slot.Trinket1]?.Name.ToLower(), Equipment[Slot.Trinket2]?.Name.ToLower() };

            if (tr.Any(x => x == "kiss of the spider"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 15, "Kiss of the Spider", new Dictionary<Attribute, double>() { { Attribute.Haste, 200 } }), 15);
            }
            if (tr.Any(x => x == "abacus of violent odds"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 10, "Abacus of Violet Odds", new Dictionary<Attribute, double>() { { Attribute.Haste, 260 / RatingRatios[Attribute.Haste] / 100 } }), 10);
            }
            if (tr.Any(x => x == "bladefist's breadth"))
            {
                cds.Add(new ActiveItemBuff(this, 90, 15, "Bladefist's Breadth", new Dictionary<Attribute, double>() { { Attribute.AP, 200 } }), 15);
            }
            if (tr.Any(x => x == "bloodlust brooch"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 20, "Bloodlust Brooch", new Dictionary<Attribute, double>() { { Attribute.AP, 278 } }), 20);
            }
            if (tr.Any(x => x == "ancient draenei war talisman"))
            {
                cds.Add(new ActiveItemBuff(this, 90, 15, "Ancient Draenei War Talisman", new Dictionary<Attribute, double>() { { Attribute.AP, 200 } }), 15);
            }
            if (tr.Any(x => x == "core of ar'kelos"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 20, "Core of Ar'kelos", new Dictionary<Attribute, double>() { { Attribute.AP, 200 } }), 20);
            }
            if (tr.Any(x => x == "uniting charm"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 15, "Uniting Charm", new Dictionary<Attribute, double>() { { Attribute.AP, 185 } }), 15);
            }
            if (tr.Any(x => x == "terokkar tablet of precision"))
            {
                cds.Add(new ActiveItemBuff(this, 90, 15, "Terokkar Tablet of Precision", new Dictionary<Attribute, double>() { { Attribute.AP, 140 } }), 15);
            }
            if (tr.Any(x => x == "crystalforged trinket"))
            {
                cds.Add(new ActiveItemBuff(this, 60, 10, "Crystalforged Trinket", new Dictionary<Attribute, double>() { { Attribute.AP, 216 } }), 10);
            }
            if (tr.Any(x => x == "icon of unyielding courage"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 20, "Icon of Unyielding Courage", new Dictionary<Attribute, double>() { { Attribute.ArmorPen, 600 } }), 20);
            }
            if (tr.Any(x => x == "berserker's call"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 20, "Berserker's Call", new Dictionary<Attribute, double>() { { Attribute.AP, 360 } }), 20);
            }
            if (tr.Any(x => x == "empty mug of direbrew"))
            {
                cds.Add(new ActiveItemBuff(this, 120, 20, "Empty Mug of Direbrew", new Dictionary<Attribute, double>() { { Attribute.AP, 278 } }), 20);
            }
            if (tr.Any(x => x.Contains("nightseye panther")))
            {
                cds.Add(new ActiveItemBuff(this, 180, 12, "Figurine - Nightseye Panther", new Dictionary<Attribute, double>() { { Attribute.AP, 320 } }), 12);
            }

            if(Equipment[Slot.Chest].Name.ToLower().Contains("bulwark of"))
            {
                cds.Add(new ActiveItemBuff(this, 1800, 15, "Bulwark chest", new Dictionary<Attribute, double>() { { Attribute.Strength, 150 } }), 15);
            }
        }

        public abstract void Rota();

        public void Unshift()
        {
            Form = Forms.Humanoid;
            ResetMHSwing();

            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : Unshifted", Sim.CurrentTime));
            }
        }

        public void CheckAAs()
        {
            if(mh != null)
            {
                CheckAA(mh);
            }
            if(DualWielding)
            {
                CheckAA(oh);
            }
        }

        public void CheckAA(AutoAttack aa)
        {
            if (aa.Available())
            {
                if (casting == null)
                {
                    if (aa.MH && applyAtNextAA != null)
                    {
                        if (applyAtNextAA.CanUse())
                        {
                            applyAtNextAA.DoAction();
                            aa.CastNextSwing();
                        }
                        else
                        {
                            applyAtNextAA = null;
                            aa.Cast(Target);
                        }
                    }
                    else
                    {
                        aa.Cast(Target);
                    }
                }
                else
                {
                    aa.LockedUntil = Sim.CurrentTime;
                }
            }
        }

        #endregion

        #region Methods

        public void ResetMHSwing()
        {
            ResetAATimer(mh);
        }

        public void ResetAATimer(AutoAttack auto)
        {
            if(auto != null)
            {
                auto.ResetSwing();
            }
        }

        public void SetupSets()
        {
            CheckSets();
            ApplySets();
        }

        public void CheckSets()
        {
            foreach (string s in SPECIALSETS_LIST)
            {
                int nb = NbSet(s);
                if (nb > 0) Sets.Add(s, nb);
            }
        }

        public void ApplySets()
        {
            if (Class == Classes.Rogue)
            {
                if (NbSet("Nightslayer") >= 5)
                {
                    MaxResource += 10;
                }
            }
        }

        public int NbSet(string set)
        {
            return Equipment.Where(a => a.Value != null).Count(e => e.Value.Name.ToLower().Contains(set.ToLower()));
        }

        public int GetTalentPoints(string talent)
        {
            return Talents != null && Talents.ContainsKey(talent) ? Talents[talent] : 0;
        }

        public override void Reset()
        {
            base.Reset();

            Resource = 0;

            GCDUntil = 0;
            ResourcesTick = 0;
            Combo = 0;

            HasteMod = CalcHaste();
            DamageMod = 1;
            ThreatMod = 1;
            MPTRatio = 1;
            CastingManaRegenRate = 0;

            Form = Forms.Humanoid;
            Stealthed = false;

            Attributes = new Attributes();
            BaseMana = Attributes.GetValue(Attribute.Mana);
        }

        public void SetBaseAttributes()
        {
            Attributes = new Attributes();
            Attributes.Values = new Dictionary<Attribute, double>(BaseAttributes(Class, Race, Level).Values);
            Attributes.SetValue(Attribute.AP, BaseAP(Class, Level));
            Attributes.SetValue(Attribute.RangedAP, BaseRAP(Class, Level));
            BaseMana = Attributes.GetValue(Attribute.Mana);
        }

        public void CalculateAttributes()
        {
            SetBaseAttributes();

            DamageMod = 1;
            ThreatMod = 1;
            MPTRatio = 1;
            CastingManaRegenRate = 0;

            foreach (Slot s in Equipment.Keys.Where(v => Equipment[v] != null))
            {
                Item i = Equipment[s];
                Attributes += i.Attributes;
                if (i.Enchantment != null)
                {
                    Attributes += i.Enchantment.Attributes;
                }
                if (s == Slot.MH)
                {
                    Weapon w = ((Weapon)i);
                    if (w.Buff != null)
                        Attributes += w.Buff.Attributes;
                }
            }

            foreach (Enchantment e in Buffs.Where(v => v != null))
            {
                Attributes += e.Attributes;
            }

            double wbonus = Attributes.GetValue(Attribute.WeaponDamage);
            double wbonusMH = Attributes.GetValue(Attribute.WeaponDamageMH);
            double wbonusOH = Attributes.GetValue(Attribute.WeaponDamageOH);
            MH.DamageMin = MH.BaseMin + wbonus + wbonusMH;
            MH.DamageMax = MH.BaseMax + wbonus + wbonusMH;
            if (DualWielding)
            {
                OH.DamageMin = OH.BaseMin + wbonus + wbonusOH;
                OH.DamageMax = OH.BaseMax + wbonus + wbonusOH;
            }

            WildStrikes = Buffs.Any(b => b.Name.Contains("Wild Strikes"));
            WindfuryTotemImp = 0;
            WindfuryTotem = WildStrikes || Buffs.Any(b => b.Name.Contains("Windfury Totem"));
            if (WindfuryTotem && !WildStrikes)
            {
                string[] split = Buffs.Where(b => b.Name.Contains("Windfury Totem")).First().Name.Split('*');
                if (!int.TryParse(split.Last(), out WindfuryTotemImp)) WindfuryTotemImp = 0;
            }

            if (NbSet("Wastewalker") >= 2) Attributes.AddToValue(Attribute.HitChance, 35 / RatingRatios[Attribute.HitChance] / 100);
            if (NbSet("Felsteel") >= 3) Attributes.AddToValue(Attribute.Strength, 25);
            if (NbSet("Ragesteel") >= 2) Attributes.AddToValue(Attribute.HitChance, 20 / RatingRatios[Attribute.HitChance] / 100);
            if (NbSet("Doomplate") >= 2) Attributes.AddToValue(Attribute.HitChance, 35 / RatingRatios[Attribute.HitChance] / 100);
            if (NbSet("Desolation") >= 2) Attributes.AddToValue(Attribute.HitChance, 35 / RatingRatios[Attribute.HitChance] / 100);

            if (Class == Classes.Druid)
            {
                Attributes.SetValue(Attribute.Intellect, Attributes.GetValue(Attribute.Intellect)
                    * (1 + 0.04 * GetTalentPoints("HW")));
                if (Program.version == Version.Vanilla && !Tanking) Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength) * (1 + 0.04 * GetTalentPoints("HW")));
                Attributes.AddToValue(Attribute.CritChance, 0.01 * GetTalentPoints("SC"));
                Attributes.AddToValue(Attribute.AP, 0.5 * GetTalentPoints("PS") * Level);

                if (Tanking)
                {
                    Form = Forms.Bear;
                    ThreatMod *= 1.3 * (1 + 0.03 * GetTalentPoints("FI"));  // Bear form
                }
                else
                {
                    Form = Forms.Cat;
                    Resource = MaxResource;
                    ThreatMod *= 0.8;   // Cat form
                }
            }
            else if (Class == Classes.Mage)
            {
                Attributes.AddToValue(Attribute.SpellCritChance, 0.01 * GetTalentPoints("AI"));
                DamageMod *= 1 + 0.01 * GetTalentPoints("AI");
                CastingManaRegenRate += 0.05 * GetTalentPoints("AMed");
            }
            else if (Class == Classes.Priest)
            {
                CastingManaRegenRate += 0.05 * GetTalentPoints("Med");
            }
            else if (Class == Classes.Rogue)
            {
                Attributes.AddToValue(Attribute.HitChance, GetTalentPoints("Prec") / 100.0);
                Attributes.AddToValue(Attribute.CritChance, GetTalentPoints("Malice") / 100.0);
                Attributes.AddToValue(Attribute.CritChance, GetTalentPoints("FS") / 100.0);
                Attributes.AddToValue(Attribute.CritChance, GetTalentPoints("DS") / 100.0);

                if(Program.version == Version.Vanilla)
                {
                    Attributes.AddToValue(Attribute.SkillSword, GetTalentPoints("WE") > 0 ? (GetTalentPoints("WE") == 1 ? 3 : 5) : 0);
                    Attributes.AddToValue(Attribute.SkillDagger, GetTalentPoints("WE") > 0 ? (GetTalentPoints("WE") == 1 ? 3 : 5) : 0);
                }
                else
                {
                    Attributes.AddToValue(Attribute.Expertise, GetTalentPoints("WE") * 5);

                    Attributes.SetValue(Attribute.Agility, Attributes.GetValue(Attribute.Agility)
                        * (1 + 0.01 * GetTalentPoints("Vitality")));
                }

                if (MH.Type == Weapon.WeaponType.Fist) MH.Buff.Attributes.AddToValue(Attribute.CritChance, 0.01 * GetTalentPoints("Fist"));
                if (DualWielding && OH.Type == Weapon.WeaponType.Fist) OH.Buff.Attributes.SetValue(Attribute.CritChance, 0.01 * GetTalentPoints("Fist"));
            }
            else if(Class == Classes.Warlock)
            {
                Attributes.AddToValue(Attribute.Stamina, 0.03 * GetTalentPoints("DE"));
                Attributes.AddToValue(Attribute.Spirit, -0.01 * GetTalentPoints("DE"));

                if(!Cooldowns.Contains("Demonic Sacrifice"))
                {
                    DamageMod *= (1 + 0.03 * GetTalentPoints("SL"))
                                * (Pet.Name == "Succubus" ? 1 + 0.02 * GetTalentPoints("MD") : 1);
                    ThreatMod *= Pet.Name == "Imp" ? 1 - 0.04 * GetTalentPoints("MD") : 1;
                }

                if (Runes.Contains("Demonic Tactics"))
                {
                    SpellCritChance += 0.1;
                    CritChance += 0.1;
                }

                if (Tanking && Runes.Contains("Metamorphosis"))
                {
                    ThreatMod *= 1.5;   // Meta 50% threat increase
                }
            }
            else if (Class == Classes.Warrior)
            {
                Attributes.AddToValue(Attribute.CritChance, 0.01 * GetTalentPoints("Cruelty")
                    + (Tanking ? 0 : 0.03)); // Berserker Stance, should be a spell for stance dancing

                Attributes.AddToValue(Attribute.HitChance, 0.01 * GetTalentPoints("Precision"));

                Attributes.AddToValue(Attribute.Expertise, 0.01 * GetTalentPoints("WM"));

                Attributes.SetValue(Attribute.Stamina, Attributes.GetValue(Attribute.Stamina)
                    * (1 + 0.01 * GetTalentPoints("Vitality")));

                Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength)
                    * (1 + 0.02 * GetTalentPoints("Vitality")));

                if(Program.version == Version.TBC)
                {
                    Attributes.AddToValue(Attribute.Expertise, 0.005 * GetTalentPoints("Defiance"));

                    if (MH.Type == Weapon.WeaponType.Axe || MH.Type == Weapon.WeaponType.Polearm) MH.Buff.Attributes.AddToValue(Attribute.CritChance, 0.01 * GetTalentPoints("Poleaxe"));
                    if (DualWielding && (OH.Type == Weapon.WeaponType.Axe || MH.Type == Weapon.WeaponType.Polearm)) OH.Buff.Attributes.SetValue(Attribute.CritChance, 0.01 * GetTalentPoints("Poleaxe"));
                }

                if (Tanking && Program.jsonSim.TankHitRage > 0 && Program.jsonSim.TankHitEvery > 0)
                {
                    DamageMod *= 0.9;
                    ThreatMod *= 1.3 * (1 + 0.03 * GetTalentPoints("Defiance"));
                }
                else
                {
                    ThreatMod *= 0.8 * (1 - 0.02 * GetTalentPoints("IBS"));
                }
            }

            if(Buffs.Any(b => b.Name.ToLower().Equals("blessing of salvation")))
            {
                ThreatMod *= 0.7;
            }

            Attributes += BonusAttributesByRace(Race, Attributes);

            if (Buffs.Any(b => b.Name.ToLower().Equals("blessing of kings")))
            {
                Attributes.SetValue(Attribute.Stamina, Attributes.GetValue(Attribute.Stamina) * 1.1);
                Attributes.SetValue(Attribute.Intellect, Attributes.GetValue(Attribute.Intellect) * 1.1);
                Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength) * 1.1);
                Attributes.SetValue(Attribute.Agility, Attributes.GetValue(Attribute.Agility) * 1.1);
                Attributes.SetValue(Attribute.Spirit, Attributes.GetValue(Attribute.Spirit) * 1.1);
            }

            Attributes.AddToValue(Attribute.Health, 20 + (Attributes.GetValue(Attribute.Stamina) - 20) * 10);
            if (Attributes.GetValue(Attribute.Mana) > 0)
            {
                Attributes.AddToValue(Attribute.Mana, Attributes.GetValue(Attribute.Intellect) * 15);
                if (Class == Classes.Mage)
                {
                    Attributes.SetValue(Attribute.Mana, Attributes.GetValue(Attribute.Mana) * 1 + (0.02 * GetTalentPoints("AMind")));
                }
                if (Class == Classes.Priest)
                {
                    Attributes.SetValue(Attribute.Mana, Attributes.GetValue(Attribute.Mana) * 1 + (0.02 * GetTalentPoints("MS")));
                }
            }
            Attributes.AddToValue(Attribute.AP, Attributes.GetValue(Attribute.Strength) * StrToAPRatio(Class) + Attributes.GetValue(Attribute.Agility) * AgiToAPRatio(this));
            Attributes.AddToValue(Attribute.RangedAP, Attributes.GetValue(Attribute.Agility) * AgiToRangedAPRatio(Class));
            Attributes.AddToValue(Attribute.CritChance, Attributes.GetValue(Attribute.Agility) * AgiToCritRatio(Class, Level));
            Attributes.AddToValue(Attribute.SpellCritChance, BaseSpellCrit(Class, Level) + Attributes.GetValue(Attribute.Intellect) * IntToCritRatio(Class, Level));

            if (Class == Classes.Druid && Program.version == Version.TBC && Form != Forms.Bear) Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP) * (1 + 0.02 * GetTalentPoints("HW")));
            if (Class == Classes.Warrior && Program.version == Version.TBC) Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP) * (1 + 0.02 * GetTalentPoints("IBStance")));

            if (Buffs.Any(b => b.Name.ToLower().Equals("unleashed rage"))) Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP) * 1.06);
            if (Buffs.Any(b => b.Name.ToLower().Contains("ferocious inspiration")))
            {
                string name = Buffs.Where(b => b.Name.ToLower().Contains("ferocious inspiration")).First().Name;
                string[] split = name.Split('*');
                int nb;
                if (!int.TryParse(split.Last(), out nb)) nb = 0;
                DamageMod *= 1 + (0.03 * nb);
            }
            if (Buffs.Any(b => b.Name.ToLower().Equals("improved sanctity aura"))) DamageMod *= 1.02;

            if (Program.jsonSim.Boss.Debuffs.Any(b => b.Key.ToLower().Equals("improved faerie fire"))) Attributes.AddToValue(Attribute.HitChance, 0.03);

            int baseSkill = Level * 5;
            WeaponSkill = new Dictionary<Weapon.WeaponType, int>();
            foreach (Weapon.WeaponType type in (Weapon.WeaponType[])Enum.GetValues(typeof(Weapon.WeaponType)))
            {
                int skill = baseSkill;

                if(Program.version == Version.Vanilla)
                {
                    if (Race == Races.Orc && type == Weapon.WeaponType.Axe)
                    {
                        skill += 5;
                    }
                    else if (Race == Races.Human && (type == Weapon.WeaponType.Mace || type == Weapon.WeaponType.Sword))
                    {
                        skill += 5;
                    }
                    else if (Race == Races.Troll && (type == Weapon.WeaponType.Bow || type == Weapon.WeaponType.Throwable))
                    {
                        skill += 5;
                    }
                    else if (Race == Races.Dwarf && type == Weapon.WeaponType.Gun)
                    {
                        skill += 5;
                    }
                }

                WeaponSkill[type] = skill;
            }

            WeaponSkill[Weapon.WeaponType.Axe] += (int)Attributes.GetValue(Attribute.SkillAxe) + (int)Attributes.GetValue(Attribute.Skill1H) + (int)Attributes.GetValue(Attribute.Skill2H);
            WeaponSkill[Weapon.WeaponType.Bow] += (int)Attributes.GetValue(Attribute.SkillBow);
            WeaponSkill[Weapon.WeaponType.Crossbow] += (int)Attributes.GetValue(Attribute.SkillCrossbow);
            WeaponSkill[Weapon.WeaponType.Dagger] += (int)Attributes.GetValue(Attribute.SkillDagger) + (int)Attributes.GetValue(Attribute.Skill1H);
            WeaponSkill[Weapon.WeaponType.Fist] += (int)Attributes.GetValue(Attribute.SkillFist) + (int)Attributes.GetValue(Attribute.Skill1H);
            WeaponSkill[Weapon.WeaponType.Gun] += (int)Attributes.GetValue(Attribute.SkillGun);
            WeaponSkill[Weapon.WeaponType.Mace] += (int)Attributes.GetValue(Attribute.SkillMace) + (int)Attributes.GetValue(Attribute.Skill1H) + (int)Attributes.GetValue(Attribute.Skill2H);
            WeaponSkill[Weapon.WeaponType.Polearm] += (int)Attributes.GetValue(Attribute.SkillPolearm) + (int)Attributes.GetValue(Attribute.Skill2H);
            WeaponSkill[Weapon.WeaponType.Staff] += (int)Attributes.GetValue(Attribute.SkillStaff) + (int)Attributes.GetValue(Attribute.Skill2H);
            WeaponSkill[Weapon.WeaponType.Sword] += (int)Attributes.GetValue(Attribute.SkillSword) + (int)Attributes.GetValue(Attribute.Skill1H) + (int)Attributes.GetValue(Attribute.Skill2H);
            WeaponSkill[Weapon.WeaponType.Throwable] += (int)Attributes.GetValue(Attribute.SkillThrowable);

            if(Program.version == Version.TBC)
            {
                if (Race == Races.Orc)
                {
                    if (MH.Type == Weapon.WeaponType.Axe) MH.Buff.Attributes.AddToValue(Attribute.Expertise, 0.0125);
                    if (DualWielding && OH.Type == Weapon.WeaponType.Axe) OH.Buff.Attributes.SetValue(Attribute.Expertise, 0.0125);
                }
                else if (Race == Races.Human)
                {
                    if (MH.Type == Weapon.WeaponType.Sword || MH.Type == Weapon.WeaponType.Mace) MH.Buff.Attributes.AddToValue(Attribute.Expertise, 0.0125);
                    if (DualWielding && OH.Type == Weapon.WeaponType.Sword || MH.Type == Weapon.WeaponType.Mace) OH.Buff.Attributes.AddToValue(Attribute.Expertise, 0.0125);
                }
                else if (Race == Races.Troll)
                {
                    if (Ranged.Type == Weapon.WeaponType.Bow || MH.Type == Weapon.WeaponType.Throwable) Attributes.AddToValue(Attribute.CritChance, 0.0125);
                }
                else if (Race == Races.Dwarf)
                {
                    if (Ranged.Type == Weapon.WeaponType.Gun) Attributes.AddToValue(Attribute.CritChance, 0.01);
                }
            }

            HasteMod = CalcHaste();
        }

        public void ExtraAA(List<string> alreadyProc)
        {
            if(Program.version == Version.Vanilla)
            {
                if (applyAtNextAA != null)
                {
                    applyAtNextAA.DoAction();
                }
                else
                {
                    mh.DoAA(alreadyProc, true);
                }
                ResetMHSwing();
            }
            else
            {
                mh.DoAA(alreadyProc, true);
            }
        }

        public double BonusHit(string name, Entity target, School school)
        {
            double res = 0;

            if(Class == Classes.Priest)
            {
                res += school == School.Shadow ? 0.02 * GetTalentPoints("SF") : 0;
            }
            else if(Class == Classes.Warlock)
            {
                res += new List<string>() { Corruption.NAME, CurseOfAgony.NAME, DrainLife.NAME }.Contains(name) ? 0.02 * GetTalentPoints("Suppr") : 0;
            }

            return res;
        }

        public double BonusCrit(string name, Entity target, School school)
        {
            double res = 0;

            return res;
        }

        public double TotalModifiers(string name, Entity target, School school, ResultType res)
        {
            return SelfModifiers(name, target, school, res) * ExternalModifiers(name, target, school, res);
        }

        public double SelfModifiers(string name, Entity target, School school, ResultType res)
        {
            double ratio = 1;

            ratio *= ((name == DeepWounds.NAME || name == Rip.NAME || name == Shred.NAME) && target.Effects.ContainsKey("Mangle") ? 1.3 : 1)
                ;

            if(Class == Classes.Druid)
            {
                ratio *= (school == School.Physical ? 1 + 0.02 * GetTalentPoints("NW") : 1)
                    * (school == School.Physical && Effects.ContainsKey(SavageRoar.NAME) ? 1.3 : 1)
                    ;
            }
            else if(Class == Classes.Priest)
            {

                ratio *= (school == School.Shadow ? 1 + 0.15 * GetTalentPoints("Form") : 1)
                    * (school == School.Shadow ? 1 + 0.02 * GetTalentPoints("Darkness") : 1)
                    ;
            }
            else if (Class == Classes.Rogue)
            {
                ratio *= (new List<string>() { DeadlyPoisonDoT.NAME }.Contains(name) ? 1 + 0.04 * GetTalentPoints("VP") : 1)
                    ;
            }
            else if (Class == Classes.Warlock)
            {
                ratio *= (school == School.Fire ? 1 + 0.02 * GetTalentPoints("Emberstorm") : 1)
                    * (res == ResultType.Crit ? 1 + (0.5 / 1.5) * GetTalentPoints("Ruin") : 1)
                    * ((school == School.Fire && GetTalentPoints("DS") > 0 && Cooldowns.Contains("Demonic Sacrifice") && Pet.Name == "Imp") ? 1.15 : 1)
                    * ((school == School.Shadow && GetTalentPoints("DS") > 0 && Cooldowns.Contains("Demonic Sacrifice") && Pet.Name == "Succubus") ? 1.15 : 1)
                    * (school == School.Shadow ? 1 + 0.02 * GetTalentPoints("SM") : 1)
                    ;
            }
            else if(Class == Classes.Warrior)
            {
                ratio *= (DualWielding ? 1 : (1 + 0.01 * GetTalentPoints("2HS")))
                    * (Program.version == Version.TBC && !MH.TwoHanded ? 1 + 0.02 * GetTalentPoints("1HS") : 1)
                    ;
            }

            return ratio;
        }

        public double ExternalModifiers(string name, Entity target, School school, ResultType res)
        {
            return (school == School.Fire && target.Effects.ContainsKey("Improved Scorch") ? 1.15 : 1)
                * (school == School.Fire && target.Effects.ContainsKey("Lake of Fire") ? 1.40 : 1)
                * (school == School.Fire && target.Effects.ContainsKey("Incinerate") ? 1.25 : 1)
                * (new List<School>() { School.Fire, School.Frost }.Contains(school) && target.Effects.ContainsKey("Curse of the Elements") ? Level >= 60 ? 1.10 : (Level >= 46 ? 1.08 : (Level >= 32 ? 1.06 : 1)) : 1)
                * (school == School.Shadow && target.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                * (school == School.Shadow && target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ShadowVulnerability.Modifier : 1)
                * (new List<string>() { Shred.NAME, RuptureDoT.NAME, DeepWounds.NAME }.Contains(name) && Target.Effects.ContainsKey("Mangle") ? 1.3 : 1)
                * (school == School.Physical && target.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                ;
        }

        public void CheckOnSpell(Spell spell, ResultType res)
        {
            if(spell.School == School.Shadow)
            {
                ShadowVulnerability.CheckProc(this, spell, res);
            }
        }

        public void CheckOnTick(EffectOnTime spell)
        {
            if(Class == Classes.Warlock)
            {
                if (GetTalentPoints("NF") > 0)
                {
                    ShadowTrance.CheckProc(this);
                }
            }
        }

        public void CheckOnHits(bool isMH, bool isAA, ResultType res, bool extra = false, List<string> alreadyProc = null, Action action = null)
        {
            if(alreadyProc == null)
            {
                alreadyProc = new List<string>();
            }

            Weapon w = isMH ? MH : OH;

            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                if (Class == Classes.Warrior)
                {
                    if (GetTalentPoints("DW") > 0)
                    {
                        DeepWounds.CheckProc(this, res);
                    }
                    if (GetTalentPoints("Flurry") > 0)
                    {
                        Flurry.CheckProc(this, res, extra || !isAA);
                    }
                    if (isAA && GetTalentPoints("UW") > 0)
                    {
                        UnbridledWrath.CheckProc(this, res, GetTalentPoints("UW"), isMH ? MH.Speed : OH.Speed);
                    }
                    if (Program.version == Version.TBC)
                    {
                        if (Effects.ContainsKey(RampageBuff.NAME))
                        {
                            RampageBuff.CheckProc(this, res, isMH ? MH.Speed : OH.Speed);
                        }

                        if (!alreadyProc.Contains("Mace")
                            && GetTalentPoints("Mace") > 0
                            && Randomer.NextDouble() < w.Speed * (0.3 * GetTalentPoints("Mace")) / 60)
                        {
                            alreadyProc.Add("Mace");
                            
                            Resource += 7;

                            if (Program.logFight)
                            {
                                Program.Log(string.Format("{0:N2} : Mace specialization procs ({3} {1}/{2})", Sim.CurrentTime, Resource, MaxResource, "rage"));
                            }
                        }
                    }
                }
                else if (Class == Classes.Druid)
                {
                    if (GetTalentPoints("OC") > 0)
                    {
                        ClearCasting.CheckProc(this);
                    }
                    if(res == ResultType.Crit && GetTalentPoints("SF") > 0)
                    {
                        if(GetTalentPoints("SF") > 1 || Randomer.NextDouble() < 0.5)
                        {
                            Resource += 5;
                        }
                    }
                }
                else if (Class == Classes.Rogue)
                {
                    if(new List<Entity.MobType>() { MobType.Humanoid, MobType.Giant, MobType.Beast, MobType.Dragonkin }.Contains(Sim.Boss[0].Type))
                    {
                        DamageMod *= 1 + 0.01 * GetTalentPoints("Murder");
                    }

                    if (!alreadyProc.Contains("rSP") && GetTalentPoints("SS") > 0 && Randomer.NextDouble() < 0.01 * GetTalentPoints("SS"))
                    {
                        alreadyProc.Add("rSP");
                        if (Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Sword Specialization procs", Sim.CurrentTime));
                        }
                        
                        switch (Program.version)
                        {
                            case Version.Vanilla:
                                ExtraAA(alreadyProc);
                                break;
                            case Version.TBC:
                                mh.DoAA(alreadyProc, true, true);
                                break;
                        }
                    }
                    if ((WindfuryTotem || !isMH)
                        && (w.Buff == null || w.Buff.Name == "Instant Poison")
                        && !alreadyProc.Contains("IP")
                        && (action is Shiv || Randomer.NextDouble() < (0.2 + 0.02 * GetTalentPoints("IP"))))
                    {
                        alreadyProc.Add("IP");
                        string procName = "Instant Poison";
                        int procDmg = Randomer.Next(112, 148 + 1);
                        if (!CustomActions.ContainsKey(procName))
                        {
                            CustomActions.Add(procName, new CustomAction(this, procName, School.Nature));
                        }

                        double mitigation = Simulation.MagicMitigation(Target.ResistChances[School.Nature]);
                        ResultType res2 = mitigation == 0 ? ResultType.Resist : SpellAttackEnemy(Target);
                        int dmg = (int)Math.Round(MiscDamageCalc(procDmg, res2, School.Nature) * mitigation);
                        CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)(dmg * ThreatMod)));
                    }
                    if ((WindfuryTotem || !isMH)
                        && w.Buff?.Name == "Deadly Poison"
                        && !alreadyProc.Contains("DP")
                        && (action is Shiv || Randomer.NextDouble() < (0.2 + 0.02 * GetTalentPoints("IP"))))
                    {
                        alreadyProc.Add("IP");
                        string procName = "Deadly Poison";
                        double mitigation = Simulation.MagicMitigation(Target.ResistChances[School.Nature]);
                        ResultType res2 = mitigation == 0 ? ResultType.Resist : ResultType.Hit;

                        if (!CustomActions.ContainsKey(procName))
                        {
                            CustomActions.Add(procName, new CustomAction(this, procName, School.Nature));
                        }
                        Sim.RegisterAction(new RegisteredAction(CustomActions[procName], new ActionResult(res2, 0, 0), Sim.CurrentTime));

                        if(res2 == ResultType.Hit)
                        {
                            if (Target.Effects.ContainsKey(DeadlyPoisonDoT.NAME))
                            {
                                Target.Effects[DeadlyPoisonDoT.NAME].StackAdd();
                                Target.Effects[DeadlyPoisonDoT.NAME].Refresh();
                            }
                            else
                            {
                                new DeadlyPoisonDoT(this, Target).StartEffect();
                            }
                        }
                    }
                    if (!alreadyProc.Contains("CdG") && NbSet("Deathmantle") >= 4
                        && Randomer.NextDouble() < w.Speed / 60     // TODO : 1 PPM ? Check proc-rate
                        )                                           // TODO : icd ?
                    {
                        string procName = "CdG";
                        alreadyProc.Add(procName);
                        if (Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Deathmantle 4P (Coup de Grace) procs", Sim.CurrentTime));
                        }

                        int procDuration = 15;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomEffect buff = new CustomEffect(this, this, procName, true, procDuration);
                            buff.StartEffect();
                        }
                    }

                    if (!isMH && !alreadyProc.Contains("CombatPotency") && GetTalentPoints("CombatPotency") > 0
                        && Randomer.NextDouble() < 0.2)
                    {
                        alreadyProc.Add("CombatPotency");

                        if (Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Combat Potency procs ({3} {1}/{2})", Sim.CurrentTime, Resource, MaxResource, "energy"));
                        }

                        Resource += 7;
                    }
                }
                else if(Class == Classes.Warlock)
                {
                    if (isMH
                        && OH.Name.Contains("Firestone")
                        && !alreadyProc.Contains("Firestone")
                        && (!icds.ContainsKey("Firestone") || icds["Firestone"] < Sim.CurrentTime - 5)  // ICD 5s ?
                        //&& Randomer.NextDouble() < MH.Speed * 10 / 60                                 // PPM 10 ?
                        )
                    {
                        string procName = "Firestone";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;

                        int min;
                        if (Level >= 56) min = 80;
                        else if (Level >= 46) min = 60;
                        else if (Level >= 36) min = 40;
                        else min = 25;

                        int max;
                        if (Level >= 56) max = 120;
                        else if (Level >= 46) max = 90;
                        else if (Level >= 36) max = 60;
                        else max = 35;

                        School school = School.Fire;

                        int procDmg = (int)(Randomer.Next(min, max + 1) * (1 + 0.15 * GetTalentPoints("IF")));
                        if (!CustomActions.ContainsKey(procName))
                        {
                            CustomActions.Add(procName, new CustomAction(this, procName, school));
                        }

                        double mitigation = Simulation.MagicMitigation(Target.ResistChances[school]);
                        ResultType res2 = mitigation == 0 ? ResultType.Resist : SpellAttackEnemy(Target);
                        int dmg = (int)Math.Round(MiscDamageCalc(procDmg, res2, school) * mitigation);
                        CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)(dmg * ThreatMod)));
                    }
                }

                if ((Form == Forms.Humanoid || Form == Forms.Metamorphosis) &&
                    !alreadyProc.Contains("crusader") &&
                    ((isMH && MH?.Enchantment?.Name.ToLower().Contains("crusader") == true) || (!isMH && OH?.Enchantment?.Name.ToLower().Contains("crusader") == true))
                    && Randomer.NextDouble() < (isMH ? MH.Speed : OH.Speed) / 60)
                {
                    string procName = "Crusader" + (isMH ? "MH" : "OH");
                    alreadyProc.Add(procName);
                    Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Strength, Program.version == Version.TBC ? 60 : 100 },
                        };
                    int procDuration = 15;
                        
                    if (Effects.ContainsKey(procName))
                    {
                        Effects[procName].Refresh();
                    }
                    else
                    {
                        CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                        buff.StartEffect();
                    }
                }

                if (isMH
                    && (WildStrikes || (WindfuryTotem && (Form == Forms.Humanoid || Form == Forms.Metamorphosis)))
                    && (Program.version == Version.Vanilla || isAA) && !alreadyProc.Contains("WF") && Randomer.NextDouble() < 0.2)
                {
                    alreadyProc.Add("WF");

                    if (Program.logFight)
                    {
                        Program.Log(string.Format("{0:N2} : Windfury procs", Sim.CurrentTime));
                    }

                    nextAABonus = (int)(WildStrikes ? AP * 0.2 : ((Program.version == Version.TBC ? 445 : 315) * (1 + 0.15 * WindfuryTotemImp)));

                    switch (Program.version)
                    {
                        case Version.Vanilla:
                            ExtraAA(alreadyProc);
                            break;
                        case Version.TBC:
                            mh.DoAA(alreadyProc, true, true);
                            break;
                    }
                }

                if(isAA && (Class == Classes.Rogue || Form == Forms.Cat))
                {
                    if ((NbSet("Shadowcraft") >= 6 || NbSet("Darkmantle") >= 4) && 
                        (Form == Forms.Cat && Randomer.NextDouble() < 1.0 / 60 ||
                        (Form != Forms.Cat && ((isMH && Randomer.NextDouble() < MH.Speed / 60) || (!isMH && Randomer.NextDouble() < OH.Speed / 60)))))
                    {
                        Resource += 35;
                        if(Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Shadowcraft 6P procs (energy {1}/{2})", Sim.CurrentTime, Resource, MaxResource));
                        }
                    }
                }

                if (!alreadyProc.Contains("Sword") && GetTalentPoints("Sword") > 0 && Randomer.NextDouble() < 0.01 * GetTalentPoints("Sword"))
                {
                    alreadyProc.Add("Sword");

                    if (Program.logFight)
                    {
                        Program.Log(string.Format("{0:N2} : Sword Specialization procs", Sim.CurrentTime));
                    }

                    switch (Program.version)
                    {
                        case Version.Vanilla:
                            ExtraAA(alreadyProc);
                            break;
                        case Version.TBC:
                            mh.DoAA(alreadyProc, true, true);
                            break;
                    }
                }
                if ((Equipment[Slot.Trinket1]?.Name == "Hand of Justice" || Equipment[Slot.Trinket2]?.Name == "Hand of Justice") && !alreadyProc.Contains("HoJ") && Randomer.NextDouble() < (Program.version == Version.Vanilla ? 0.02 : 0.012))
                {
                    alreadyProc.Add("HoJ");
                    if (Program.logFight)
                    {
                        Program.Log(string.Format("{0:N2} : Hand of Justice procs", Sim.CurrentTime));
                    }
                    ExtraAA(alreadyProc);
                }
                if (((isMH && MH?.Name == "Flurry Axe") || (!isMH && OH?.Name == "Flurry Axe")) && !alreadyProc.Contains("FA") && Randomer.NextDouble() < 0.0466)
                {
                    alreadyProc.Add("FA");
                    if (Program.logFight)
                    {
                        Program.Log(string.Format("{0:N2} : Flurry Axe procs", Sim.CurrentTime));
                    }
                    ExtraAA(alreadyProc);
                }
                if (((isMH && MH?.Name == "Thrash Blade") || (!isMH && OH?.Name == "Thrash Blade")) && !alreadyProc.Contains("TB") && Randomer.NextDouble() < 0.044)
                {
                    alreadyProc.Add("TB");
                    if (Program.logFight)
                    {
                        Program.Log(string.Format("{0:N2} : Thrash Blade procs", Sim.CurrentTime));
                    }
                    ExtraAA(alreadyProc);
                }
                if (((isMH && MH?.Name == "Deathbringer") || (!isMH && OH?.Name == "Deathbringer")) && !alreadyProc.Contains("DB") && Randomer.NextDouble() < 0.0396)
                {
                    alreadyProc.Add("DB");
                    string procName = "Deathbringer";
                    int procDmg = Randomer.Next(110, 140 + 1);
                    if (!CustomActions.ContainsKey(procName))
                    {
                        CustomActions.Add(procName, new CustomAction(this, procName, School.Shadow));
                    }

                    double mitigation = Simulation.MagicMitigation(Target.ResistChances[School.Shadow]);
                    ResultType res2 = mitigation == 0 ? ResultType.Resist : SpellAttackEnemy(Target);
                    int dmg = (int)Math.Round(MiscDamageCalc(procDmg, res2, School.Shadow) * mitigation);
                    CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)Math.Round(dmg * ThreatMod)));
                }
                if (((isMH && MH?.Name == "Perdition's Blade") || (!isMH && OH?.Name == "Perdition's Blade")) && !alreadyProc.Contains("PB") && Randomer.NextDouble() < 0.04)
                {
                    alreadyProc.Add("PB");
                    string procName = "Perdition's Blade";
                    int procDmg = Randomer.Next(40, 56 + 1);
                    if (!CustomActions.ContainsKey(procName))
                    {
                        CustomActions.Add(procName, new CustomAction(this, procName, School.Fire));
                    }

                    double mitigation = Simulation.MagicMitigation(Target.ResistChances[School.Fire]);
                    ResultType res2 = mitigation == 0 ? ResultType.Resist : SpellAttackEnemy(Target);
                    int dmg = (int)Math.Round(MiscDamageCalc(procDmg, res2, School.Fire) * mitigation);
                    CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)Math.Round(dmg * ThreatMod)));
                }
                if (((isMH && MH?.Name == "Vis'kag the Bloodletter") || (!isMH && OH?.Name == "Vis'kag the Bloodletter")) && !alreadyProc.Contains("VtB") && Randomer.NextDouble() < 0.0253)
                {
                    alreadyProc.Add("VtB");
                    string procName = "Vis'kag the Bloodletter";
                    int procDmg = 240;
                    if (!CustomActions.ContainsKey(procName))
                    {
                        CustomActions.Add(procName, new CustomAction(this, procName));
                    }

                    ResultType res2 = YellowAttackEnemy(Target);
                    int dmg = MiscDamageCalc(procDmg, res2);
                    CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)Math.Round(dmg * ThreatMod)));
                }
                if (((isMH && MH?.Name.ToLower().Contains("thunderfury") == true) || (!isMH && OH?.Name.ToLower().Contains("thunderfury") == true)) && !alreadyProc.Contains("tf") && Randomer.NextDouble() < 0.1917)
                {
                    alreadyProc.Add("tf");
                    string procName = "Thunderfury";
                    int procDmg = 300;
                    int bonusThreat = Simulation.MagicMitigationBinary(Target.MagicResist[School.Nature]) == ResultType.Hit && SpellAttackEnemy(Target, false) == ResultType.Hit ? 235 : 0;
                    if (!CustomActions.ContainsKey(procName))
                    {
                        CustomActions.Add(procName, new CustomAction(this, procName, School.Nature));
                    }

                    double mitigation = Simulation.MagicMitigation(Target.ResistChances[School.Nature]);
                    ResultType res2 = mitigation == 0 ? ResultType.Resist : SpellAttackEnemy(Target);
                    int dmg = (int)Math.Round(MiscDamageCalc(procDmg, res2, School.Nature) * mitigation);
                    CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)Math.Round((dmg + bonusThreat) * ThreatMod)));
                    for(int i = 1; i < Sim.NbTargets; i++)
                    {
                        CustomActions[procName].RegisterDamage(new ActionResult(res2, 0, (int)Math.Round(bonusThreat * ThreatMod)));
                    }
                }

                if(Program.version == Version.TBC)
                {
                    if (Form == Forms.Humanoid)
                    {
                        if (!alreadyProc.Contains("mongoose")
                            && ((isMH && MH?.Enchantment?.Name.ToLower().Contains("mongoose") == true) || (!isMH && OH?.Enchantment?.Name.ToLower().Contains("mongoose") == true))
                            && Randomer.NextDouble() < (isMH ? MH.Speed : OH.Speed) / 60)
                        {
                            string procName = "Mongoose" + (isMH ? "MH" : "OH");
                            alreadyProc.Add(procName);
                            Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                            {
                                { Attribute.Agility, 120 },
                                { Attribute.Haste, 0.02 },
                            };
                            int procDuration = 15;

                            if (Effects.ContainsKey(procName))
                            {
                                Effects[procName].Refresh();
                            }
                            else
                            {
                                CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                                buff.StartEffect();
                            }
                        }
                        if (!alreadyProc.Contains("executioner")
                            && ((isMH && MH?.Enchantment?.Name.ToLower().Contains("executioner") == true) || (!isMH && OH?.Enchantment?.Name.ToLower().Contains("executioner") == true))
                            && (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
                            && Randomer.NextDouble() < (isMH ? MH.Speed : OH.Speed) / 60)
                        {
                            string procName = "Executioner" + (isMH ? "MH" : "OH");
                            alreadyProc.Add(procName);
                            Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                            {
                                { Attribute.ArmorPen, 840 },
                            };
                            int procDuration = 15;

                            if (Effects.ContainsKey(procName))
                            {
                                Effects[procName].Refresh();
                            }
                            else
                            {
                                CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                                buff.StartEffect();
                            }
                        }
                    }

                    if (!alreadyProc.Contains("Thundering Skyfire Diamond")
                        && Buffs.Any(b => b.Name.ToLower().Contains("thundering"))
                        && (!icds.ContainsKey("Thundering Skyfire Diamond") || icds["Thundering Skyfire Diamond"] < Sim.CurrentTime - 60)
                        && Randomer.NextDouble() < MH.Speed * 0.8 / 60)   // TODO : Check proc-rate + ICD
                    {
                        string procName = "Thundering Skyfire Diamond";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Haste, 240 / RatingRatios[Attribute.Haste] / 100 }
                        };
                        int procDuration = 6;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }

                    if (!alreadyProc.Contains("Warglaives")
                        && (MH?.Name.ToLower().Contains("warglaive") == true && OH?.Name.ToLower().Contains("warglaive") == true)
                        && (!icds.ContainsKey("Warglaives") || icds["Warglaives"] < Sim.CurrentTime - 45)
                        && Randomer.NextDouble() < MH.Speed * 2 / 60)   // TODO : Check proc-rate (2PPM on Mangos) 
                    {
                        string procName = "Warglaives";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Haste, 450 / RatingRatios[Attribute.Haste] / 100 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("LHC")
                        && isMH && (MH?.Name.ToLower().Contains("lionheart champion") == true || MH?.Name.ToLower().Contains("lionheart executioner") == true || MH?.Name.ToLower().Contains("lhc") == true) && Randomer.NextDouble() < 0.06)
                    {
                        string procName = "LHC";
                        alreadyProc.Add(procName);
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Strength, 100 }
                        };
                        int procDuration = 10;
                        
                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("dragonstrike")
                        && (w.Name.ToLower().Contains("dragonstrike") || w.Name.ToLower().Contains("dragonmaw") || w.Name.ToLower().Contains("drakefist"))
                        && Randomer.NextDouble() < 0.045)
                    {
                        string procName = "Dragonstrike";
                        alreadyProc.Add(procName);

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Haste, 212 / RatingRatios[Attribute.Haste] / 100 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("blackout")
                        && w.Name.ToLower().Contains("blackout")
                        && Randomer.NextDouble() < 0.025)       // TODO : Check 1PPM
                    {
                        string procName = "Blackout Truncheon";
                        alreadyProc.Add(procName);

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Haste, 132 / RatingRatios[Attribute.Haste] / 100 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("bladefist")
                        && w.Name.ToLower().Contains("bladefist")
                        && Randomer.NextDouble() < 0.045)
                    {
                        string procName = "Bladefist";
                        alreadyProc.Add(procName);

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Haste, 180 / RatingRatios[Attribute.Haste] / 100 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("Blinkstrike")
                        && w.Name.ToLower().Contains("blinkstrike")
                        && Randomer.NextDouble() < 0.066) // TODO : Check proc-rate
                    {
                        string procName = "Blinkstrike";
                        alreadyProc.Add(procName);
                        if (Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Blinkstrike procs", Sim.CurrentTime));
                        }
                        ExtraAA(alreadyProc);
                    }
                    if ((Class == Classes.Warrior || Class == Classes.Rogue)
                        && !alreadyProc.Contains("Rod of the Sun King")
                        && w.Name.ToLower().Contains("rod of the sun king")
                        && Randomer.NextDouble() < 0.05) // TODO : Check proc-rate
                    {
                        string procName = "Rod of the Sun King";
                        alreadyProc.Add(procName);

                        if (Class == Classes.Warrior) Resource += 5;
                        else if (Class == Classes.Rogue) Resource += 10;

                        if (Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Rod of the Sun King procs ({3} {1}/{2})", Sim.CurrentTime, Resource, MaxResource, Class == Classes.Warrior ? "rage" : "energy"));
                        }
                    }
                    if (!alreadyProc.Contains("Syphon of the Nathrezim")
                        && w.Name.ToLower().Contains("syphon of the nathrezim")
                        && Randomer.NextDouble() < w.Speed * 1 / 60) // TODO : Check proc-rate
                    {
                        string procName = "Syphon of the Nathrezim";
                        alreadyProc.Add(procName);
                        int procDuration = 6;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomEffect buff = new CustomEffect(this, this, procName, true, procDuration);
                            buff.StartEffect();
                        }
                    }
                    if (Effects.ContainsKey("Syphon of the Nathrezim"))
                    {
                        string procName = "Syphon of the Nathrezim";
                        if (!CustomActions.ContainsKey(procName))
                        {
                            CustomActions.Add(procName, new CustomAction(this, procName, School.Shadow));
                        }

                        double mitigation = Simulation.MagicMitigation(Target.ResistChances[School.Shadow]);
                        ResultType res2 = mitigation == 0 ? ResultType.Resist : SpellAttackEnemy(Target);
                        int dmg = (int)Math.Round(MiscDamageCalc(20, res2, School.Shadow) * mitigation);
                        CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)Math.Round(dmg * ThreatMod)));
                    }

                    if (!alreadyProc.Contains("Hourglass")
                        && res == ResultType.Crit
                        && (Equipment[Slot.Trinket1]?.Name.ToLower() == "hourglass of the unraveller" || Equipment[Slot.Trinket2]?.Name.ToLower() == "hourglass of the unraveller")
                        && (!icds.ContainsKey("Hourglass") || icds["Hourglass"] < Sim.CurrentTime - 50)
                        && Randomer.NextDouble() < 0.10)
                    {
                        string procName = "Hourglass";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, 300 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("Dragonspine Trophy")
                        && (Equipment[Slot.Trinket1]?.Name.ToLower() == "dragonspine trophy" || Equipment[Slot.Trinket2]?.Name.ToLower() == "dragonspine trophy"
                            || Equipment[Slot.Trinket1]?.Name.ToLower() == "dst" || Equipment[Slot.Trinket1]?.Name.ToLower() == "dst")
                        && (!icds.ContainsKey("Dragonspine Trophy") || icds["Dragonspine Trophy"] < Sim.CurrentTime - 20)
                        && Randomer.NextDouble() < w.Speed / 60)
                    {
                        string procName = "Dragonspine Trophy";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Haste, 325 / RatingRatios[Attribute.Haste] / 100 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("romulo's")
                        && w.Name.ToLower().Contains("romulo's")
                        && Randomer.NextDouble() < 0.02) // TODO : Check proc-rate
                    {
                        alreadyProc.Add("romulo's");
                        string procName = "Romulo's Poison Vial";
                        int procDmg = Randomer.Next(222, 332 + 1);
                        if (!CustomActions.ContainsKey(procName))
                        {
                            CustomActions.Add(procName, new CustomAction(this, procName, School.Nature));
                        }

                        double mitigation = Simulation.MagicMitigation(Target.ResistChances[School.Nature]);
                        ResultType res2 = mitigation == 0 ? ResultType.Resist : SpellAttackEnemy(Target);
                        int dmg = (int)Math.Round(MiscDamageCalc(procDmg, res2, School.Nature) * mitigation);
                        CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg, (int)(dmg * ThreatMod)));
                    }
                    if (!alreadyProc.Contains("Tsunami Talisman")
                        && res == ResultType.Crit
                        && (Equipment[Slot.Trinket1]?.Name.ToLower() == "tsunami talisman" || Equipment[Slot.Trinket2]?.Name.ToLower() == "tsunami talisman")
                        && (!icds.ContainsKey("Tsunami Talisman") || icds["Tsunami Talisman"] < Sim.CurrentTime - 45)
                        && Randomer.NextDouble() < 0.10)
                    {
                        string procName = "Tsunami Talisman";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, 340 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("Madness of the Betrayer")
                        && (Equipment[Slot.Trinket1]?.Name.ToLower() == "madness of the betrayer" || Equipment[Slot.Trinket2]?.Name.ToLower() == "madness of the betrayer")
                        && Randomer.NextDouble() < w.Speed * 1.5 / 60)  // TODO : Check proc-rate
                    {
                        string procName = "Madness of the Betrayer";
                        alreadyProc.Add(procName);
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.ArmorPen, 300 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("Shard of Contempt")
                        && (Equipment[Slot.Trinket1]?.Name.ToLower() == "shard of contempt" || Equipment[Slot.Trinket2]?.Name.ToLower() == "shard of contempt")
                        && (!icds.ContainsKey("Shard of Contempt") || icds["Shard of Contempt"] < Sim.CurrentTime - 45)
                        && Randomer.NextDouble() < 0.1)  // TODO : Check proc-rate
                    {
                        string procName = "Shard of Contempt";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, 230 }
                        };
                        int procDuration = 20;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("Blackened Naaru Sliver")
                        && (Equipment[Slot.Trinket1]?.Name.ToLower() == "blackened naaru sliver" || Equipment[Slot.Trinket2]?.Name.ToLower() == "blackened naaru sliver")
                        && (!icds.ContainsKey("Blackened Naaru Sliver") || Effects.ContainsKey("Blackened Naaru Sliver") || icds["Blackened Naaru Sliver"] < Sim.CurrentTime - 45)
                        && (Effects.ContainsKey("Blackened Naaru Sliver") || Randomer.NextDouble() < 0.1))
                    {
                        string procName = "Blackened Naaru Sliver";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, 44 }
                        };
                        int procDuration = 20;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].StackAdd();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes, 10);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("wastewalker")
                        && (NbSet("wastewalker") >= 4)
                        && Randomer.NextDouble() < 0.02)
                    {
                        string procName = "Wastewalker";
                        alreadyProc.Add(procName);
                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                            {
                                { Attribute.AP, 160 },
                            };
                        int procDuration = 15;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("Doomplate")
                        && NbSet("Doomplate") >= 4
                        && Randomer.NextDouble() < 0.02)
                    {
                        string procName = "Doomplate";
                        alreadyProc.Add(procName);

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, 160 }
                        };
                        int procDuration = 15;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                    if (!alreadyProc.Contains("Desolation")
                        && NbSet("Desolation") >= 4
                        && Randomer.NextDouble() < 0.01)
                    {
                        string procName = "Doomplate";
                        alreadyProc.Add(procName);

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, 160 }
                        };
                        int procDuration = 15;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }

                    if ((Equipment[Slot.Finger1]?.Name.ToLower() == "band of the eternal champion" || Equipment[Slot.Finger2]?.Name.ToLower() == "band of the eternal champion")
                        && (!icds.ContainsKey("Band of the Eternal Champion") || icds["Band of the Eternal Champion"] < Sim.CurrentTime - 60))
                    {
                        string procName = "Band of the Eternal Champion";
                        alreadyProc.Add(procName);
                        icds[procName] = Sim.CurrentTime;

                        Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, 160 }
                        };
                        int procDuration = 10;

                        if (Effects.ContainsKey(procName))
                        {
                            Effects[procName].Refresh();
                        }
                        else
                        {
                            CustomStatsBuff buff = new CustomStatsBuff(this, procName, procDuration, 1, attributes);
                            buff.StartEffect();
                        }
                    }
                }
            }
            else if(Class == Classes.Warrior && (res == ResultType.Parry || res == ResultType.Dodge) && NbSet("Warbringer") >= 4)
            {
                Resource += 2;
                if (Program.logFight)
                {
                    Program.Log(string.Format("{0:N2} : Warbringer 4P procs (rage {1}/{2})", Sim.CurrentTime, Resource, MaxResource));
                }
            }
        }

        public int MiscDamageCalc(int baseDmg, ResultType res, School school = School.Physical, double APRatio = 0)
        {
            if(school == School.Physical)
            {
                return (int)Math.Round(baseDmg
                    * Sim.DamageMod(res)
                    * Simulation.ArmorMitigation(Target.Armor, Level, Attributes.GetValue(Attribute.ArmorPen))
                    * DamageMod);
            }
            else
            {
                return (int)Math.Round((baseDmg + APRatio * AP)
                    * Sim.DamageMod(res)
                    * Simulation.MagicMitigation(Target.ResistChances[school])
                    * DamageMod);
            }
        }

        public void CalculateHitChances(Entity enemy = null)
        {
            if(enemy == null)
            {
                enemy = Target;
            }

            double MHParryExpertise = 0, OHParryExpertise = 0;

            if(Program.version == Version.TBC)
            {
                MHParryExpertise = Math.Max(0, ExpertisePercent + (MH.Buff == null ? 0 : MH.Buff.Attributes.GetValue(Attribute.Expertise)) - enemy.DodgeChance(WeaponSkill[MH.Type]));
                if(DualWielding) OHParryExpertise = Math.Max(0, ExpertisePercent + (OH.Buff == null ? 0 : OH.Buff.Attributes.GetValue(Attribute.Expertise)) - enemy.DodgeChance(WeaponSkill[OH.Type]));
            }

            double MHBonusCrit = (MH.Buff != null ? MH.Buff.Attributes.GetValue(Attribute.CritChance) : 0);
            double OHBonusCrit = (DualWielding && OH != null && OH.Buff != null ? OH.Buff.Attributes.GetValue(Attribute.CritChance) : 0);

            Dictionary<ResultType, double> whiteHitChancesMH = new Dictionary<ResultType, double>();
            whiteHitChancesMH.Add(ResultType.Miss, MissChance(DualWielding, HitChance, WeaponSkill[MH.Type], enemy.Level));
            whiteHitChancesMH.Add(ResultType.Dodge, Program.version == Version.TBC ? EnemyDodgeChance(enemy.DodgeChance(WeaponSkill[MH.Type]), ExpertisePercent + (MH.Buff == null ? 0 : MH.Buff.Attributes.GetValue(Attribute.Expertise))) : enemy.DodgeChance(WeaponSkill[MH.Type]));
            whiteHitChancesMH.Add(ResultType.Parry, EnemyParryChance(Level, WeaponSkill[MH.Type], enemy.Level, Facing, MHParryExpertise));
            whiteHitChancesMH.Add(ResultType.Glance, GlancingChance(Level, enemy.Level));
            whiteHitChancesMH.Add(ResultType.Block, enemy.BlockChance());
            whiteHitChancesMH.Add(ResultType.Crit, RealCritChance(CritWithSuppression(CritChance + MHBonusCrit, Level, enemy.Level), whiteHitChancesMH[ResultType.Miss], whiteHitChancesMH[ResultType.Glance], whiteHitChancesMH[ResultType.Dodge], whiteHitChancesMH[ResultType.Parry], whiteHitChancesMH[ResultType.Block]));
            whiteHitChancesMH.Add(ResultType.Hit, RealHitChance(whiteHitChancesMH[ResultType.Miss], whiteHitChancesMH[ResultType.Glance], whiteHitChancesMH[ResultType.Crit], whiteHitChancesMH[ResultType.Dodge], whiteHitChancesMH[ResultType.Parry], whiteHitChancesMH[ResultType.Block]));

            Dictionary<ResultType, double> whiteHitChancesOH = null;
            if (DualWielding)
            {
                whiteHitChancesOH = new Dictionary<ResultType, double>();
                whiteHitChancesOH.Add(ResultType.Miss, MissChance(true, HitChance + (OH.Buff == null ? 0 : OH.Buff.Attributes.GetValue(Attribute.HitChance)), WeaponSkill[OH.Type], enemy.Level));
                whiteHitChancesOH.Add(ResultType.Dodge, Program.version == Version.TBC ? EnemyDodgeChance(enemy.DodgeChance(WeaponSkill[OH.Type]), ExpertisePercent + (OH.Buff == null ? 0 : OH.Buff.Attributes.GetValue(Attribute.Expertise))) : enemy.DodgeChance(WeaponSkill[OH.Type]));
                whiteHitChancesOH.Add(ResultType.Parry, EnemyParryChance(Level, WeaponSkill[OH.Type], enemy.Level, Facing, OHParryExpertise));
                whiteHitChancesOH.Add(ResultType.Glance, GlancingChance(Level, enemy.Level));
                whiteHitChancesOH.Add(ResultType.Block, enemy.BlockChance());
                whiteHitChancesOH.Add(ResultType.Crit, RealCritChance(CritWithSuppression(CritChance + OHBonusCrit, Level, enemy.Level), whiteHitChancesOH[ResultType.Miss], whiteHitChancesOH[ResultType.Glance], whiteHitChancesOH[ResultType.Dodge], whiteHitChancesOH[ResultType.Parry], whiteHitChancesOH[ResultType.Block]));
                whiteHitChancesOH.Add(ResultType.Hit, RealHitChance(whiteHitChancesOH[ResultType.Miss], whiteHitChancesOH[ResultType.Glance], whiteHitChancesOH[ResultType.Crit], whiteHitChancesOH[ResultType.Dodge], whiteHitChancesOH[ResultType.Parry], whiteHitChancesOH[ResultType.Block]));
            }

            Dictionary<ResultType, double> yellowHitChances = new Dictionary<ResultType, double>();
            yellowHitChances.Add(ResultType.Miss, MissChanceYellow(HitChance, WeaponSkill[MH.Type], enemy.Level));
            yellowHitChances.Add(ResultType.Dodge, Program.version == Version.TBC ? EnemyDodgeChance(enemy.DodgeChance(WeaponSkill[MH.Type]), ExpertisePercent + (MH.Buff == null ? 0 : MH.Buff.Attributes.GetValue(Attribute.Expertise))) : enemy.DodgeChance(WeaponSkill[MH.Type]));
            yellowHitChances.Add(ResultType.Parry, EnemyParryChance(Level, WeaponSkill[MH.Type], enemy.Level, Facing, MHParryExpertise));
            yellowHitChances.Add(ResultType.Block, enemy.BlockChance());
            yellowHitChances.Add(ResultType.Crit, RealCritChance(CritWithSuppression(CritChance + MHBonusCrit, Level, enemy.Level), yellowHitChances[ResultType.Miss], 0, yellowHitChances[ResultType.Dodge], yellowHitChances[ResultType.Parry], yellowHitChances[ResultType.Block]));
            yellowHitChances.Add(ResultType.Hit, RealHitChance(yellowHitChances[ResultType.Miss], 0, yellowHitChances[ResultType.Crit], yellowHitChances[ResultType.Dodge], yellowHitChances[ResultType.Parry], yellowHitChances[ResultType.Block]));

            if (HitChancesByEnemy.ContainsKey(enemy))
            {
                HitChancesByEnemy[enemy] = new HitChances(whiteHitChancesMH, yellowHitChances, whiteHitChancesOH);
            }
            else
            {
                HitChancesByEnemy.Add(enemy, new HitChances(whiteHitChancesMH, yellowHitChances, whiteHitChancesOH));
            }
        }

        public void StartGCD()
        {
            GCDUntil = Sim.CurrentTime + GCD_Hasted();
        }

        public void StartGCD(double time = 1.5, bool wand = false)
        {
            GCDUntil = Sim.CurrentTime + Math.Max(1, (!wand && Program.version == Version.TBC ? HasteMod : 1) * time);
        }

        public bool HasGCD()
        {
            return GCDUntil <= Sim.CurrentTime;
        }

        public double GCD_Hasted()
        {
            return Math.Max(1, ((Class == Classes.Rogue || Form == Forms.Cat) ? GCD_ENERGY : GCD) / (Program.version == Version.TBC && IsCaster() ? HasteMod : 1));
        }

        public void CheckEnergyTick()
        {
            if (Sim.CurrentTime >= ResourcesTick + 2)
            {
                ResourcesTick = ResourcesTick + 2;
                Resource += 20 * (Effects.ContainsKey(AdrenalineRushBuff.NAME) ? 2 : 1);
                if (Program.logFight)
                {
                    Program.Log(string.Format("{0:N2} : Energy ticks ({1}/{2})", Sim.CurrentTime, Resource, MaxResource));
                }
            }
        }

        public double ManaPct()
        {
            return (double)Mana / MaxMana;
        }

        public bool SpiritTicking()
        {
            return CastingManaRegenRate > 0 || Sim.CurrentTime >= LastManaExpenditure + 5;
        }

        public void CheckSpiritTick()
        {
            if (SpiritTicking() && Sim.CurrentTime >= SpiritTick + 2)
            {
                int tick = (int)Math.Round(SpiritMPT() * (Sim.CurrentTime < LastManaExpenditure + 5 ? CastingManaRegenRate : 1) * MPTRatio);
                Mana += tick;
                SpiritTick = SpiritTick < LastManaExpenditure + 5 ? (CastingManaRegenRate > 0 ? Sim.CurrentTime : LastManaExpenditure + 5) : SpiritTick + 2;
            }
        }

        public void CheckMPT()
        {
            if (Sim.CurrentTime >= ManaTick + 2)
            {
                Mana += (int)Math.Round(MPT());
                ManaTick += 2;
            }
        }

        public double MPT()
        {
            return Attributes.GetValue(Attribute.MP5) * 2f / 5f;
        }

        public double SpiritMPT()
        {
            switch (Class)
            {
                case Classes.Warlock: return 8 + Attributes.GetValue(Attribute.Spirit) / 4;
                case Classes.Mage: return 13 + Attributes.GetValue(Attribute.Spirit) / 4;
                case Classes.Priest: return 13 + Attributes.GetValue(Attribute.Spirit) / 4;
                default: return 15 + Attributes.GetValue(Attribute.Spirit) / 5;

                    // TODO TBC
            }
        }

        public double CalcHaste()
        {
            return 1 + (Attributes != null ? Attributes.GetValue(Attribute.Haste) : 0);
        }

        public override double BlockChance()
        {
            return 0;
        }

        public string MainWeaponInfo()
        {
            Weapon.WeaponType type = Class == Classes.Druid ? Weapon.WeaponType.Fist : MH.Type;
            return string.Format("Main weapon : {0} with {1} skill", type, WeaponSkill[type]);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} level {2} | Talents : {3} | Stats : {4}", Race, Class, Level, TalentsStr, Attributes);
        }

        #endregion

        #region Attack tables

        public static double MissChance(bool dualWield, double hitRating, int skill, int enemyLevel)
        {
            double miss = MissChanceBase(hitRating, skill, enemyLevel);

            return Math.Max(0, dualWield ? miss * 0.8 + 0.2 : miss);
        }

        public static double GlancingChance(int level, int enemyLevel)
        {
            return Math.Max(0, 0.10 + (enemyLevel * 5 - level * 5) * 0.02);
        }

        public static double RealCritChance(double netCritChance, double realMiss, double realGlancing, double enemyDodgeChance, double enemyParryChance, double enemyBlockChance)
        {
            return Math.Max(0, Math.Min(netCritChance, 1 - realMiss - realGlancing - enemyDodgeChance - enemyParryChance - enemyBlockChance));
        }

        public static double RealHitChance(double realMiss, double realGlancing, double realCrit, double enemyDodgeChance, double enemyParryChance, double enemyBlockChance)
        {
            return Math.Max(0, 1 - realMiss - realGlancing - realCrit - enemyDodgeChance - enemyParryChance - enemyBlockChance);
        }
        
        public static double MissChanceBase(double hitRating, int skill, int enemyLevel)
        {
            int enemySkill = enemyLevel * 5;
            int skillDif = enemySkill - skill;
            
            return BASE_MISS - hitRating + (skillDif > 10 ? 0.01 : 0) + skillDif * (skillDif > 10 ? 0.002 : 0.001);
        }

        public static double MissChanceYellow(double hitRating, int skill, int enemyLevel)
        {
            return Math.Max(0, MissChanceBase(hitRating, skill, enemyLevel));
        }

        public static double SpellHitChanceReal(double spellHit, int level, int enemyLevel)
        {
            int skillDif = enemyLevel - level;
            double baseHit;
            if (skillDif < 3)
            {
                baseHit = Math.Min(1, 0.96 - 0.01 * skillDif);
            }
            else
            {
                baseHit = Math.Max(0, 0.83 - 0.11 * (skillDif - 3));
            }
            return Math.Max(0, Math.Min(0.99, baseHit + spellHit));
        }

        public static double RangedMagicHitChance(double hitRating, int skill, int enemyLevel)
        {
            return Math.Max(0, Math.Min(1, 1 - MissChanceBase(hitRating, skill, enemyLevel)));
        }

        public static double EnemyParryChance(int level, int skill, int enemyLevel, bool facing = false, double parryExpertise = 0)
        {
            int skillDif = enemyLevel * 5 - level * 5;
            return facing ? Math.Max(0, (skillDif > 10 ? 0.14 - (skill - level * 5) * 0.001 : 0.05 + (skill - enemyLevel * 5) * 0.001) - parryExpertise) : 0;
        }

        public static double EnemyDodgeChance(double baseDodge, double expertise = 0)
        {
            return Math.Max(0, baseDodge - expertise);
        }

        #endregion

        #region Attack methods

        public ResultType SpellAttackEnemy(Entity enemy, bool canCrit = true, double bonusHit = 0, double bonusCrit = 0)
        {
            if(Randomer.NextDouble() < SpellHitChanceReal(SpellHitChance + bonusHit, Level, enemy.Level))
            {
                if (canCrit && Randomer.NextDouble() < SpellCritChance + bonusCrit)
                {
                    return ResultType.Crit;
                }
                else
                {
                    return ResultType.Hit;
                }
            }
            else
            {
                return ResultType.Resist;
            }
        }

        public ResultType RangedMagicAttackEnemy(Entity enemy, bool canCrit = true, double bonusHit = 0, double bonusCrit = 0)
        {
            if (Randomer.NextDouble() < RangedMagicHitChance(HitChance + bonusHit, WeaponSkill[Weapon.WeaponType.Wand], enemy.Level))
            {
                if (canCrit && Randomer.NextDouble() < CritChance)
                {
                    return ResultType.Crit;
                }
                else
                {
                    return ResultType.Hit;
                }
            }
            else
            {
                return ResultType.Resist;
            }
        }

        public ResultType WhiteAttackEnemy(Entity enemy, bool MH)
        {
            if (!HitChancesByEnemy.ContainsKey(enemy))
            {
                CalculateHitChances(enemy);
            }

            var table = MH ? HitChancesByEnemy[enemy].WhiteHitChancesMH : HitChancesByEnemy[enemy].WhiteHitChancesOH;

            if (Class == Classes.Warrior && Effects.ContainsKey(RecklessnessBuff.NAME))
            {
                table = new Dictionary<ResultType, double>(table);
                table[ResultType.Crit] = 1;
            }
            else if (Class == Classes.Warlock && Effects.ContainsKey(DemonicGraceBuff.NAME))
            {
                table = new Dictionary<ResultType, double>(table);
                table[ResultType.Crit] += 0.3;
            }

            return PickFromTable(table, Randomer.NextDouble());
        }

        public ResultType YellowAttackEnemy(Entity enemy, string spell = "")
        {
            if (!HitChancesByEnemy.ContainsKey(enemy))
            {
                CalculateHitChances(enemy);
            }

            Dictionary<ResultType, double> table = HitChancesByEnemy[enemy].YellowHitChances;

            if (Class == Classes.Warrior && Effects.ContainsKey(RecklessnessBuff.NAME))
            {
                table = new Dictionary<ResultType, double>(table);
                table[ResultType.Crit] = 1;
            }
            else if (Class == Classes.Rogue)
            {
                if (spell.Equals("Backstab")) // TODO : Mutilate
                {
                    table = new Dictionary<ResultType, double>(table);
                    table[ResultType.Crit] += 0.1 * GetTalentPoints("IB");
                }
                if (GetTalentPoints("SA") > 0 && (spell.Equals("Eviscerate") || spell.Equals("Rupture")))
                {
                    table = new Dictionary<ResultType, double>(table);
                    table[ResultType.Dodge] = 0;
                }
            }
            else if(Class == Classes.Warlock && Effects.ContainsKey(DemonicGraceBuff.NAME))
            {
                table = new Dictionary<ResultType, double>(table);
                table[ResultType.Crit] += 0.3;
            }

            return PickFromTable(table, Randomer.NextDouble());
        }

        public ResultType PickFromTable(Dictionary<ResultType, double> pickTable, double roll)
        {            
            /*
            string debug = "rand " + rand;
            foreach (ResultType type in pickTable.Keys)
            {
                debug += " | " + type + " - " + table[type];
            }
            if(Program.logFight)
            {
                Program.Log(debug);
            }
            */

            double i = 0;
            foreach(ResultType type in ATTACKTABLE_ORDER)
            {
                if(pickTable.ContainsKey(type))
                {
                    i += pickTable[type];
                    if (roll <= i)
                    {
                        return type;
                    }
                }
            }

            throw new Exception("Hit Table Failed");
        }

        private void ParryHaste()
        {
            double remaining = mh.LockedUntil - Sim.CurrentTime;
            double speed = mh.CurrentSpeed();
            double newRemaining = Math.Max(0.2 * speed, remaining - 0.4 * speed);
            mh.LockedUntil = Sim.CurrentTime + newRemaining;
        }

        #endregion
    }
}

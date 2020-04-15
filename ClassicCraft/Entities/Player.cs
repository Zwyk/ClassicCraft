using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Player : Entity
    {
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
            Troll
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
            Wrist,
            Hands,
            Waist,
            Legs,
            Feet,
            Ring1,
            Ring2,
            Trinket1,
            Trinket2,
            MH,
            OH,
            Ranged
        }

        public enum Forms
        {
            Human,
            Bear,
            Cat,
        }

        public static Races ToRace(string s)
        {
            switch (s)
            {
                case "Human": return Races.Human;
                case "Dwarf": return Races.Dwarf;
                case "NightElf": return Races.NightElf;
                case "Gnome": return Races.Gnome;
                case "Orc": return Races.Orc;
                case "Undead": return Races.Undead;
                case "Tauren": return Races.Tauren;
                case "Troll": return Races.Troll;
                default: throw new Exception("Race not found");
            }
        }

        public static string FromRace(Races r)
        {
            switch (r)
            {
                case Races.Human: return "Human";
                case Races.Dwarf: return "Dwarf";
                case Races.NightElf: return "NightElf";
                case Races.Gnome: return "Gnome";
                case Races.Orc: return "Orc";
                case Races.Undead: return "Undead";
                case Races.Tauren: return "Tauren";
                case Races.Troll: return "Troll";
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
                case "Wrist": return Slot.Wrist;
                case "Hands": return Slot.Hands;
                case "Waist": return Slot.Waist;
                case "Legs": return Slot.Legs;
                case "Feet": return Slot.Feet;
                case "Ring1": return Slot.Ring1;
                case "Ring2": return Slot.Ring2;
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
                case Slot.Wrist: return "Wrist";
                case Slot.Hands: return "Hands";
                case Slot.Waist: return "Waist";
                case Slot.Legs: return "Legs";
                case Slot.Feet: return "Feet";
                case Slot.Ring1: return "Ring1";
                case Slot.Ring2: return "Ring2";
                case Slot.Trinket1: return "Trinket1";
                case Slot.Trinket2: return "Trinket2";
                case Slot.MH: return "MH";
                case Slot.OH: return "OH";
                case Slot.Ranged: return "Ranged";
                default: throw new Exception("Slot not found");
            }
        }
        
        public static int StrToAPRatio(Classes c)
        {
            return (c == Classes.Druid || c == Classes.Warrior || c == Classes.Shaman || c == Classes.Paladin) ? 2 : 1;
        }

        public static int AgiToRangedAPRatio(Classes c)
        {
            return (c == Classes.Warrior || c == Classes.Hunter || c == Classes.Rogue) ? 2 : 1;
        }

        public static int AgiToAPRatio(Classes c)
        {
            return (c == Classes.Druid || c == Classes.Hunter || c == Classes.Rogue) ? 1 : 0;
        }

        public static double AgiToCritRatio(Classes c)
        {
            switch (c)
            {
                case Classes.Hunter: return 1.0 / 5300;
                case Classes.Rogue: return 1.0 / 2900;
                default: return 1.0 / 2000;
            }
        }

        public static double IntToCritRatio(Classes c)
        {
            switch (c)
            {
                case Classes.Druid: return 1.0 / 6000;
                case Classes.Mage: return 1.0 / 5950;
                case Classes.Paladin: return 1.0 / 5400;
                case Classes.Priest: return 1.0 / 5920;
                case Classes.Shaman: return 1.0 / 5950;
                case Classes.Warlock: return 1.0 / 6060;
                default: return 0;
            }
        }

        public static double BaseCrit(Classes c)
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

        public static Attributes BaseAttributes(Classes c, Races r, int level = 60)
        {
            // TODO : by level

            Attributes res = new Attributes();

            switch (c)
            {
                case Classes.Druid:
                    switch (r)
                    {
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 62);
                            res.Values.Add(Attribute.Agility, 65);
                            res.Values.Add(Attribute.Stamina, 69);
                            res.Values.Add(Attribute.Intellect, 100);
                            res.Values.Add(Attribute.Spirit, 110);
                            res.Values.Add(Attribute.Health, 1483);
                            res.Values.Add(Attribute.Mana, 1244);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 70);
                            res.Values.Add(Attribute.Agility, 55);
                            res.Values.Add(Attribute.Stamina, 72);
                            res.Values.Add(Attribute.Intellect, 95);
                            res.Values.Add(Attribute.Spirit, 112);
                            res.Values.Add(Attribute.Health, 1483);
                            res.Values.Add(Attribute.Mana, 1244);
                            break;
                        default: break;
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
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 30);
                            res.Values.Add(Attribute.Agility, 35);
                            res.Values.Add(Attribute.Stamina, 45);
                            res.Values.Add(Attribute.Intellect, 125);
                            res.Values.Add(Attribute.Spirit, 126);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 31);
                            res.Values.Add(Attribute.Agility, 37);
                            res.Values.Add(Attribute.Stamina, 46);
                            res.Values.Add(Attribute.Intellect, 121);
                            res.Values.Add(Attribute.Spirit, 121);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 29);
                            res.Values.Add(Attribute.Agility, 33);
                            res.Values.Add(Attribute.Stamina, 46);
                            res.Values.Add(Attribute.Intellect, 123);
                            res.Values.Add(Attribute.Spirit, 125);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
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
                            res.Values.Add(Attribute.Mana, 1232);
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
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 35);
                            res.Values.Add(Attribute.Agility, 40);
                            res.Values.Add(Attribute.Stamina, 50);
                            res.Values.Add(Attribute.Intellect, 120);
                            res.Values.Add(Attribute.Spirit, 131);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 32);
                            res.Values.Add(Attribute.Agility, 45);
                            res.Values.Add(Attribute.Stamina, 49);
                            res.Values.Add(Attribute.Intellect, 120);
                            res.Values.Add(Attribute.Spirit, 125);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 36);
                            res.Values.Add(Attribute.Agility, 42);
                            res.Values.Add(Attribute.Stamina, 51);
                            res.Values.Add(Attribute.Intellect, 116);
                            res.Values.Add(Attribute.Spirit, 126);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 34);
                            res.Values.Add(Attribute.Agility, 38);
                            res.Values.Add(Attribute.Stamina, 51);
                            res.Values.Add(Attribute.Intellect, 118);
                            res.Values.Add(Attribute.Spirit, 130);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Rogue:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 82);
                            res.Values.Add(Attribute.Agility, 126);
                            res.Values.Add(Attribute.Stamina, 78);
                            res.Values.Add(Attribute.Intellect, 34);
                            res.Values.Add(Attribute.Spirit, 49);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 75);
                            res.Values.Add(Attribute.Agility, 133);
                            res.Values.Add(Attribute.Stamina, 74);
                            res.Values.Add(Attribute.Intellect, 40);
                            res.Values.Add(Attribute.Spirit, 50);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 80);
                            res.Values.Add(Attribute.Agility, 130);
                            res.Values.Add(Attribute.Stamina, 75);
                            res.Values.Add(Attribute.Intellect, 35);
                            res.Values.Add(Attribute.Spirit, 52);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 77);
                            res.Values.Add(Attribute.Agility, 135);
                            res.Values.Add(Attribute.Stamina, 74);
                            res.Values.Add(Attribute.Intellect, 35);
                            res.Values.Add(Attribute.Spirit, 50);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 83);
                            res.Values.Add(Attribute.Agility, 127);
                            res.Values.Add(Attribute.Stamina, 77);
                            res.Values.Add(Attribute.Intellect, 32);
                            res.Values.Add(Attribute.Spirit, 53);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 81);
                            res.Values.Add(Attribute.Agility, 132);
                            res.Values.Add(Attribute.Stamina, 76);
                            res.Values.Add(Attribute.Intellect, 31);
                            res.Values.Add(Attribute.Spirit, 51);
                            res.Values.Add(Attribute.Health, 1523);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 79);
                            res.Values.Add(Attribute.Agility, 128);
                            res.Values.Add(Attribute.Stamina, 76);
                            res.Values.Add(Attribute.Intellect, 33);
                            res.Values.Add(Attribute.Spirit, 55);
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
                    switch (r)
                    {
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 40);
                            res.Values.Add(Attribute.Agility, 53);
                            res.Values.Add(Attribute.Stamina, 64);
                            res.Values.Add(Attribute.Intellect, 119);
                            res.Values.Add(Attribute.Spirit, 115);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 45);
                            res.Values.Add(Attribute.Agility, 50);
                            res.Values.Add(Attribute.Stamina, 65);
                            res.Values.Add(Attribute.Intellect, 110);
                            res.Values.Add(Attribute.Spirit, 120);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 48);
                            res.Values.Add(Attribute.Agility, 47);
                            res.Values.Add(Attribute.Stamina, 66);
                            res.Values.Add(Attribute.Intellect, 107);
                            res.Values.Add(Attribute.Spirit, 118);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 44);
                            res.Values.Add(Attribute.Agility, 48);
                            res.Values.Add(Attribute.Stamina, 66);
                            res.Values.Add(Attribute.Intellect, 108);
                            res.Values.Add(Attribute.Spirit, 120);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Warrior:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 122);
                            res.Values.Add(Attribute.Agility, 76);
                            res.Values.Add(Attribute.Stamina, 113);
                            res.Values.Add(Attribute.Intellect, 29);
                            res.Values.Add(Attribute.Spirit, 44);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 115);
                            res.Values.Add(Attribute.Agility, 83);
                            res.Values.Add(Attribute.Stamina, 109);
                            res.Values.Add(Attribute.Intellect, 35);
                            res.Values.Add(Attribute.Spirit, 45);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 120);
                            res.Values.Add(Attribute.Agility, 80);
                            res.Values.Add(Attribute.Stamina, 110);
                            res.Values.Add(Attribute.Intellect, 30);
                            res.Values.Add(Attribute.Spirit, 47);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 117);
                            res.Values.Add(Attribute.Agility, 85);
                            res.Values.Add(Attribute.Stamina, 109);
                            res.Values.Add(Attribute.Intellect, 30);
                            res.Values.Add(Attribute.Spirit, 45);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 123);
                            res.Values.Add(Attribute.Agility, 77);
                            res.Values.Add(Attribute.Stamina, 112);
                            res.Values.Add(Attribute.Intellect, 27);
                            res.Values.Add(Attribute.Spirit, 48);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 125);
                            res.Values.Add(Attribute.Agility, 75);
                            res.Values.Add(Attribute.Stamina, 112);
                            res.Values.Add(Attribute.Intellect, 27);
                            res.Values.Add(Attribute.Spirit, 47);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 121);
                            res.Values.Add(Attribute.Agility, 82);
                            res.Values.Add(Attribute.Stamina, 111);
                            res.Values.Add(Attribute.Intellect, 26);
                            res.Values.Add(Attribute.Spirit, 46);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 119);
                            res.Values.Add(Attribute.Agility, 78);
                            res.Values.Add(Attribute.Stamina, 111);
                            res.Values.Add(Attribute.Intellect, 28);
                            res.Values.Add(Attribute.Spirit, 50);
                            res.Values.Add(Attribute.Health, 1689);
                            res.Values.Add(Attribute.Mana, 0);
                            break;
                        default: break;
                    }
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

        public static int BaseAP(Classes c, int level = 60)
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
                    // level * 3 - 20 // Bear
                    return level * 2 - 20; // Cat
                default:
                    return level * 2 - 20;
            }
        }

        public static int BaseRAP(Classes c, int level = 60)
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
            "Nightslayer", "Shadowcraft"
        };

        #endregion

        #region Properties

        public static double GCD = 1.5;
        public static double GCD_ENERGY = 1;

        public Forms Form { get; set; }
        public bool Stealthed { get; set; }
        public bool BehindBoss { get; set; }

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
                if (value < 0)
                {
                    value = 0;
                }
                else if(value > MaxMana)
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

        public Dictionary<string, int> Talents { get; set; }

        public List<Enchantment> Buffs { get; set; }

        public Weapon MH
        {
            get
            {
                return (Weapon)Equipment[Slot.MH];
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
                if (Equipment[Slot.OH] != null)
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
        public double ManaTick { get; set; }

        public double MPTRatio { get; set; }
        public double CastingManaRegenRate { get; set; }

        public double AP
        {
            get
            {
                return Attributes.GetValue(Attribute.AP) + BonusAttributes.GetValue(Attribute.AP);
            }
        }

        public double SP
        {
            get
            {
                return Attributes.GetValue(Attribute.SP) + BonusAttributes.GetValue(Attribute.SP);
            }
        }

        public double CritRating
        {
            get
            {
                return Attributes.GetValue(Attribute.CritChance) + BonusAttributes.GetValue(Attribute.CritChance);
            }
        }

        public double HitRating
        {
            get
            {
                return Attributes.GetValue(Attribute.HitChance) + BonusAttributes.GetValue(Attribute.HitChance);
            }
        }

        public bool DualWielding
        {
            get { return !MH.TwoHanded && OH != null && OH.Type != Weapon.WeaponType.Offhand; }
        }

        public Attributes Attributes { get; set; }
        public Attributes BonusAttributes { get; set; }

        public double HasteMod { get; set; }
        public double DamageMod { get; set; }

        public Races Race { get; set; }
        public Classes Class { get; set; }

        public Action applyAtNextAA = null;
        public int nextAABonus = 0;

        public Dictionary<Weapon.WeaponType, int> WeaponSkill { get; set; }

        public Dictionary<Entity, HitChances> HitChancesByEnemy { get; set; }

        public bool WindfuryTotem { get; set; }

        public List<string> Cooldowns { get; set; }

        public Dictionary<string, int> Sets { get; set; }

        public Dictionary<string, CustomAction> CustomActions { get; set; }

        public Dictionary<Skill, int> cds = new Dictionary<Skill, int>();

        public AutoAttack mh = null;
        public AutoAttack oh = null;

        public Spell casting = null;

        public int rota = 0;

        #endregion

        #region Constructors

        public Player(Player p)
            : this(null, p)
        {
        }

        public Player(Simulation s, Player p)
            : this(s, p.Class, p.Race, p.Level, p.Equipment, p.Talents, p.Buffs)
        {
            BehindBoss = p.BehindBoss;
            rota = p.rota;
            Attributes.Values = new Dictionary<Attribute, double>(p.Attributes.Values);
            WeaponSkill = p.WeaponSkill;
            MH.DamageMin = p.MH.DamageMin;
            MH.DamageMax = p.MH.DamageMax;
            if (DualWielding)
            {
                OH.DamageMin = p.OH.DamageMin;
                OH.DamageMax = p.OH.DamageMax;
            }
            WindfuryTotem = p.WindfuryTotem;
            Cooldowns = p.Cooldowns;
            BaseMana = p.BaseMana;

            Sets = p.Sets;
            ApplySets();
        }

        public Player(Simulation s = null, Classes c = Classes.Warrior, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, 
            Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, MobType.Humanoid, level)
        {
            Race = r;
            Class = c;
            
            Talents = new Dictionary<string, int>(talents != null ? talents : new Dictionary<string, int>());
            
            Buffs = new List<Enchantment>(buffs != null ? buffs : new List<Enchantment>());

            int baseSkill = Level * 5;
            WeaponSkill = new Dictionary<Weapon.WeaponType, int>();
            foreach (Weapon.WeaponType type in (Weapon.WeaponType[])Enum.GetValues(typeof(Weapon.WeaponType)))
            {
                int skill = baseSkill;

                if(Race == Races.Orc && type == Weapon.WeaponType.Axe)
                {
                    skill += 5;
                }
                else if(Race == Races.Human && (type == Weapon.WeaponType.Mace || type == Weapon.WeaponType.Sword))
                {
                    skill += 5;
                }

                WeaponSkill[type] = skill;
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

            HitChancesByEnemy = new Dictionary<Entity, HitChances>();

            CustomActions = new Dictionary<string, CustomAction>();

            Sets = new Dictionary<string, int>();

            Reset();
        }

        #endregion

        #region Rota

        public virtual void PrepFight()
        {
            if(MH.Speed > 0)
            {
                mh = new AutoAttack(this, MH, true);
                if (DualWielding)
                {
                    oh = new AutoAttack(this, OH, false);
                }
            }
        }

        public abstract void Rota();

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
                            aa.Cast();
                        }
                    }
                    else
                    {
                        aa.Cast();
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

        public abstract void SetupTalents(string ptal);

        public void ResetMHSwing()
        {
            ResetAATimer(mh);
        }

        public void ResetAATimer(AutoAttack auto)
        {
            if(auto != null)
            {
                auto.ResetSwing();
                auto.CastNextSwing();
            }
        }

        public void ApplySets()
        {
            if (Class == Classes.Rogue)
            {
                if (Sets["Nightslayer"] >= 5)
                {
                    MaxResource += 10;
                }
            }
        }

        public void CheckSets()
        {
            foreach(string s in SPECIALSETS_LIST)
            {
                Sets.Add(s, NbSet(s));
            }
        }

        public int NbSet(string set)
        {
            return Equipment.Where(a => a.Value != null).Count(e => e.Value.Name.Contains(set));
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
            MPTRatio = 1;
            CastingManaRegenRate = 0;

            Form = Forms.Human;
            Stealthed = false;

            Attributes = new Attributes();
            BaseMana = Attributes.GetValue(Attribute.Mana);

            BonusAttributes = new Attributes();
        }

        public void SetBaseAttributes()
        {
            Attributes = new Attributes();
            Attributes.Values = new Dictionary<Attribute, double>(BaseAttributes(Class, Race).Values);
            Attributes.SetValue(Attribute.AP, BaseAP(Class, Level));
            Attributes.SetValue(Attribute.RangedAP, BaseRAP(Class, Level));
            BaseMana = Attributes.GetValue(Attribute.Mana);
        }

        public void CalculateAttributes()
        {
            SetBaseAttributes();

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

            if (Class == Classes.Warrior)
            {
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + 0.01 * GetTalentPoints("Cruelty")
                    + (Simulation.tank ? 0 : 0.03)); // Berserker Stance, should be a spell for stance dancing
                if(Simulation.tank)
                {
                    DamageMod *= 0.9;
                }
            }
            else if (Class == Classes.Druid)
            {
                Attributes.SetValue(Attribute.Intellect, Attributes.GetValue(Attribute.Intellect)
                    * (1 + 0.04 * GetTalentPoints("HW")));
                Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength)
                    * (1 + 0.04 * GetTalentPoints("HW")));
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + 0.01 * GetTalentPoints("SC"));
                Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP)
                    + 0.5 * GetTalentPoints("PS") * Level);
            }
            else if (Class == Classes.Rogue)
            {
                Attributes.SetValue(Attribute.HitChance, Attributes.GetValue(Attribute.HitChance)
                    + GetTalentPoints("Prec") / 100.0);
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + GetTalentPoints("Malice") / 100.0);
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + GetTalentPoints("FS") / 100.0);
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + GetTalentPoints("DS") / 100.0);
                Attributes.SetValue(Attribute.SkillSword, Attributes.GetValue(Attribute.SkillSword)
                    + (GetTalentPoints("WE") > 0 ? (GetTalentPoints("WE") == 1 ? 3 : 5) : 0));
                Attributes.SetValue(Attribute.SkillDagger, Attributes.GetValue(Attribute.SkillDagger)
                    + (GetTalentPoints("WE") > 0 ? (GetTalentPoints("WE") == 1 ? 3 : 5) : 0));
            }
            else if(Class == Classes.Warlock)
            {
                Attributes.SetValue(Attribute.Stamina, Attributes.GetValue(Attribute.Stamina)
                    + 0.03 * GetTalentPoints("DE"));
                Attributes.SetValue(Attribute.Spirit, Attributes.GetValue(Attribute.Spirit)
                    - 0.01 * GetTalentPoints("DE"));
            }
            else if (Class == Classes.Mage)
            {
                Attributes.SetValue(Attribute.SpellCritChance, Attributes.GetValue(Attribute.SpellCritChance)
                    + 0.01 * GetTalentPoints("AI"));
                DamageMod *= 1 + 0.01 * GetTalentPoints("AI");
                CastingManaRegenRate += 0.05 * GetTalentPoints("AMed");
            }
            else if (Class == Classes.Paladin)
            {
                Attributes.SetValue(Attribute.HitChance, Attributes.GetValue(Attribute.HitChance)
                    + GetTalentPoints("Prcsn") / 100.0);
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance) + 0.01 * GetTalentPoints("Conv"));
                Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength) + Attributes.GetValue(Attribute.Strength) * 0.02 * GetTalentPoints("DivStr"));
                Attributes.SetValue(Attribute.Intellect, Attributes.GetValue(Attribute.Intellect) + Attributes.GetValue(Attribute.Intellect) * 0.02 * GetTalentPoints("DivInt"));
            }

            Attributes += BonusAttributesByRace(Race, Attributes);

            if (Buffs.Any(b => b.Name.ToLower().Contains("blessing of kings")))
            {
                Attributes.SetValue(Attribute.Stamina, Attributes.GetValue(Attribute.Stamina) * 1.1);
                Attributes.SetValue(Attribute.Intellect, Attributes.GetValue(Attribute.Intellect) * 1.1);
                Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength) * 1.1);
                Attributes.SetValue(Attribute.Agility, Attributes.GetValue(Attribute.Agility) * 1.1);
                Attributes.SetValue(Attribute.Spirit, Attributes.GetValue(Attribute.Spirit) * 1.1);
            }

            Attributes.SetValue(Attribute.Health, Attributes.GetValue(Attribute.Health) + 20 + (Attributes.GetValue(Attribute.Stamina) - 20) * 10);
            if (Attributes.GetValue(Attribute.Mana) > 0)
            {
                Attributes.SetValue(Attribute.Mana, Attributes.GetValue(Attribute.Mana) + Attributes.GetValue(Attribute.Intellect) * 15);
                if(Class == Classes.Mage)
                {
                    Attributes.SetValue(Attribute.Mana, Attributes.GetValue(Attribute.Mana) * 1 + (0.02 * GetTalentPoints("AMind")));
                }
            }
            Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Strength) * StrToAPRatio(Class) + Attributes.GetValue(Attribute.Agility) * AgiToAPRatio(Class));
            Attributes.SetValue(Attribute.RangedAP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Agility) * AgiToRangedAPRatio(Class));
            Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance) + Attributes.GetValue(Attribute.Agility) * AgiToCritRatio(Class));
            Attributes.SetValue(Attribute.SpellCritChance, Attributes.GetValue(Attribute.SpellCritChance) + BaseCrit(Class) + Attributes.GetValue(Attribute.Intellect) * IntToCritRatio(Class));

            int baseSkill = Level * 5;
            WeaponSkill = new Dictionary<Weapon.WeaponType, int>();
            foreach (Weapon.WeaponType type in (Weapon.WeaponType[])Enum.GetValues(typeof(Weapon.WeaponType)))
            {
                int skill = baseSkill;

                if (Race == Races.Orc && type == Weapon.WeaponType.Axe)
                {
                    skill += 5;
                }
                else if (Race == Races.Human && (type == Weapon.WeaponType.Mace || type == Weapon.WeaponType.Sword))
                {
                    skill += 5;
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

            HasteMod = CalcHaste();
        }

        public void ExtraAA(List<string> alreadyProc)
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

        public void CheckOnHits(bool isMH, bool isAA, ResultType res, bool extra = false, List<string> alreadyProc = null)
        {
            if(alreadyProc == null)
            {
                alreadyProc = new List<string>();
            }

            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                if (Class == Classes.Warrior)
                {
                    if (GetTalentPoints("DW") > 0)
                    {
                        DeepWounds.CheckProc(this, res, GetTalentPoints("DW"));
                    }
                    if (GetTalentPoints("Flurry") > 0)
                    {
                        Flurry.CheckProc(this, res, GetTalentPoints("Flurry"), extra);
                    }
                    if (GetTalentPoints("UW") > 0)
                    {
                        UnbridledWrath.CheckProc(this, res, GetTalentPoints("UW"));
                    }
                }
                else if (Class == Classes.Druid)
                {
                    if (GetTalentPoints("OC") > 0)
                    {
                        ClearCasting.CheckProc(this);
                    }
                }
                else if (Class == Classes.Paladin)
                {
                    //  Turn on Vengeance
                    if (GetTalentPoints("Veng") > 0)
                    {
                        Vengeance.CheckProc(this, res);
                    }

                    //  Refresh the currently judged seal
                    JudgementOfTheCrusaderDebuff.RefreshJudgement(this, res);

                    //  See if seal of command should proc
                    if (Effects.ContainsKey(SealOfCommandBuff_Rank5.NAME))
                    {
                        SealOfCommandBuff_Rank5.CheckProc(this, res, 0);
                    }
                    else if (Effects.ContainsKey(SealOfCommandBuff_Rank1.NAME))
                    {
                        SealOfCommandBuff_Rank1.CheckProc(this, res, 0);
                    }
                    else if (Effects.ContainsKey(SealOfRighteousnessBuff.NAME))
                    {
                        SealOfRighteousnessBuff.CheckProc(this, res, 0);
                    }
                }
                else if (Class == Classes.Rogue)
                {
                    if (!alreadyProc.Contains("rSP") && GetTalentPoints("SS") > 0 && Randomer.NextDouble() < 0.01 * GetTalentPoints("SS"))
                    {
                        alreadyProc.Add("rSP");
                        if (Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Sword Specialization procs", Sim.CurrentTime));
                        }
                        ExtraAA(alreadyProc);
                    }
                    if((!WindfuryTotem || !isMH) && !alreadyProc.Contains("IP") && Randomer.NextDouble() < 0.2)
                    {
                        alreadyProc.Add("IP");
                        string procName = "Instant Poison";
                        int procDmg = Randomer.Next(112, 148 + 1);
                        if (!CustomActions.ContainsKey(procName))
                        {
                            CustomActions.Add(procName, new CustomAction(this, procName, School.Nature));
                        }

                        ResultType res2 = SpellAttackEnemy(Sim.Boss);
                        int dmg = MiscDamageCalc(procDmg, res2, School.Nature);
                        CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg));
                    }
                }

                //  See if spell vulnerability has been triggered
                SpellVulnerability.CheckProc(this, null, res);

                if (Form == Forms.Human)
                {
                    if ((isMH && MH.Enchantment != null && MH.Enchantment.Name == "Crusader") || (!isMH && OH.Enchantment != null && OH.Enchantment.Name == "Crusader"))
                    {
                        Crusader.CheckProc(this, res, MH.Speed);
                    }

                    if (isMH && WindfuryTotem && !alreadyProc.Contains("WF") && Randomer.NextDouble() < 0.2)
                    {
                        alreadyProc.Add("WF");

                        if (Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Windfury procs", Sim.CurrentTime));
                        }

                        nextAABonus = 315;
                        ExtraAA(alreadyProc);
                    }
                }

                if(isAA && (Class == Classes.Rogue || Form == Forms.Cat))
                {
                    if (Sets["Shadowcraft"] >= 6 && 
                        (Form == Forms.Cat && Randomer.NextDouble() < 1.0 / 60) ||
                        (Form != Forms.Cat && ((isMH && Randomer.NextDouble() < MH.Speed / 60) || (!isMH && Randomer.NextDouble() < OH.Speed / 60))))
                    {
                        Resource += 35;
                        if(Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Shadowcraft 6P procs (energy {1}/{2})", Sim.CurrentTime, Resource, MaxResource));
                        }
                    }
                }

                if ((Equipment[Slot.Trinket1]?.Name == "Hand of Justice" || Equipment[Slot.Trinket2]?.Name == "Hand of Justice") && !alreadyProc.Contains("HoJ") && Randomer.NextDouble() < 0.02)
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

                    ResultType res2 = SpellAttackEnemy(Sim.Boss);
                    int dmg = MiscDamageCalc(procDmg, res2, School.Shadow);
                    CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg));
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

                    ResultType res2 = SpellAttackEnemy(Sim.Boss);
                    int dmg = MiscDamageCalc(procDmg, res2, School.Fire);
                    CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg));
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

                    ResultType res2 = YellowAttackEnemy(Sim.Boss);
                    int dmg = MiscDamageCalc(procDmg, res2);
                    CustomActions[procName].RegisterDamage(new ActionResult(res2, dmg));
                }
            }
        }

        public int MiscDamageCalc(int baseDmg, ResultType res, School school = School.Physical, double APRatio = 0)
        {
            if(school == School.Physical)
            {
                return (int)Math.Round(baseDmg
                    * Sim.DamageMod(res)
                    * Simulation.ArmorMitigation(Sim.Boss.Armor)
                    * DamageMod);
            }
            else
            {
                return (int)Math.Round((baseDmg + APRatio * AP)
                    * Sim.DamageMod(res)
                    * Simulation.MagicMitigation(Sim.Boss.ResistChances[school])
                    * DamageMod);
            }
        }

        public void CalculateHitChances(Entity enemy = null)
        {
            if(enemy == null)
            {
                enemy = Sim.Boss;
            }

            Dictionary<ResultType, double> whiteHitChancesMH = new Dictionary<ResultType, double>();
            whiteHitChancesMH.Add(ResultType.Miss, MissChance(DualWielding, HitRating, WeaponSkill[MH.Type], enemy.Level));
            whiteHitChancesMH.Add(ResultType.Dodge, enemy.DodgeChance(WeaponSkill[MH.Type]));
            whiteHitChancesMH.Add(ResultType.Glance, GlancingChance(Level, enemy.Level));
            //  If behind boss, parry & block chance moves into hit chance
            if (BehindBoss)
            {
                whiteHitChancesMH.Add(ResultType.Parry, 0);
                whiteHitChancesMH.Add(ResultType.Block, 0);
            }
            else
            {
                whiteHitChancesMH.Add(ResultType.Parry, EnemyParryChance(Level, WeaponSkill[MH.Type], enemy.Level));
                whiteHitChancesMH.Add(ResultType.Block, enemy.BlockChance());
            }
            whiteHitChancesMH.Add(ResultType.Crit, RealCritChance(CritWithSuppression(CritRating + (MH.Buff == null ? 0 : MH.Buff.Attributes.GetValue(Attribute.CritChance)), Level, enemy.Level), whiteHitChancesMH[ResultType.Miss], whiteHitChancesMH[ResultType.Glance], whiteHitChancesMH[ResultType.Dodge], whiteHitChancesMH[ResultType.Parry], whiteHitChancesMH[ResultType.Block]));
            whiteHitChancesMH.Add(ResultType.Hit, RealHitChance(whiteHitChancesMH[ResultType.Miss], whiteHitChancesMH[ResultType.Glance], whiteHitChancesMH[ResultType.Crit], whiteHitChancesMH[ResultType.Dodge], whiteHitChancesMH[ResultType.Parry], whiteHitChancesMH[ResultType.Block]));

            Dictionary<ResultType, double> whiteHitChancesOH = null;
            if (DualWielding)
            {
                whiteHitChancesOH = new Dictionary<ResultType, double>();
                whiteHitChancesOH.Add(ResultType.Miss, MissChance(true, HitRating + (OH.Buff == null ? 0 : OH.Buff.Attributes.GetValue(Attribute.HitChance)), WeaponSkill[OH.Type], enemy.Level));
                whiteHitChancesOH.Add(ResultType.Dodge, enemy.DodgeChance(WeaponSkill[OH.Type]));
                //  If behind boss, parry & block chance moves into hit chance
                if (BehindBoss)
                {
                    whiteHitChancesOH.Add(ResultType.Parry, 0);
                    whiteHitChancesOH.Add(ResultType.Block, 0);
                }
                else
                {
                    whiteHitChancesOH.Add(ResultType.Parry, EnemyParryChance(Level, WeaponSkill[OH.Type], enemy.Level));
                    whiteHitChancesOH.Add(ResultType.Block, enemy.BlockChance());
                }
                whiteHitChancesOH.Add(ResultType.Glance, GlancingChance(Level, enemy.Level));
                whiteHitChancesOH.Add(ResultType.Crit, RealCritChance(CritWithSuppression(CritRating + (OH.Buff == null ? 0 : OH.Buff.Attributes.GetValue(Attribute.CritChance)), Level, enemy.Level), whiteHitChancesOH[ResultType.Miss], whiteHitChancesOH[ResultType.Glance], whiteHitChancesOH[ResultType.Dodge], whiteHitChancesOH[ResultType.Parry], whiteHitChancesOH[ResultType.Block]));
                whiteHitChancesOH.Add(ResultType.Hit, RealHitChance(whiteHitChancesOH[ResultType.Miss], whiteHitChancesOH[ResultType.Glance], whiteHitChancesOH[ResultType.Crit], whiteHitChancesOH[ResultType.Dodge], whiteHitChancesOH[ResultType.Parry], whiteHitChancesOH[ResultType.Block]));
            }

            Dictionary<ResultType, double> yellowHitChances = new Dictionary<ResultType, double>();
            yellowHitChances.Add(ResultType.Miss, MissChanceYellow(HitRating, WeaponSkill[MH.Type], enemy.Level));
            yellowHitChances.Add(ResultType.Dodge, enemy.DodgeChance(WeaponSkill[MH.Type]));
            //  If behind boss, parry & block chance moves into hit chance
            if (BehindBoss)
            {
                yellowHitChances.Add(ResultType.Parry, 0);
                yellowHitChances.Add(ResultType.Block, 0);
            }
            else
            {
                yellowHitChances.Add(ResultType.Parry, EnemyParryChance(Level, WeaponSkill[MH.Type], enemy.Level));
                yellowHitChances.Add(ResultType.Block, enemy.BlockChance());
            }
            yellowHitChances.Add(ResultType.Crit, RealCritChance(CritWithSuppression(CritRating + (MH.Buff == null ? 0 : MH.Buff.Attributes.GetValue(Attribute.CritChance)), Level, enemy.Level), yellowHitChances[ResultType.Miss], 0, yellowHitChances[ResultType.Dodge], yellowHitChances[ResultType.Parry], yellowHitChances[ResultType.Block]));
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
            GCDUntil = Sim.CurrentTime + ((Class == Classes.Rogue || Form == Forms.Cat) ? GCD_ENERGY : GCD);
        }

        public bool HasGCD()
        {
            return GCDUntil <= Sim.CurrentTime;
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

        public bool ManaTicking()
        {
            return CastingManaRegenRate > 0 || Sim.CurrentTime >= LastManaExpenditure + 5;
        }

        public void CheckManaTick()
        {
            if (ManaTicking() && Sim.CurrentTime >= ManaTick + 2)
            {
                ManaTick = ManaTick < LastManaExpenditure + 5 ? (CastingManaRegenRate > 0 ? Sim.CurrentTime : LastManaExpenditure + 5) : ManaTick + 2;
                Mana += (int)(MPT() * (Sim.CurrentTime < LastManaExpenditure + 5 ? CastingManaRegenRate : 1) * MPTRatio);
            }
        }

        public double MPT()
        {
            switch (Class)
            {
                case Classes.Warlock: return 8 + Attributes.GetValue(Attribute.Spirit) / 4;
                case Classes.Mage: return 13 + Attributes.GetValue(Attribute.Spirit) / 4;
                case Classes.Priest: return 13 + Attributes.GetValue(Attribute.Spirit) / 4;
                default: return 15 + Attributes.GetValue(Attribute.Spirit) / 5;
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
            return string.Format("{0} {1} level {2} | Stats : {3}", Race, Class, Level, Attributes);
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

        public static double SpellHitChance(double spellHit, int level, int enemyLevel)
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

        public static double EnemyParryChance(int level, int skill, int enemyLevel)
        {
            int skillDif = enemyLevel * 5 - level * 5;
            return Simulation.tank ? Math.Max(0, skillDif > 10 ? 0.14 - (skill - level * 5) * 0.001 : 0.05 + (skill - enemyLevel * 5) * 0.001) : 0;
        }

        #endregion

        #region Attack methods

        public ResultType SpellAttackEnemy(Entity enemy, bool canCrit = true, double bonusHit = 0, double bonusCrit = 0)
        {
            if (Randomer.NextDouble() < SpellHitChance(Attributes.GetValue(Attribute.SpellHitChance) + bonusHit, Level, enemy.Level))
            {
                if (canCrit && Randomer.NextDouble() < Attributes.GetValue(Attribute.SpellCritChance) + bonusCrit)
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

        //  Paladin's Judgement uses spell hit but melee crit
        public ResultType MeleeSpellAttackEnemy(Entity enemy, bool canCrit = true, double bonusHit = 0, double bonusCrit = 0)
        {
            if (Randomer.NextDouble() < SpellHitChance(Attributes.GetValue(Attribute.SpellHitChance) + bonusHit, Level, enemy.Level))
            {
                if (canCrit && Randomer.NextDouble() < Attributes.GetValue(Attribute.CritChance) + bonusCrit)
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

            return PickFromTable(MH ? HitChancesByEnemy[enemy].WhiteHitChancesMH : HitChancesByEnemy[enemy].WhiteHitChancesOH, Randomer.NextDouble());
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

            if (Class == Classes.Rogue && spell.Equals("Backstab"))
            {
                table = new Dictionary<ResultType, double>(table);
                table[ResultType.Crit] += 0.1 * GetTalentPoints("IB");
            }

            ResultType res = PickFromTable(table, Randomer.NextDouble());

            return res;
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

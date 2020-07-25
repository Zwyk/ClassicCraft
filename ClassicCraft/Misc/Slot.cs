using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
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
        Finger,
        Trinket,
        Weapon,
        Ranged,
        Offhand,
        Any
    }

    public class SlotUtil
    {
        public static Slot FromString(string s)
        {
            switch (s)
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
                case "Finger": return Slot.Finger;
                case "Trinket": return Slot.Trinket;
                case "Weapon": return Slot.Weapon;
                case "Ranged": return Slot.Ranged;
                case "Offhand": return Slot.Offhand;
                default: return Slot.Any;
            }
        }

        public static string ToString(Slot s)
        {
            switch (s)
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
                case Slot.Finger: return "Finger";
                case Slot.Trinket: return "Trinket";
                case Slot.Weapon: return "Weapon";
                case Slot.Ranged: return "Ranged";
                case Slot.Offhand: return "Offhand";
                case Slot.Any: return "Any";
                default: throw new Exception("Slot not found");
            }
        }

        public static List<Player.Slot> ToPlayerSlot(Slot s)
        {
            switch(s)
            {
                case Slot.Any: return new List<Player.Slot>((Player.Slot[])Enum.GetValues(typeof(Player.Slot)));
                case Slot.Head: return new List<Player.Slot>() { Player.Slot.Head };
                case Slot.Neck: return new List<Player.Slot>() { Player.Slot.Neck };
                case Slot.Shoulders: return new List<Player.Slot>() { Player.Slot.Shoulders };
                case Slot.Back: return new List<Player.Slot>() { Player.Slot.Back };
                case Slot.Chest: return new List<Player.Slot>() { Player.Slot.Chest };
                case Slot.Wrists: return new List<Player.Slot>() { Player.Slot.Wrists };
                case Slot.Hands: return new List<Player.Slot>() { Player.Slot.Hands };
                case Slot.Waist: return new List<Player.Slot>() { Player.Slot.Waist };
                case Slot.Legs: return new List<Player.Slot>() { Player.Slot.Legs };
                case Slot.Feet: return new List<Player.Slot>() { Player.Slot.Feet };
                case Slot.Finger: return new List<Player.Slot>() { Player.Slot.Finger1, Player.Slot.Finger2 };
                case Slot.Trinket: return new List<Player.Slot>() { Player.Slot.Trinket1, Player.Slot.Trinket2 };
                case Slot.Weapon: return new List<Player.Slot>() { Player.Slot.OH, Player.Slot.MH };
                case Slot.Ranged: return new List<Player.Slot>() { Player.Slot.Ranged };
                case Slot.Offhand: return new List<Player.Slot>() { Player.Slot.OH };
                default: throw new Exception("Slot not eligible for Player Slot");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Weapon : Item
    {
        public enum WeaponType
        {
            Sword,
            Axe,
            Mace,
            Spear,
            Staff,
            Dagger,
            Bow,
            Crossbow,
            Gun,
            Throwable
        }

        public static string TypeToString(WeaponType wt)
        {
            switch(wt)
            {
                case WeaponType.Sword: return "Sword";
                case WeaponType.Axe: return "Axe";
                case WeaponType.Mace: return "Mace";
                case WeaponType.Spear: return "Spear";
                case WeaponType.Staff: return "Staff";
                case WeaponType.Dagger: return "Dagger";
                case WeaponType.Bow: return "Bow";
                case WeaponType.Crossbow: return "Crossbow";
                case WeaponType.Gun: return "Gun";
                case WeaponType.Throwable: return "Throwable";
                default: throw new Exception("WeaponType not found");
            }
        }

        public static WeaponType StringToType(string s)
        {
            switch(s)
            {
                case "Sword": return WeaponType.Sword;
                case "Axe": return WeaponType.Axe;
                case "Mace": return WeaponType.Mace;
                case "Spear": return WeaponType.Spear;
                case "Staff": return WeaponType.Staff;
                case "Dagger": return WeaponType.Dagger;
                case "Bow": return WeaponType.Bow;
                case "Crossbow": return WeaponType.Crossbow;
                case "Gun": return WeaponType.Gun;
                case "Throwable": return WeaponType.Throwable;
                default: throw new Exception("WeaponType not found");
            }
        }

        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public double Speed { get; set; }
        public bool TwoHanded { get; set; }
        public WeaponType Type { get; set; }

        public Weapon(Player p = null, int min = 1, int max = 2, double speed = 1, bool twoHanded = true, WeaponType type = WeaponType.Axe, Attributes attributes = null, int id = 0, string name = "New Item", ItemEffect effect = null)
            : base(p, Slot.Weapon, attributes, id, name, effect)
        {
            DamageMin = min;
            DamageMax = max;
            Speed = speed;
            TwoHanded = twoHanded;
            Type = type;
        }

        public Weapon(int min, int max, double speed, bool twoHanded, WeaponType type, Attributes attributes = null, int id = 0, string name = "New Item", ItemEffect effect = null)
            : this(null, min, max, speed, twoHanded, type, attributes, id, name, effect)
        {
        }

        public override string ToString()
        {
            string attributes = "";
            foreach (Attribute a in Attributes.Values.Keys)
            {
                attributes += "[" + a + ":" + Attributes.Values[a] + "]";
            }

            return String.Format("[{0}] ({1}) {2} : {3} | {4}-{5} at {6}, 2H = {7}, Type = {8}", Slot, Id, Name, attributes, DamageMin, DamageMax, Speed, TwoHanded, Type);
        }
    }
}

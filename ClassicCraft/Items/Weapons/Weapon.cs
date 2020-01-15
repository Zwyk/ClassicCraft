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
            Polearm,
            Staff,
            Dagger,
            Fist,
            Bow,
            Crossbow,
            Gun,
            Throwable,
        }

        public static string TypeToString(WeaponType wt)
        {
            switch(wt)
            {
                case WeaponType.Sword: return "Sword";
                case WeaponType.Axe: return "Axe";
                case WeaponType.Mace: return "Mace";
                case WeaponType.Polearm: return "Polearm";
                case WeaponType.Staff: return "Staff";
                case WeaponType.Dagger: return "Dagger";
                case WeaponType.Fist: return "Fist";
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
                case "Polearm": return WeaponType.Polearm;
                case "Staff": return WeaponType.Staff;
                case "Dagger": return WeaponType.Dagger;
                case "Fist": return WeaponType.Fist;
                case "Bow": return WeaponType.Bow;
                case "Crossbow": return WeaponType.Crossbow;
                case "Gun": return WeaponType.Gun;
                case "Throwable": return WeaponType.Throwable;
                default: throw new Exception("WeaponType not found : " + s);
            }
        }

        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public double Speed { get; set; }
        public bool TwoHanded { get; set; }
        public WeaponType Type { get; set; }
        public Enchantment Buff { get; set; }
        public double Dps
        {
            get
            {
                return ((double)DamageMin + DamageMax) / 2 * Speed;
            }
        }

        public Weapon(int min = 1, int max = 2, double speed = 1, bool twoHanded = true, WeaponType type = WeaponType.Axe, Attributes attributes = null, int id = 0, string name = "New Item", Enchantment enchantment = null, Enchantment buff = null, ItemEffect effect = null)
            : base(Slot.Weapon, attributes, id, name, enchantment, effect)
        {
            DamageMin = min;
            DamageMax = max;
            Speed = speed;
            TwoHanded = twoHanded;
            Type = type;
            Buff = buff;

            if (Buff != null && Buff.Attributes.GetValue(Attribute.WeaponDamage) > 0)
            {
                int bonus = (int)Math.Round(Buff.Attributes.GetValue(Attribute.WeaponDamage));
                DamageMin += bonus;
                DamageMax += bonus;
            }
        }

        public override string ToString()
        {
            string attributes = "";
            foreach (Attribute a in Attributes.Values.Keys)
            {
                attributes += "[" + a + ":" + Attributes.Values[a] + "]";
            }

            return string.Format("[{0}] ({1}) {2} : {3} | {4}-{5} at {6}, 2H = {7}, Type = {8}", Slot, Id, Name, attributes, DamageMin, DamageMax, Speed, TwoHanded, Type);
        }
    }
}

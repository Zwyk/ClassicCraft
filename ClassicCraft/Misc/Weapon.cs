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
            Wand,
            Throwable,
            Offhand,
            Shield,
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
                case WeaponType.Wand: return "Wand";
                case WeaponType.Throwable: return "Throwable";
                case WeaponType.Offhand: return "Offhand";
                case WeaponType.Shield: return "Shield";
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
                case "Wand": return WeaponType.Wand;
                case "Throwable": return WeaponType.Throwable;
                case "Offhand": return WeaponType.Offhand;
                case "Shield": return WeaponType.Shield;
                default: throw new Exception("WeaponType not found : " + s);
            }
        }

        public static WeaponType FromAttribute(Attribute a)
        {
            return StringToType(AttributeUtil.ToString(a));
        }

        public double BaseMin { get; set; }
        public double BaseMax { get; set; }
        public double DamageMin { get; set; }
        public double DamageMax { get; set; }
        public double Speed { get; set; }
        public bool TwoHanded { get; set; }
        public WeaponType Type { get; set; }
        public School School { get; set; }
        public Enchantment Buff { get; set; }
        public double Dps
        {
            get
            {
                return (DamageMin + DamageMax) / 2 * Speed;
            }
        }

        public Weapon(double min = 1, double max = 2, double speed = 1, bool twoHanded = true, WeaponType type = WeaponType.Axe, Attributes attributes = null, int id = 0, string name = "New Item", Enchantment enchantment = null, Enchantment buff = null, ItemEffect effect = null, School school = School.Physical)
            : base(Slot.Weapon, attributes, id, name, enchantment, effect)
        {
            DamageMin = min;
            DamageMax = max;
            Speed = speed;
            TwoHanded = twoHanded;
            Type = type;
            Buff = buff;
            School = school;

            if (Buff != null && Buff.Attributes.GetValue(Attribute.WeaponDamage) > 0)
            {
                double bonus = Buff.Attributes.GetValue(Attribute.WeaponDamage);
                DamageMin += bonus;
                DamageMax += bonus;
            }
            else if(Buff == null)
            {
                Buff = new Enchantment();
            }

            BaseMin = DamageMin;
            BaseMax = DamageMax;
        }

        public override string ToString()
        {
            string attributes = "";
            foreach (Attribute a in Attributes.Values.Keys)
            {
                attributes += "[" + a + ":" + Attributes.Values[a] + "]";
            }

            string s = string.Format("[{0}] ({1}) {2} : {3} | SpellType = {4}", Slot, Id, Name, attributes, Type);
            if(Type != WeaponType.Offhand && Type != WeaponType.Shield)
            {
                s += string.Format(", {0}-{1} at {2}, 2H = {3}", DamageMin, DamageMax, Speed, TwoHanded);
            }
            return s;
        }
    }
}

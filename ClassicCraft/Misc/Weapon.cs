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
                default: throw new Exception("WeaponType not found : " + s);
            }
        }

        public static WeaponType FromAttribute(Attribute a)
        {
            return StringToType(AttributeUtil.ToString(a));
        }

        public static List<Weapon> BuiltIn = new List<Weapon>()
        {
            new HanzoSword(),
            new TheUnstoppableForce(),
            new ObsidianEdgedBlade(),
            new Nightfall(),
            new Sulfuras(),
            new BonereaversEdge(),
            new Ashkandi(),
        };

        public double BaseMin { get; set; }
        public double BaseMax { get; set; }
        public double DamageMin { get; set; }
        public double DamageMax { get; set; }
        public double Speed { get; set; }
        public bool TwoHanded { get; set; }
        public WeaponType Type { get; set; }
        public Enchantment Buff { get; set; }
        public double Dps
        {
            get
            {
                return (DamageMin + DamageMax) / 2 * Speed;
            }
        }

        public Weapon(double min = 1, double max = 2, double speed = 1, bool twoHanded = true, WeaponType type = WeaponType.Axe, Attributes attributes = null, int id = 0, string name = "New Item", Enchantment enchantment = null, Enchantment buff = null, ItemEffect effect = null)
            : base(Slot.Weapon, attributes, id, name, enchantment, effect)
        {
            Name = name;
            DamageMin = min;
            DamageMax = max;
            Speed = speed;
            TwoHanded = twoHanded;
            Type = type;
            Buff = buff;

            if (Buff != null && Buff.Attributes.GetValue(Attribute.WeaponDamage) > 0)
            {
                double bonus = Buff.Attributes.GetValue(Attribute.WeaponDamage);
                DamageMin += bonus;
                DamageMax += bonus;
            }

            BaseMin = DamageMin;
            BaseMax = DamageMax;
        }

        public Weapon(string name = "New Item", Enchantment enchantment = null)
        {
            var selectedWeapon = BuiltIn.Where(x => string.Compare(x.Name, name, true) == 0)?.ToList().FirstOrDefault();
            if (selectedWeapon != null)
            {
                Name = selectedWeapon.Name;
                DamageMin = selectedWeapon.DamageMin;
                DamageMax = selectedWeapon.DamageMax;
                Speed = selectedWeapon.Speed;
                TwoHanded = selectedWeapon.TwoHanded;
                Type = selectedWeapon.Type;
                Buff = selectedWeapon.Buff;

                if (Buff != null && Buff.Attributes.GetValue(Attribute.WeaponDamage) > 0)
                {
                    double bonus = Buff.Attributes.GetValue(Attribute.WeaponDamage);
                    DamageMin += bonus;
                    DamageMax += bonus;
                }

                BaseMin = DamageMin;
                BaseMax = DamageMax;
            }

            Enchantment = enchantment;
        }

        public override string ToString()
        {
            string attributes = "";
            foreach (Attribute a in Attributes.Values.Keys)
            {
                attributes += "[" + a + ":" + Attributes.Values[a] + "]";
            }

            string s = string.Format("[{0}] ({1}) {2} : {3} | Type = {4}", Slot, Id, Name, attributes, Type);
            if(Type != WeaponType.Offhand)
            {
                s += string.Format(", {0}-{1} at {2}, 2H = {3}", DamageMin, DamageMax, Speed, TwoHanded);
            }
            return s;
        }
    }
}

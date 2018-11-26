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
        }

        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public double Speed { get; set; }
        public bool TwoHanded { get; set; }
        public WeaponType Type { get; set; }

        public Weapon(Player p, Slot slot, int min, int max, double speed, bool twoHanded, WeaponType type, Attributes attributes = null, ItemEffect effect = null)
            : base(p, slot, attributes, effect)
        {
            DamageMin = min;
            DamageMax = max;
            Speed = speed;
            TwoHanded = twoHanded;
            Type = type;
        }
    }
}

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

        public delegate void WeaponEffect();

        public int DamageMin { get; set; }
        public int DamageMax { get; set; }
        public double Speed { get; set; }
        public bool TwoHanded { get; set; }
        public WeaponType Type { get; set; }
        public WeaponEffect SpecialEffect { get; set; }

        public Weapon(int min, int max, double speed, bool twoHanded, WeaponType type, int str = 0, int agi = 0, int intel = 0, WeaponEffect effect = null)
            : base(str, agi, intel)
        {
            DamageMin = min;
            DamageMax = max;
            Speed = speed;
            TwoHanded = twoHanded;
            Type = type;
            SpecialEffect = effect;
        }
    }
}

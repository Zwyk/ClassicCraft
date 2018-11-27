using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Boss : Entity
    {
        public enum BossType
        {
            NoArmor,
            LightArmor,
            HeavyArmor,
            Custom
        }

        public Boss(Boss b)
            : base(null, b.Level, b.MaxLife, b.Armor)
        {
        }

        public Boss(Simulation s, Boss b)
            : base(s, b.Level, b.MaxLife, b.Armor)
        {
        }

        public Boss(Simulation s = null, int level = 63, int maxLife = 100000, BossType type = BossType.LightArmor, int customArmor = 0)
            : base(s, level, maxLife, ArmorByType(type, customArmor))
        {
        }

        public static int ArmorByType(BossType type, int customArmor = 0)
        {
            switch(type)
            {
                case BossType.HeavyArmor: return 5600;
                case BossType.LightArmor: return 4400;
                case BossType.Custom: return customArmor;
                default:  return 0;
            }
        }

        public override double BlockChance()
        {
            return 0;
        }

        public override double ParryChance()
        {
            return 0;
        }
    }
}

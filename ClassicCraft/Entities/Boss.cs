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
            HeavyArmor
        }

        public Boss(Boss b)
            : base(null, b.Level, b.Armor, b.MaxLife)
        {
        }

        public Boss(int level = 63, int customArmor = 4400, int maxLife = 100000)
            : base(null, level, customArmor, maxLife)
        {
        }

        public Boss(Simulation s, Boss b)
            : base(s, b.Level, b.Armor, b.MaxLife)
        {
        }

        public Boss(Simulation s, int level = 63, int customArmor = 4400, int maxLife = 100000)
            : base(s, level, customArmor, maxLife)
        {
        }

        public Boss(Simulation s, int level = 63, BossType type = BossType.LightArmor, int maxLife = 100000)
            : base(s, level, ArmorByType(type), maxLife)
        {
        }

        public static int ArmorByType(BossType type)
        {
            switch(type)
            {
                case BossType.HeavyArmor: return 5600;
                case BossType.LightArmor: return 4400;
                default: return 0;
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

        public override string ToString()
        {
            return String.Format("Level {0}, {1} Armor\n", Level, Armor);
        }
    }
}

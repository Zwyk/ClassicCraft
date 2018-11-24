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

        public Boss(BossType type = BossType.LightArmor, int customArmor = 0)
            : base()
        {
            Level = 63;
            Armor = ArmorByType(type, customArmor);
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

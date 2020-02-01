using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Boss : Entity
    {
        public enum ArmorType
        {
            NoArmor,
            LightArmor,
            HeavyArmor
        }

        public Boss(Boss b)
            : base(null, b.Type, b.Level, b.Armor, b.MaxLife)
        {
        }

        public Boss(MobType type = MobType.Humanoid, int level = 63, int customArmor = 4400, int maxLife = 100000)
            : base(null, type, level, customArmor, maxLife)
        {
        }

        public Boss(Simulation s, Boss b)
            : base(s, b.Type, b.Level, b.Armor, b.MaxLife)
        {
        }

        public Boss(Simulation s, MobType type = MobType.Humanoid, int level = 63, int customArmor = 4400, Dictionary<School, int> magicResist = null, int maxLife = 100000)
            : base(s, type, level, customArmor, maxLife, magicResist)
        {
        }

        public Boss(Simulation s, MobType type = MobType.Humanoid, int level = 63, ArmorType armor = ArmorType.LightArmor, Dictionary<School, int> magicResist = null, int maxLife = 100000)
            : base(s, type, level, ArmorByType(armor), maxLife, magicResist)
        {
        }

        public static int ArmorByType(ArmorType armor)
        {
            switch(armor)
            {
                case ArmorType.HeavyArmor: return 5600;
                case ArmorType.LightArmor: return 4400;
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
            return string.Format("Level {0}, {1} Armor ({2:N2}% mitigation)\n", Level, Armor, (1-Simulation.ArmorMitigation(Armor))*100);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Entity
    {
        public static double BASE_MISS = 0.05;

        public int Life { get; set; }
        public int MaxLife { get; set; }
        public double LifePct
        {
            get
            {
                return (double)Life / MaxLife;
            }
            set
            {
                Life = (int)Math.Round(MaxLife * value);
            }
        }
        public int Armor { get; set; }
        public int Level { get; set; }

        public List<Effect> Effects { get; set; }

        public List<Effect> Permanent { get; set; }

        public Entity()
        {
            Reset();
        }

        public virtual void Reset()
        {
            Effects = new List<Effect>();
        }

        public static double ArmorMitigation(int armor, int attackerLevel = 60)
        {
            double res = armor / (armor + 400 + 85.0 * attackerLevel);
            return 1 - (res > 0.75 ? 0.75 : res);
        }

        public double DodgeChance(int attackerSkill)
        {
            return Math.Max(0.05 + 0.0004 * Math.Abs(Level * 5 - attackerSkill), 0);
        }

        public abstract double BlockChance();

        public abstract double ParryChance();
    }
}

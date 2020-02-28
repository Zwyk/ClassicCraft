using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public enum School
    {
        Physical,
        Magical,
        Shadow,
        Light,
        Fire,
        Nature,
        Frost,
        Arcane,
    }

    public abstract class Entity : SimulationObject
    {
        public enum MobType
        {
            Humanoid,
            Giant,
            Beast,
            Dragonkin,
            Undead,
            Other
        }

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
        public MobType Type { get; set; }
        public int Armor { get; set; }

        public Dictionary<School, Dictionary<double, double>> ResistChances { get; set; }
        public Dictionary<School, int> MagicResist { get; set; }

        public int Level { get; set; }

        public Dictionary<string, Effect> Effects { get; set; }

        public Entity(Simulation s, MobType type, int level, int armor = 0, int maxLife = 1, Dictionary<School, int> magicResist = null)
            : base(s)
        {
            Type = type;
            Level = level;
            Armor = armor;
            MaxLife = maxLife;

            ResistChances = new Dictionary<School, Dictionary<double, double>>();
            MagicResist = new Dictionary<School, int>();
            if (magicResist == null)
            {
                foreach (School school in (School[])Enum.GetValues(typeof(School)))
                {
                    MagicResist.Add(school, 0);
                    ResistChances.Add(school, Simulation.ResistChances(MagicResist[school]));
                }
            }
            else
            {
                foreach (School school in (School[])Enum.GetValues(typeof(School)))
                {
                    MagicResist.Add(school, Math.Max(0, magicResist.ContainsKey(school) ? magicResist[school] : (magicResist.ContainsKey(School.Magical) ? magicResist[School.Magical] : 0)));
                    ResistChances.Add(school, Simulation.ResistChances(MagicResist[school]));
                }
            }

            Reset();
        }

        public virtual void Reset()
        {
            Life = MaxLife;
            Effects = new Dictionary<string, Effect>();
        }

        public double DodgeChance(int attackerSkill)
        {
            return Math.Max(0.05 + 0.001 * (Level * 5 - attackerSkill), 0);
        }

        public abstract double BlockChance();
    }
}

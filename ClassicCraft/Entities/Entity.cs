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
        Holy,
        Fire,
        Nature,
        Frost,
        Arcane,
    }

    public class Entity : SimulationObject
    {
        public enum MobType
        {
            Humanoid,
            Demon,
            Giant,
            Beast,
            Dragonkin,
            Undead,
            Other
        }

        public static double BASE_MISS = 0.05;

        public string Name { get; set; }
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

        public Dictionary<string, Effect> BaseEffects { get; set; }
        public Dictionary<string, Effect> Effects { get; set; }

        public Entity(string name, Simulation s, MobType type, int level, int armor, int maxLife, Dictionary<School, int> magicResist, Dictionary<string, Effect> baseEffects)
            : base(s)
        {
            Name = name;
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
                    if(school != School.Physical)
                    {
                        MagicResist.Add(school, 0);
                        ResistChances.Add(school, Simulation.ResistChances(MagicResist[school]));
                    }
                }
            }
            else
            {
                foreach (School school in (School[])Enum.GetValues(typeof(School)))
                {
                    if (school != School.Physical)
                    {
                        MagicResist.Add(school, Math.Max(0, magicResist.ContainsKey(school) ? magicResist[school] : (magicResist.ContainsKey(School.Magical) ? magicResist[School.Magical] : 0)));
                        ResistChances.Add(school, Simulation.ResistChances(MagicResist[school]));
                    }
                }
            }

            Reset();

            BaseEffects = new Dictionary<string, Effect>();
            if(baseEffects != null)
            {
                foreach (string k in baseEffects.Keys)
                {
                    BaseEffects.Add(k, baseEffects[k]);
                    Effects.Add(k, baseEffects[k]);
                }
            }
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

        public virtual double BlockChance()
        {
            throw new NotImplementedException();
        }
    }
}

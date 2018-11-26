using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public enum Attribute
    {
        Stamina,
        Strength,
        Agility,
        Intelligence,
        Spirit,
        Health,
        Mana,
        Armor,
        AP,
        RangedAP,
        SP,
        HP,
        HitChance,
        CritChance,
        MP5,
        SkillSword,
        SkillAxe,
        SkillMace,
        SkillSpear,
        SkillStaff,
        SkillDagger,
        Skill1H,
        Skill2H,
    }

    public class Attributes
    {
        public Dictionary<Attribute, double> Values { get; set; }

        public Attributes(Attributes a)
            : this(a == null ? new Dictionary<Attribute, double>() : a.Values) { }

        public Attributes(Dictionary<Attribute, double> values)
        {
            Values = new Dictionary<Attribute, double>(values);
        }

        public Attributes()
        {
            Values = new Dictionary<Attribute, double>();
        }

        public void SetValue(Attribute a, double val)
        {
            if(Values.ContainsKey(a))
            {
                Values[a] = val;
            }
            else
            {
                Values.Add(a, val);
            }
        }

        public double GetValue(Attribute a)
        {
            if(Values.ContainsKey(a))
            {
                return Values[a];
            }
            else
            {
                return 0;
            }
        }

        public static Attributes operator +(Attributes b, Attributes c)
        {
            foreach(Attribute a in c.Values.Keys)
            {
                b.SetValue(a, b.GetValue(a) + c.GetValue(a));
            }

            return new Attributes(b);
        }
    }
}

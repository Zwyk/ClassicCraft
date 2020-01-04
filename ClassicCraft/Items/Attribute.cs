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
        Intellect,
        Spirit,
        Health,
        Mana,
        Armor,
        ArmorPen,
        Haste,
        AP,
        RangedAP,
        SP,
        HSP,
        HitChance,
        SpellHitChance,
        CritChance,
        SpellCritChance,
        MP5,
        SkillSword,
        SkillAxe,
        SkillMace,
        SkillPolearm,
        SkillStaff,
        SkillDagger,
        SkillFist,
        Skill1H,
        Skill2H,
        SkillBow,
        SkillCrossbow,
        SkillGun,
        SkillThrowable,
        WeaponDamage,
    }

    public class AttributeUtil
    {
        public static Attribute FromString(string s)
        {
            switch(s)
            {
                case "Sta": return Attribute.Stamina;
                case "Str": return Attribute.Strength;
                case "Agi": return Attribute.Agility;
                case "Int": return Attribute.Intellect;
                case "Spi": return Attribute.Spirit;
                case "HP": return Attribute.Health;
                case "Mana": return Attribute.Mana;
                case "Armor": return Attribute.Armor;
                case "ArmorPen": return Attribute.ArmorPen;
                case "Haste": return Attribute.Haste;
                case "AP": return Attribute.AP;
                case "RAP": return Attribute.RangedAP;
                case "SP": return Attribute.SP;
                case "HSP": return Attribute.HSP;
                case "Hit": return Attribute.HitChance;
                case "SpellHit": return Attribute.SpellHitChance;
                case "Crit": return Attribute.CritChance;
                case "SpellCrit": return Attribute.SpellCritChance;
                case "MP5": return Attribute.MP5;
                case "Sword": return Attribute.SkillSword;
                case "Axe": return Attribute.SkillAxe;
                case "Mace": return Attribute.SkillMace;
                case "Polearm": return Attribute.SkillPolearm;
                case "Staff": return Attribute.SkillStaff;
                case "Dagger": return Attribute.SkillDagger;
                case "Fist": return Attribute.SkillFist;
                case "1H": return Attribute.Skill1H;
                case "2H": return Attribute.Skill2H;
                case "Bow": return Attribute.SkillBow;
                case "Crossbow": return Attribute.SkillCrossbow;
                case "Gun": return Attribute.SkillGun;
                case "Throwable": return Attribute.SkillThrowable;
                case "WDmg": return Attribute.WeaponDamage;
                default: throw new Exception("Attribute not found : " + s);
            }
        }

        public static string ToString(Attribute a)
        {
            switch(a)
            {
                case Attribute.Stamina: return "Sta";
                case Attribute.Strength: return "Str";
                case Attribute.Agility: return "Agi";
                case Attribute.Intellect: return "Int";
                case Attribute.Spirit: return "Spi";
                case Attribute.Health: return "HP";
                case Attribute.Mana: return "Mana";
                case Attribute.Armor: return "Armor";
                case Attribute.ArmorPen: return "ArmorPen";
                case Attribute.Haste: return "Haste";
                case Attribute.AP: return "AP";
                case Attribute.RangedAP: return "RAP";
                case Attribute.SP: return "SP";
                case Attribute.HSP: return "HSP";
                case Attribute.HitChance: return "Hit";
                case Attribute.SpellHitChance: return "SpellHit";
                case Attribute.CritChance: return "Crit";
                case Attribute.SpellCritChance: return "SpellCrit";
                case Attribute.MP5: return "MP5";
                case Attribute.SkillSword: return "Sword";
                case Attribute.SkillAxe: return "Axe";
                case Attribute.SkillMace: return "Mace";
                case Attribute.SkillPolearm: return "Polearm";
                case Attribute.SkillStaff: return "Staff";
                case Attribute.SkillDagger: return "Dagger";
                case Attribute.SkillFist: return "Fist";
                case Attribute.Skill1H: return "1H";
                case Attribute.Skill2H: return "2H";
                case Attribute.SkillBow: return "Bow";
                case Attribute.SkillCrossbow: return "Crossbow";
                case Attribute.SkillGun: return "Gun";
                case Attribute.SkillThrowable: return "Throwable";
                case Attribute.WeaponDamage: return "WDmg";
                default: throw new Exception("Attribute not found");
            }
        }
    }

    public class Attributes
    {
        public Dictionary<Attribute, double> Values { get; set; }

        public Attributes(Attributes a)
            : this(a.Values) { }

        public Attributes(Dictionary<Attribute, double> values)
        {
            Values = values;
        }

        public Attributes(Dictionary<string, double> values)
        {
            Values = new Dictionary<Attribute, double>();

            if(values != null)
            {
                foreach (string s in values.Keys)
                {
                    Values.Add(AttributeUtil.FromString(s), values[s]);
                }
            }
        }

        public Attributes()
        {
            Values = new Dictionary<Attribute, double>();
        }

        public static Attributes FromStringDic(Dictionary<string, double> values)
        {
            return new Attributes(values);
        }

        public static Dictionary<string, double> ToStringDic(Attributes at)
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();

            foreach(Attribute a in at.Values.Keys)
            {
                dic.Add(AttributeUtil.ToString(a), at.Values[a]);
            }

            return dic;
        }

        public void SetValue(Attribute a, double val)
        {
            if (Values == null) Values = new Dictionary<Attribute, double>();

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
            if(Values != null && Values.ContainsKey(a))
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

        public override string ToString()
        {
            string stats = "";
            foreach (Attribute a in Values.Keys)
            {
                double val = Values[a];
                if (a == Attribute.CritChance || a == Attribute.HitChance || a == Attribute.Haste) val *= 100;
                stats += "[" + a + ":" + Math.Round(val*100)/100 + "]";
            }
            return stats;
        }
    }
}

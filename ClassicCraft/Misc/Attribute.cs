﻿using System;
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
        FirePen,
        ShadowPen,
        NaturePen,
        HolyPen,
        ArcanePen,
        FrostPen,
        Haste,
        AP,
        RangedAP,
        SP,
        SPFire,
        SPShadow,
        SPNature,
        SPHoly,
        SPArcane,
        SPFrost,
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
        WeaponDamageMH,
        WeaponDamageOH,
        Expertise,
        BlockValue,
    }

    public class AttributeUtil
    {
        public static Attribute FromString(string s)
        {
            switch(s)
            {
                case "Sta": return Attribute.Stamina;
                case "Stamina": return Attribute.Stamina;
                case "Str": return Attribute.Strength;
                case "Strength": return Attribute.Strength;
                case "Agi": return Attribute.Agility;
                case "Agility": return Attribute.Agility;
                case "Int": return Attribute.Intellect;
                case "Intellect": return Attribute.Intellect;
                case "Spi": return Attribute.Spirit;
                case "Spirit": return Attribute.Spirit;
                case "HP": return Attribute.Health;
                case "Health": return Attribute.Health;
                case "Mana": return Attribute.Mana;
                case "Armor": return Attribute.Armor;
                case "ArmorPen": return Attribute.ArmorPen;
                case "FirePen":
                    return Attribute.FirePen;
                case "ShadowPen":
                    return Attribute.ShadowPen;
                case "NaturePen":
                    return Attribute.NaturePen;
                case "HolyPen":
                    return Attribute.HolyPen;
                case "ArcanePen":
                    return Attribute.ArcanePen;
                case "FrostPen":
                    return Attribute.FrostPen;
                case "ArPen": return Attribute.ArmorPen;
                case "Haste": return Attribute.Haste;
                case "AP": return Attribute.AP;
                case "RAP": return Attribute.RangedAP;
                case "SP": return Attribute.SP;
                case "SPArcane": return Attribute.SPArcane;
                case "SPFire": return Attribute.SPFire;
                case "SPFrost": return Attribute.SPFrost;
                case "SPHoly": return Attribute.SPHoly;
                case "SPNature": return Attribute.SPNature;
                case "SPShadow": return Attribute.SPShadow;
                case "HSP": return Attribute.HSP;
                case "Hit": return Attribute.HitChance;
                case "SHit": return Attribute.SpellHitChance;
                case "SpellHit": return Attribute.SpellHitChance;
                case "Crit": return Attribute.CritChance;
                case "SCrit": return Attribute.SpellCritChance;
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
                case "Thrown": return Attribute.SkillThrowable;
                case "WDmg": return Attribute.WeaponDamage;
                case "WDmgMH": return Attribute.WeaponDamageMH;
                case "WDmgOH": return Attribute.WeaponDamageOH;
                case "Expertise": return Attribute.Expertise;
                case "BlockValue": return Attribute.BlockValue;
                case "Block": return Attribute.BlockValue;
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
                case Attribute.FirePen: return "FirePen";
                case Attribute.ShadowPen: return "ShadowPen";
                case Attribute.NaturePen: return "NaturePen";
                case Attribute.HolyPen: return "HolyPen";
                case Attribute.ArcanePen: return "ArcanePen";
                case Attribute.FrostPen: return "FrostPen";
                case Attribute.Haste: return "Haste";
                case Attribute.AP: return "AP";
                case Attribute.RangedAP: return "RAP";
                case Attribute.SP: return "SP";
                case Attribute.SPArcane: return "SPArcane";
                case Attribute.SPFire: return "SPFire";
                case Attribute.SPFrost: return "SPFrost";
                case Attribute.SPHoly: return "SPHoly";
                case Attribute.SPNature: return "SPNature";
                case Attribute.SPShadow: return "SPShadow";
                case Attribute.HSP: return "HSP";
                case Attribute.HitChance: return "Hit";
                case Attribute.SpellHitChance: return "SHit";
                case Attribute.CritChance: return "Crit";
                case Attribute.SpellCritChance: return "SCrit";
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
                case Attribute.SkillThrowable: return "Thrown";
                case Attribute.WeaponDamage: return "WDmg";
                case Attribute.WeaponDamageMH: return "WDmgMH";
                case Attribute.WeaponDamageOH: return "WDmgOH";
                case Attribute.Expertise: return "Expertise";
                case Attribute.BlockValue: return "BlockValue";
                default: throw new Exception("Attribute not found");
            }
        }

        public static Attribute PenFromSchool(School school)
        {
            switch(school)
            {
                case School.Arcane: return Attribute.ArcanePen;
                case School.Fire: return Attribute.FirePen;
                case School.Frost: return Attribute.FrostPen;
                case School.Holy: return Attribute.HolyPen;
                case School.Nature: return Attribute.NaturePen;
                case School.Shadow: return Attribute.ShadowPen;
                default: throw new Exception("School not found");
            }
        }

        public static School SchoolFromPen(Attribute at)
        {
            switch (at)
            {
                case Attribute.ArcanePen: return School.Arcane;
                case Attribute.FirePen: return School.Fire;
                case Attribute.FrostPen:    return School.Frost;
                case Attribute.HolyPen: return School.Holy;
                case Attribute.NaturePen: return School.Nature;
                case Attribute.ShadowPen: return School.Shadow;
                default: throw new Exception("School not found");
            }
        }

        public static Attribute FromWeaponType(Weapon.WeaponType w)
        {
            return FromString(Weapon.TypeToString(w));
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

            if (Values.ContainsKey(a))
            {
                Values[a] = val;
            }
            else
            {
                Values.Add(a, val);
            }
        }

        public void AddToValue(Attribute a, double val)
        {
            SetValue(a, GetValue(a) + val);
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
            if(c != null && c.Values != null)
            {
                foreach (Attribute a in c.Values.Keys)
                {
                    b.SetValue(a, b.GetValue(a) + c.GetValue(a));
                }
            }

            return new Attributes(b);
        }

        public override string ToString()
        {
            string stats = "";
            string pct;
            foreach (Attribute a in Values.Keys)
            {
                double val = Values[a];
                if (a == Attribute.CritChance || a == Attribute.HitChance || a == Attribute.Haste
                    || a == Attribute.SpellCritChance || a == Attribute.SpellHitChance || a == Attribute.Expertise)
                {
                    val *= 100;
                    pct = "%";
                }
                else pct = "";

                stats += "[" + a + ":" + Math.Round(val*100)/100 + pct + "]";
            }
            return stats;
        }
    }
}

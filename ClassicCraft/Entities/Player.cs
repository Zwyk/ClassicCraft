using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Player : Entity
    {
        public class HitChances
        {
            public HitChances(Dictionary<ResultType, double> whiteHitChancesMH, Dictionary<ResultType, double> yellowHitChances, Dictionary<ResultType, double> whiteHitChancesOH = null)
            {
                WhiteHitChancesMH = whiteHitChancesMH;
                WhiteHitChancesOH = whiteHitChancesOH;
                YellowHitChances = yellowHitChances;
            }

            public Dictionary<ResultType, double> WhiteHitChancesMH { get; set; }
            public Dictionary<ResultType, double> WhiteHitChancesOH { get; set; }
            public Dictionary<ResultType, double> YellowHitChances { get; set; }
        }

        public enum Races
        {
            Human,
            Dwarf,
            NightElf,
            Gnome,
            Orc,
            Undead,
            Tauren,
            Troll
        }

        public enum Classes
        {
            Priest,
            Rogue,
            Warrior,
            Mage,
            Druid,
            Hunter,
            Warlock,
            Shaman,
            Paladin
        }

        public enum Slot
        {
            Head,
            Neck,
            Shoulders,
            Back,
            Chest,
            Wrist,
            Hands,
            Waist,
            Legs,
            Feet,
            Ring1,
            Ring2,
            Trinket1,
            Trinket2,
            MH,
            OH
        }

        #region Propriétés

        public static double GCD = 1.5;

        private int ressource;
        public int Ressource
        {
            get
            {
                return ressource;
            }
            set
            {
                if(value > 100)
                {
                    ressource = 100;
                }
                else if(value < 0)
                {
                    ressource = 0;
                }
                else
                {
                    ressource = value;
                }
            }
        }

        public Dictionary<Slot, Item> Items { get; set; }

        public Dictionary<string, int> Talents { get; set; }

        public Weapon MH
        {
            get
            {
                return (Weapon)Items[Slot.MH];
            }
            set
            {
                Items[Slot.MH] = value;
            }
        }

        public Weapon OH
        {
            get
            {
                if(Items[Slot.OH] != null)
                {
                    return (Weapon)Items[Slot.OH];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Items[Slot.OH] = value;
            }
        }

        public double GCDUntil { get; set; }

        public double AP
        {
            get
            {
                return Attributes.GetValue(Attribute.AP);
            }
        }

        public double CritRating
        {
            get
            {
                return Attributes.GetValue(Attribute.CritChance);
            }
        }

        public double HitRating
        {
            get
            {
                return Attributes.GetValue(Attribute.HitChance);
            }
        }

        public Attributes Attributes { get; set; }
        public Attributes BonusAttributes { get; set; }

        public double HasteMod { get; set; }
        public double DamageMod { get; set; }

        public Races Race { get; set; }
        public Classes Class { get; set; }

        public Action applyAtNextAA = null;

        public Dictionary<Weapon.WeaponType, int> WeaponSkill { get; set; }

        public Dictionary<Entity, HitChances> HitChancesByEnemy { get; set; }

        #endregion

        public Player(Simulation s, Player p)
            : this(s, p.Class, p.Race)
        {

        }
            /*
            : base(s, p.Level, p.MaxLife, p.Armor)
        {
            Race = p.Race;
            Class = p.Class;

            Talents = p.Talents;
            WeaponSkill = p.WeaponSkill;
            Items = p.Items;

            HitChancesByEnemy = new Dictionary<Entity, HitChances>();

            Attributes = p.Attributes;

            Reset();
        }*/

        public Player(Simulation s, Classes c, Races r, int level = 60, int maxLife = 1000, int armor = 0)
            : base(s, level, maxLife, armor)
        {
            Race = r;
            Class = c;

            Talents = new Dictionary<string, int>();

            int skill = Level * 5;
            WeaponSkill = new Dictionary<Weapon.WeaponType, int>();
            foreach (Weapon.WeaponType type in (Weapon.WeaponType[])Enum.GetValues(typeof(Weapon.WeaponType)))
            {
                if(Race == Races.Orc && type == Weapon.WeaponType.Axe)
                {
                    WeaponSkill[type] = skill + 5;
                }
                else if(Race == Races.Human && (type == Weapon.WeaponType.Mace || type == Weapon.WeaponType.Sword))
                {
                    WeaponSkill[type] = skill + 5;
                }
                else
                {
                    WeaponSkill[type] = skill;
                }
            }

            Items = new Dictionary<Slot, Item>();
            foreach (Slot slot in (Slot[])Enum.GetValues(typeof(Slot)))
            {
                Items[slot] = null;
            }

            HitChancesByEnemy = new Dictionary<Entity, HitChances>();

            Reset();
        }

        public int GetTalentPoints(string talent)
        {
            return Talents.ContainsKey(talent) ? Talents[talent] : 0;
        }

        public override void Reset()
        {
            base.Reset();

            Ressource = 0;

            GCDUntil = 0;
            
            HasteMod = 1;
            DamageMod = 1;
        }

        public void CalculateHitChances(Entity enemy = null)
        {
            if(enemy == null)
            {
                enemy = Sim.Boss;
            }

            Dictionary<ResultType, double> whiteHitChancesMH = new Dictionary<ResultType, double>();
            whiteHitChancesMH.Add(ResultType.Miss, MissChance(DualWielding(), HitRating, WeaponSkill[MH.Type], enemy.Level));
            whiteHitChancesMH.Add(ResultType.Dodge, enemy.DodgeChance(WeaponSkill[MH.Type]));
            whiteHitChancesMH.Add(ResultType.Parry, enemy.ParryChance());
            whiteHitChancesMH.Add(ResultType.Glancing, GlancingChance(WeaponSkill[MH.Type], enemy.Level));
            whiteHitChancesMH.Add(ResultType.Block, enemy.BlockChance());
            whiteHitChancesMH.Add(ResultType.Crit, RealCritChance(CritRating, whiteHitChancesMH[ResultType.Miss], whiteHitChancesMH[ResultType.Glancing], whiteHitChancesMH[ResultType.Dodge], whiteHitChancesMH[ResultType.Parry], whiteHitChancesMH[ResultType.Block]));
            whiteHitChancesMH.Add(ResultType.Hit, RealHitChance(whiteHitChancesMH[ResultType.Miss], whiteHitChancesMH[ResultType.Glancing], whiteHitChancesMH[ResultType.Crit], whiteHitChancesMH[ResultType.Dodge], whiteHitChancesMH[ResultType.Parry], whiteHitChancesMH[ResultType.Block]));

            Dictionary<ResultType, double> whiteHitChancesOH = null;
            if (DualWielding())
            {
                whiteHitChancesOH = new Dictionary<ResultType, double>();
                whiteHitChancesOH.Add(ResultType.Miss, MissChance(true, HitRating, WeaponSkill[OH.Type], enemy.Level));
                whiteHitChancesOH.Add(ResultType.Dodge, enemy.DodgeChance(WeaponSkill[OH.Type]));
                whiteHitChancesOH.Add(ResultType.Parry, enemy.ParryChance());
                whiteHitChancesOH.Add(ResultType.Glancing, GlancingChance(WeaponSkill[OH.Type], enemy.Level));
                whiteHitChancesOH.Add(ResultType.Block, enemy.BlockChance());
                whiteHitChancesOH.Add(ResultType.Crit, RealCritChance(CritRating, whiteHitChancesOH[ResultType.Miss], whiteHitChancesOH[ResultType.Glancing], whiteHitChancesOH[ResultType.Dodge], whiteHitChancesOH[ResultType.Parry], whiteHitChancesOH[ResultType.Block]));
                whiteHitChancesOH.Add(ResultType.Hit, RealHitChance(whiteHitChancesOH[ResultType.Miss], whiteHitChancesOH[ResultType.Glancing], whiteHitChancesOH[ResultType.Crit], whiteHitChancesOH[ResultType.Dodge], whiteHitChancesOH[ResultType.Parry], whiteHitChancesOH[ResultType.Block]));
            }

            Dictionary<ResultType, double> yellowHitChances = new Dictionary<ResultType, double>();
            yellowHitChances.Add(ResultType.Miss, MissChanceYellow(HitRating, WeaponSkill[MH.Type], enemy.Level));
            yellowHitChances.Add(ResultType.Dodge, enemy.DodgeChance(WeaponSkill[MH.Type]));
            yellowHitChances.Add(ResultType.Parry, enemy.ParryChance());
            yellowHitChances.Add(ResultType.Block, enemy.BlockChance());
            yellowHitChances.Add(ResultType.Crit, RealCritChanceYellow(CritRating, yellowHitChances[ResultType.Miss], yellowHitChances[ResultType.Dodge]));
            yellowHitChances.Add(ResultType.Hit, RealHitChanceYellow(yellowHitChances[ResultType.Crit], yellowHitChances[ResultType.Miss], yellowHitChances[ResultType.Dodge]));

            if (HitChancesByEnemy.ContainsKey(enemy))
            {
                HitChancesByEnemy[enemy] = new HitChances(whiteHitChancesMH, yellowHitChances, whiteHitChancesOH);
            }
            else
            {
                HitChancesByEnemy.Add(enemy, new HitChances(whiteHitChancesMH, yellowHitChances, whiteHitChancesOH));
            }
        }

        #region WhiteAttackChances

        public static double MissChance(bool dualWield, double hitRating, int skill, int enemyLevel)
        {
            int enemySkill = enemyLevel * 5;
            int skillDif = Math.Abs(enemySkill - skill);

            return Math.Max(0, Math.Max(BASE_MISS + (dualWield ? 0.19 : 0) - hitRating, 0) + (skillDif > 10 ? 0.02 : 0) + (skillDif > 10 ? (enemySkill - skill - 10) * 0.004 : (enemySkill - skill) * 0.001));
        }

        public static double GlancingChance(int level, int enemyLevel)
        {
            return Math.Max(0, 0.10 + (enemyLevel * 5 - level * 5) * 0.02);
        }

        public static double RealCritChance(double netCritChance, double realMiss, double realGlancing, double enemyDodgeChance, double enemyParryChance, double enemyBlockChance)
        {
            return Math.Max(0, Math.Min(netCritChance, 1 - realMiss - realGlancing - enemyDodgeChance - enemyParryChance - enemyBlockChance));
        }

        public static double RealHitChance(double realMiss, double realGlancing, double realCrit, double enemyDodgeChance, double enemyParryChance, double enemyBlockChance)
        {
            return Math.Max(0, 1 - realMiss - realGlancing - realCrit - enemyDodgeChance - enemyParryChance - enemyBlockChance);
        }

        #endregion

        #region YellowAttackChances


        public static double MissChanceYellow(double hitRating, int skill, int enemyLevel)
        {
            int enemySkill = enemyLevel * 5;
            int skillDif = Math.Abs(enemySkill - skill);

            return Math.Max(0, Math.Max(BASE_MISS - hitRating, 0) + (skillDif > 10 ? 0.02 : 0) + (skillDif > 10 ? (enemySkill - skill - 10) * 0.004 : (enemySkill - skill) * 0.001));
        }

        public static double RealCritChanceYellow(double netCritChance, double realMiss, double enemyDodgeChance)
        {
            return Math.Max(0, netCritChance * (1 - realMiss - enemyDodgeChance));
        }

        public static double RealHitChanceYellow(double realCrit, double realMiss, double enemyDodgeChance)
        {
            return Math.Max(0, 1 - realCrit - realMiss - enemyDodgeChance);
        }

        #endregion

        public ResultType WhiteAttackEnemy(Entity enemy, bool MH)
        {
            if (!HitChancesByEnemy.ContainsKey(enemy))
            {
                CalculateHitChances(enemy);
            }

            return PickFromTable(MH ? HitChancesByEnemy[enemy].WhiteHitChancesMH : HitChancesByEnemy[enemy].WhiteHitChancesOH, Program.random.NextDouble());
        }

        public ResultType YellowAttackEnemy(Entity enemy)
        {
            if (!HitChancesByEnemy.ContainsKey(enemy))
            {
                CalculateHitChances(enemy);
            }

            return PickFromTable(HitChancesByEnemy[enemy].YellowHitChances, Program.random.NextDouble());
        }

        public ResultType PickFromTable(Dictionary<ResultType, double> table, double rand)
        {
            Dictionary<ResultType, double> pickTable = new Dictionary<ResultType, double>(table);

            bool reckless = Effects.Any(e => e is RecklessnessBuff);
            
            if(Effects.Any(e => e is RecklessnessBuff))
            {
                pickTable[ResultType.Hit] = 0;
                pickTable[ResultType.Crit] = 1;
            }
            
            /*
            string debug = "";
            foreach (ResultType type in pickTable.Keys)
            {
                debug += " | " + type + " - " + table[type];
            }
            Console.WriteLine(debug);
            */

            double i = 0;
            foreach(ResultType type in pickTable.Keys)
            {
                i += pickTable[type];
                if(rand <= i)
                {
                    return type;
                }
            }

            throw new Exception("Hit Table Failed");
        }

        public bool DualWielding()
        {
            return OH != null;
        }

        public void StartGCD()
        {
            GCDUntil = Sim.CurrentTime + GCD;
        }

        public bool HasGCD()
        {
            return GCDUntil <= Sim.CurrentTime;
        }

        public void CalculateAttributes()
        {
            Attributes = new Attributes(new Dictionary<Attribute, double>
            {
                // TODO : attributs de base par level par classe
                { Attribute.Stamina, BaseStaByRace(Race) + BonusStaByClass(Class) + 88 },
                { Attribute.Strength, BaseStrByRace(Race) + BonusStrByClass(Class) + 97 },
                { Attribute.Agility, BaseAgiByRace(Race) + BonusAgiByClass(Class) + 60 },
                { Attribute.Intelligence, BaseIntByRace(Race) + BonusIntByClass(Class) },
                { Attribute.Spirit, BaseSpiByRace(Race) + BonusSpiByClass(Class) },
                { Attribute.AP, 160 },
                // Base armor + dodge + etc.
                // Base health + mana ?
            });

            foreach(Item i in Items.Values)
            {
                if(i != null)
                {
                    Attributes += i.Attributes;
                }
            }

            Attributes += BonusAttributesByRace(Race, Attributes);

            Attributes.SetValue(Attribute.Health, 20 + (Attributes.GetValue(Attribute.Stamina) - 20) * 10);
            Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Strength) * StrToAPRatio(Class));
            Attributes.SetValue(Attribute.RangedAP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Agility) * AgiToRangedAPRatio(Class));
            Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance) + Attributes.GetValue(Attribute.Agility) * AgiToCritRatio(Class));

            if(Class == Classes.Warrior)
            {
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + 0.01 * GetTalentPoints("Cruelty")
                    + 0.03); // Berserker Stance
            }
        }

        public static int StrToAPRatio(Classes c)
        {
            return (c == Classes.Druid || c == Classes.Warrior || c == Classes.Shaman || c == Classes.Paladin) ? 2 : 1;
        }

        public static int AgiToRangedAPRatio(Classes c)
        {
            return (c == Classes.Warrior || c == Classes.Hunter || c == Classes.Rogue) ? 2 : 1;
        }

        public static double AgiToCritRatio(Classes c)
        {
            switch(c)
            {
                case Classes.Hunter: return (double)1 / 5300;
                case Classes.Rogue: return (double)1 / 2900;
                default: return (double)1 / 2000;
            }
        }

        public static int BaseStrByRace(Races r)
        {
            switch (r)
            {
                case Races.Human: return 20;
                case Races.Dwarf: return 22;
                case Races.NightElf: return 17;
                case Races.Gnome: return 15;
                case Races.Orc: return 23;
                case Races.Undead: return 19;
                case Races.Tauren: return 25;
                case Races.Troll: return 21;
                default: return 0;
            }
        }

        public static int BaseAgiByRace(Races r)
        {
            switch (r)
            {
                case Races.Human: return 20;
                case Races.Dwarf: return 16;
                case Races.NightElf: return 25;
                case Races.Gnome: return 23;
                case Races.Orc: return 17;
                case Races.Undead: return 18;
                case Races.Tauren: return 15;
                case Races.Troll: return 22;
                default: return 0;
            }
        }

        public static int BaseStaByRace(Races r)
        {
            switch (r)
            {
                case Races.Human: return 20;
                case Races.Dwarf: return 23;
                case Races.NightElf: return 19;
                case Races.Gnome: return 19;
                case Races.Orc: return 22;
                case Races.Undead: return 21;
                case Races.Tauren: return 22;
                case Races.Troll: return 21;
                default: return 0;
            }
        }

        public static int BaseIntByRace(Races r)
        {
            switch (r)
            {
                case Races.Human: return 20;
                case Races.Dwarf: return 19;
                case Races.NightElf: return 20;
                case Races.Gnome: return 24;
                case Races.Orc: return 17;
                case Races.Undead: return 18;
                case Races.Tauren: return 15;
                case Races.Troll: return 16;
                default: return 0;
            }
        }

        public static int BaseSpiByRace(Races r)
        {
            switch (r)
            {
                case Races.Human: return 21;
                case Races.Dwarf: return 19;
                case Races.NightElf: return 20;
                case Races.Gnome: return 20;
                case Races.Orc: return 23;
                case Races.Undead: return 25;
                case Races.Tauren: return 22;
                case Races.Troll: return 21;
                default: return 0;
            }
        }

        public static int BonusStrByClass(Classes c)
        {
            switch (c)
            {
                case Classes.Warrior: return 3;
                default: return 0;
            }
        }

        public static int BonusAgiByClass(Classes c)
        {
            switch (c)
            {
                case Classes.Warrior: return 0;
                default: return 0;
            }
        }

        public static int BonusStaByClass(Classes c)
        {
            switch (c)
            {
                case Classes.Warrior: return 2;
                default: return 0;
            }
        }

        public static int BonusIntByClass(Classes c)
        {
            switch (c)
            {
                case Classes.Warrior: return 0;
                default: return 0;
            }
        }

        public static int BonusSpiByClass(Classes c)
        {
            switch (c)
            {
                case Classes.Warrior: return 0;
                default: return 0;
            }
        }

        public Attributes BonusAttributesByRace(Races r, Attributes a)
        {
            Attributes res = new Attributes();

            switch(r)
            {
                case Races.Gnome:
                    res.SetValue(Attribute.Intelligence, (int)Math.Round(a.GetValue(Attribute.Intelligence) * 0.05));
                    break;
                case Races.Human:
                    res.SetValue(Attribute.Spirit, (int)Math.Round(a.GetValue(Attribute.Spirit) * 0.05));
                    break;
                default: break;
            }

            return res;
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

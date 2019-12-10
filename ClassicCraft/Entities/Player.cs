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

        public static Races ToRace(string s)
        {
            switch (s)
            {
                case "Human": return Races.Human;
                case "Dwarf": return Races.Dwarf;
                case "NightElf": return Races.NightElf;
                case "Gnome": return Races.Gnome;
                case "Orc": return Races.Orc;
                case "Undead": return Races.Undead;
                case "Tauren": return Races.Tauren;
                case "Troll": return Races.Troll;
                default: throw new Exception("Race not found");
            }
        }

        public static string FromRace(Races r)
        {
            switch (r)
            {
                case Races.Human: return "Human";
                case Races.Dwarf: return "Dwarf";
                case Races.NightElf: return "NightElf";
                case Races.Gnome: return "Gnome";
                case Races.Orc: return "Orc";
                case Races.Undead: return "Undead";
                case Races.Tauren: return "Tauren";
                case Races.Troll: return "Troll";
                default: throw new Exception("Race not found");
            }
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

        public static Classes ToClass(string s)
        {
            switch(s)
            {
                case "Priest": return Classes.Priest;
                case "Rogue": return Classes.Rogue;
                case "Warrior": return Classes.Warrior;
                case "Mage": return Classes.Mage;
                case "Druid": return Classes.Druid;
                case "Hunter": return Classes.Hunter;
                case "Warlock": return Classes.Warlock;
                case "Shaman": return Classes.Shaman;
                case "Paladin": return Classes.Paladin;
                default: throw new Exception("Class not found");
            }
        }

        public static string FromClass(Classes c)
        {
            switch(c)
            {
                case Classes.Priest: return "Priest";
                case Classes.Rogue: return "Rogue";
                case Classes.Warrior: return "Warrior";
                case Classes.Mage: return "Mage";
                case Classes.Druid: return "Druid";
                case Classes.Hunter: return "Hunter";
                case Classes.Warlock: return "Warlock";
                case Classes.Shaman: return "Shaman";
                case Classes.Paladin: return "Paladin";
                default: throw new Exception("Class not found");
            }
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
            OH,
            Ranged
        }

        public static Slot ToSlot(string s)
        {
            switch(s)
            {
                case "Head": return Slot.Head;
                case "Neck": return Slot.Neck;
                case "Shoulders": return Slot.Shoulders;
                case "Back": return Slot.Back;
                case "Chest": return Slot.Chest;
                case "Wrist": return Slot.Wrist;
                case "Hands": return Slot.Hands;
                case "Waist": return Slot.Waist;
                case "Legs": return Slot.Legs;
                case "Feet": return Slot.Feet;
                case "Ring1": return Slot.Ring1;
                case "Ring2": return Slot.Ring2;
                case "Trinket1": return Slot.Trinket1;
                case "Trinket2": return Slot.Trinket2;
                case "MH": return Slot.MH;
                case "OH": return Slot.OH;
                case "Ranged": return Slot.Ranged;
                default: throw new Exception("Slot not found");
            }
        }

        public static string FromSlot(Slot s)
        {
            switch(s)
            {
                case Slot.Head: return "Head";
                case Slot.Neck: return "Neck";
                case Slot.Shoulders: return "Shoulders";
                case Slot.Back: return "Back";
                case Slot.Chest: return "Chest";
                case Slot.Wrist: return "Wrist";
                case Slot.Hands: return "Hands";
                case Slot.Waist: return "Waist";
                case Slot.Legs: return "Legs";
                case Slot.Feet: return "Feet";
                case Slot.Ring1: return "Ring1";
                case Slot.Ring2: return "Ring2";
                case Slot.Trinket1: return "Trinket1";
                case Slot.Trinket2: return "Trinket2";
                case Slot.MH: return "MH";
                case Slot.OH: return "OH";
                case Slot.Ranged: return "Ranged";
                default: throw new Exception("Slot not found");
            }
        }

        #region Propriétés

        public static double GCD = 1.5;

        private int resource;
        public int Resource
        {
            get
            {
                return resource;
            }
            set
            {
                if (value > 100)
                {
                    resource = 100;
                }
                else if (value < 0)
                {
                    resource = 0;
                }
                else
                {
                    resource = value;
                }
            }
        }

        private int mana;
        public int Mana
        {
            get
            {
                return mana;
            }
            set
            {
                if (value < 0)
                {
                    mana = 0;
                }
                else
                {
                    mana = value;
                }
            }
        }

        private int combo;
        public int Combo
        {
            get
            {
                return combo;
            }
            set
            {
                if (value < 0)
                {
                    combo = 0;
                }
                else if (value > 5)
                {
                    combo = 5;
                }
                else
                {
                    combo = value;
                }
            }
        }


        public Dictionary<Slot, Item> Equipment { get; set; }

        public Dictionary<string, int> Talents { get; set; }

        public List<Enchantment> Buffs { get; set; }

        public Weapon MH
        {
            get
            {
                return (Weapon)Equipment[Slot.MH];
            }
            set
            {
                Equipment[Slot.MH] = value;
            }
        }

        public Weapon OH
        {
            get
            {
                if (Equipment[Slot.OH] != null)
                {
                    return (Weapon)Equipment[Slot.OH];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Equipment[Slot.OH] = value;
            }
        }

        public Weapon Ranged
        {
            get
            {
                if (Equipment[Slot.Ranged] != null)
                {
                    return (Weapon)Equipment[Slot.Ranged];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Equipment[Slot.Ranged] = value;
            }
        }

        public double GCDUntil { get; set; }
        public double EnergyTick { get; set; }

        public double AP
        {
            get
            {
                return Attributes.GetValue(Attribute.AP) + BonusAttributes.GetValue(Attribute.AP);
            }
        }

        public double CritRating
        {
            get
            {
                return Attributes.GetValue(Attribute.CritChance) + BonusAttributes.GetValue(Attribute.CritChance);
            }
        }

        public double HitRating
        {
            get
            {
                return Attributes.GetValue(Attribute.HitChance) + BonusAttributes.GetValue(Attribute.HitChance);
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

        public bool WindfuryTotem { get; set; }

        public List<string> Cooldowns { get; set; }

        #endregion

        public Player(Player p, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : this(null, p, items, talents, buffs)
        {
        }

        public Player(Simulation s, Player p, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : this(s, p.Class, p.Race, p.Level, p.Armor, p.MaxLife, items, talents, buffs)
        {
        }

        public Player(Simulation s = null, Classes c = Classes.Warrior, Races r = Races.Orc, int level = 60, int armor = 0, int maxLife = 1000, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, level, armor, maxLife)
        {
            Race = r;
            Class = c;
            
            Talents = talents;
            
            Buffs = buffs;

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

            if(items == null)
            {
                Equipment = new Dictionary<Slot, Item>();
                foreach (Slot slot in (Slot[])Enum.GetValues(typeof(Slot)))
                {
                    Equipment[slot] = null;
                }
            }
            else
            {
                Equipment = new Dictionary<Slot, Item>(items);
            }

            HitChancesByEnemy = new Dictionary<Entity, HitChances>();

            Reset();
        }

        public int GetTalentPoints(string talent)
        {
            return Talents != null && Talents.ContainsKey(talent) ? Talents[talent] : 0;
        }

        public override void Reset()
        {
            base.Reset();

            Resource = 0;

            GCDUntil = 0;
            EnergyTick = 0;
            Combo = 0;

            HasteMod = 1;
            DamageMod = 1;

            BonusAttributes = new Attributes();
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
                whiteHitChancesOH.Add(ResultType.Miss, MissChance(true, HitRating + (OH.Buff == null ? 0 : OH.Buff.Attributes.GetValue(Attribute.HitChance)), WeaponSkill[OH.Type], enemy.Level));
                whiteHitChancesOH.Add(ResultType.Dodge, enemy.DodgeChance(WeaponSkill[OH.Type]));
                whiteHitChancesOH.Add(ResultType.Parry, enemy.ParryChance());
                whiteHitChancesOH.Add(ResultType.Glancing, GlancingChance(WeaponSkill[OH.Type], enemy.Level));
                whiteHitChancesOH.Add(ResultType.Block, enemy.BlockChance());
                whiteHitChancesOH.Add(ResultType.Crit, RealCritChance(CritRating + (OH.Buff == null ? 0 : OH.Buff.Attributes.GetValue(Attribute.CritChance)), whiteHitChancesOH[ResultType.Miss], whiteHitChancesOH[ResultType.Glancing], whiteHitChancesOH[ResultType.Dodge], whiteHitChancesOH[ResultType.Parry], whiteHitChancesOH[ResultType.Block]));
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

        // TODO : Revoir les valeurs pour Classic par rapport au weapon skill
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

            return PickFromTable(MH ? HitChancesByEnemy[enemy].WhiteHitChancesMH : HitChancesByEnemy[enemy].WhiteHitChancesOH, Sim.random.NextDouble());
        }

        public ResultType YellowAttackEnemy(Entity enemy)
        {
            if (!HitChancesByEnemy.ContainsKey(enemy))
            {
                CalculateHitChances(enemy);
            }

            return PickFromTable(HitChancesByEnemy[enemy].YellowHitChances, Sim.random.NextDouble());
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
            string debug = "rand " + rand;
            foreach (ResultType type in pickTable.Keys)
            {
                debug += " | " + type + " - " + table[type];
            }
            if(Program.logFight)
            {
                Program.Log(debug);
            }
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

        public void CheckEnergyTick()
        {
            if(Sim.CurrentTime >= EnergyTick + 2)
            {
                // Meh
                EnergyTick = EnergyTick + 2;
                Resource += 20;
            }
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
                // Base health + mana
            });

            foreach(Slot s in Equipment.Keys.Where(v => Equipment[v] != null))
            {
                Item i = Equipment[s];
                Attributes += i.Attributes;
                if (i.Enchantment != null)
                {
                    Attributes += i.Enchantment.Attributes;
                }
                if (s == Slot.MH)
                {
                    Weapon w = ((Weapon)i);
                    if (w.Buff != null)
                    Attributes += w.Buff.Attributes;
                }
            }

            int wbonus = (int)Math.Round(Attributes.GetValue(Attribute.WeaponDamage));
            MH.DamageMin += wbonus;
            MH.DamageMax += wbonus;
            if(OH != null)
            {
                OH.DamageMin += wbonus;
                OH.DamageMax += wbonus;
            }

            Attributes += BonusAttributesByRace(Race, Attributes);

            if(Class == Classes.Druid)
            {
                Attributes.SetValue(Attribute.Intelligence, Attributes.GetValue(Attribute.Intelligence)
                    * (1 + 0.04 * GetTalentPoints("HW")));
                Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength)
                    * (1 + 0.04 * GetTalentPoints("HW")));
            }

            Attributes.SetValue(Attribute.Health, 20 + (Attributes.GetValue(Attribute.Stamina) - 20) * 10);
            Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Strength) * StrToAPRatio(Class) + Attributes.GetValue(Attribute.Agility) * AgiToAPRatio(Class));
            Attributes.SetValue(Attribute.RangedAP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Agility) * AgiToRangedAPRatio(Class));
            Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance) + Attributes.GetValue(Attribute.Agility) * AgiToCritRatio(Class));

            HasteMod = 1 + Attributes.GetValue(Attribute.AS);

            if (Class == Classes.Warrior)
            {
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + 0.01 * GetTalentPoints("Cruelty")
                    + 0.03); // Berserker Stance, move elsewhere for stance dancing ?
            }
            else if (Class == Classes.Druid)
            {
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + 0.01 * GetTalentPoints("SC"));
                Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP)
                    + (0.5 * GetTalentPoints("PS")) * Level);
            }

            foreach (Enchantment e in Buffs.Where(v => v != null))
            {
                Attributes += e.Attributes;
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

        public static int AgiToAPRatio(Classes c)
        {
            return (c == Classes.Druid || c == Classes.Hunter || c == Classes.Rogue) ? 1 : 0;
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

        public Dictionary<Spell, int> CooldownsListToSpellsDic(List<string> cds)
        {
            Dictionary<Spell, int> res = new Dictionary<Spell, int>();

            foreach(string s in cds)
            {
                switch(s)
                {
                    case "DeathWish": res.Add(new DeathWish(this), DeathWishBuff.LENGTH); break;
                    case "JujuFlurry": res.Add(new JujuFlurry(this), JujuFlurryBuff.LENGTH); break;
                    case "MightyRage": res.Add(new MightyRage(this), MightyRageBuff.LENGTH); break;
                    case "Recklessness": res.Add(new Recklessness(this), RecklessnessBuff.LENGTH); break;
                    case "BloodFury": res.Add(new BloodFury(this), BloodFuryBuff.LENGTH); break;
                    case "Berserking": res.Add(new Berserking(this), BerserkingBuff.LENGTH); break;
                }
            }

            return res;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} level {2} | Stats : {3}", Race, Class, Level, Attributes);
        }
    }
}

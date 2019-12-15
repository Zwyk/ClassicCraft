using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Player : Entity
    {
        #region Util

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

        #endregion

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

        public double BaseMana { get; set; }

        public int MaxMana
        {
            get { return (int)Attributes.GetValue(Attribute.Mana); }
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
                    value = 0;
                }
                else if(value > MaxMana)
                {
                    value = MaxMana;
                }

                if (value < mana)
                {
                    LastManaExpenditure = Sim.CurrentTime;
                }

                mana = value;
            }
        }

        public double LastManaExpenditure = -1000;

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
        public double ResourcesTick { get; set; }
        public double ManaTick { get; set; }

        public double MPTRatio { get; set; }
        public double CastingRegenPct { get; set; }

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
            ResourcesTick = 0;
            Combo = 0;

            HasteMod = CalcHaste();
            DamageMod = 1;
            MPTRatio = 1;
            CastingRegenPct = 0;

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

            return Math.Max(0, BASE_MISS + (dualWield ? 0.19 : 0) - hitRating + (skillDif > 10 ? 0.02 : 0) + (skillDif > 10 ? (enemySkill - skill - 10) * 0.004 : (enemySkill - skill) * 0.001));
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

            return Math.Max(0, BASE_MISS - hitRating + (skillDif > 10 ? 0.02 : 0) + (skillDif > 10 ? (enemySkill - skill - 10) * 0.004 : (enemySkill - skill) * 0.001));
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

            return PickFromTable(MH ? HitChancesByEnemy[enemy].WhiteHitChancesMH : HitChancesByEnemy[enemy].WhiteHitChancesOH, Randomer.NextDouble());
        }

        public ResultType YellowAttackEnemy(Entity enemy)
        {
            if (!HitChancesByEnemy.ContainsKey(enemy))
            {
                CalculateHitChances(enemy);
            }

            return PickFromTable(HitChancesByEnemy[enemy].YellowHitChances, Randomer.NextDouble());
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
            if (Sim.CurrentTime >= ResourcesTick + 2)
            {
                ResourcesTick = ResourcesTick + 2;
                Resource += 20;
            }
        }

        public double ManaPct()
        {
            return (double)Mana / MaxMana;
        }

        public bool ManaTicking()
        {
            return CastingRegenPct > 0 || Sim.CurrentTime >= LastManaExpenditure + 5;
        }

        public void CheckManaTick()
        {
            if (ManaTicking() && Sim.CurrentTime >= ManaTick + 2)
            {
                ManaTick = ManaTick < LastManaExpenditure + 5 ? (CastingRegenPct > 0 ? Sim.CurrentTime : LastManaExpenditure + 5) : ManaTick + 2;
                Mana += (int)(MPT() * (Sim.CurrentTime < LastManaExpenditure + 5 ? CastingRegenPct : 1) * MPTRatio);
            }
        }

        public double MPT()
        {
            switch(Class)
            {
                case Classes.Warlock: return 8 + Attributes.GetValue(Attribute.Spirit) / 4;
                case Classes.Mage: return 13 + Attributes.GetValue(Attribute.Spirit) / 4;
                case Classes.Priest: return 13 + Attributes.GetValue(Attribute.Spirit) / 4;
                default: return 15 + Attributes.GetValue(Attribute.Spirit) / 5;
            }
        }

        public void CalculateAttributes()
        {
            Attributes = CaseAttributes(Class, Race);
            BaseMana = Attributes.GetValue(Attribute.Mana);

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

            foreach (Enchantment e in Buffs.Where(v => v != null))
            {
                Attributes += e.Attributes;
            }

            if (Class == Classes.Warrior)
            {
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + 0.01 * GetTalentPoints("Cruelty")
                    + 0.03); // Berserker Stance, move elsewhere for stance dancing ?
            }
            else if (Class == Classes.Druid)
            {
                Attributes.SetValue(Attribute.Intellect, Attributes.GetValue(Attribute.Intellect)
                    * (1 + 0.04 * GetTalentPoints("HW")));
                Attributes.SetValue(Attribute.Strength, Attributes.GetValue(Attribute.Strength)
                    * (1 + 0.04 * GetTalentPoints("HW")));
                Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance)
                    + 0.01 * GetTalentPoints("SC"));
                Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP)
                    + 0.5 * GetTalentPoints("PS") * Level);
            }

            Attributes += BonusAttributesByRace(Race, Attributes);

            Attributes.SetValue(Attribute.Health, Attributes.GetValue(Attribute.Health) + 20 + (Attributes.GetValue(Attribute.Stamina) - 20) * 10);
            if(Attributes.GetValue(Attribute.Mana) > 0)
            {
                Attributes.SetValue(Attribute.Mana, Attributes.GetValue(Attribute.Mana) + Attributes.GetValue(Attribute.Intellect) * 15);
            }
            Attributes.SetValue(Attribute.AP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Strength) * StrToAPRatio(Class) + Attributes.GetValue(Attribute.Agility) * AgiToAPRatio(Class));
            Attributes.SetValue(Attribute.RangedAP, Attributes.GetValue(Attribute.AP) + Attributes.GetValue(Attribute.Agility) * AgiToRangedAPRatio(Class));
            Attributes.SetValue(Attribute.CritChance, Attributes.GetValue(Attribute.CritChance) + Attributes.GetValue(Attribute.Agility) * AgiToCritRatio(Class));

            HasteMod = CalcHaste();
        }

        public double CalcHaste()
        {
            return 1 + (Attributes != null ? Attributes.GetValue(Attribute.Haste) : 0);
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

        public static Attributes CaseAttributes(Classes c, Races r, int level = 60)
        {
            // TODO : by level

            Attributes res = new Attributes();

            switch (c)
            {
                case Classes.Druid:
                    switch (r)
                    {
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 62);
                            res.Values.Add(Attribute.Agility, 65);
                            res.Values.Add(Attribute.Stamina, 69);
                            res.Values.Add(Attribute.Intellect, 100);
                            res.Values.Add(Attribute.Spirit, 110);
                            res.Values.Add(Attribute.Health, 1483);
                            res.Values.Add(Attribute.Mana, 1244);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 70);
                            res.Values.Add(Attribute.Agility, 55);
                            res.Values.Add(Attribute.Stamina, 72);
                            res.Values.Add(Attribute.Intellect, 95);
                            res.Values.Add(Attribute.Spirit, 112);
                            res.Values.Add(Attribute.Health, 1483);
                            res.Values.Add(Attribute.Mana, 1244);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Hunter:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 57);
                            res.Values.Add(Attribute.Agility, 121);
                            res.Values.Add(Attribute.Stamina, 93);
                            res.Values.Add(Attribute.Intellect, 64);
                            res.Values.Add(Attribute.Spirit, 69);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 52);
                            res.Values.Add(Attribute.Agility, 130);
                            res.Values.Add(Attribute.Stamina, 89);
                            res.Values.Add(Attribute.Intellect, 65);
                            res.Values.Add(Attribute.Spirit, 70);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 58);
                            res.Values.Add(Attribute.Agility, 122);
                            res.Values.Add(Attribute.Stamina, 92);
                            res.Values.Add(Attribute.Intellect, 62);
                            res.Values.Add(Attribute.Spirit, 73);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 60);
                            res.Values.Add(Attribute.Agility, 120);
                            res.Values.Add(Attribute.Stamina, 92);
                            res.Values.Add(Attribute.Intellect, 60);
                            res.Values.Add(Attribute.Spirit, 72);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 56);
                            res.Values.Add(Attribute.Agility, 127);
                            res.Values.Add(Attribute.Stamina, 91);
                            res.Values.Add(Attribute.Intellect, 61);
                            res.Values.Add(Attribute.Spirit, 71);
                            res.Values.Add(Attribute.Health, 1467);
                            res.Values.Add(Attribute.Mana, 1720);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Mage:
                    switch (r)
                    {
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 25);
                            res.Values.Add(Attribute.Agility, 38);
                            res.Values.Add(Attribute.Stamina, 44);
                            res.Values.Add(Attribute.Intellect, 133);
                            res.Values.Add(Attribute.Spirit, 120);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 30);
                            res.Values.Add(Attribute.Agility, 35);
                            res.Values.Add(Attribute.Stamina, 45);
                            res.Values.Add(Attribute.Intellect, 125);
                            res.Values.Add(Attribute.Spirit, 126);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 31);
                            res.Values.Add(Attribute.Agility, 37);
                            res.Values.Add(Attribute.Stamina, 46);
                            res.Values.Add(Attribute.Intellect, 121);
                            res.Values.Add(Attribute.Spirit, 121);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 29);
                            res.Values.Add(Attribute.Agility, 33);
                            res.Values.Add(Attribute.Stamina, 46);
                            res.Values.Add(Attribute.Intellect, 123);
                            res.Values.Add(Attribute.Spirit, 125);
                            res.Values.Add(Attribute.Health, 1360);
                            res.Values.Add(Attribute.Mana, 1273);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Paladin:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 107);
                            res.Values.Add(Attribute.Agility, 61);
                            res.Values.Add(Attribute.Stamina, 103);
                            res.Values.Add(Attribute.Intellect, 69);
                            res.Values.Add(Attribute.Spirit, 74);
                            res.Values.Add(Attribute.Health, 1381);
                            res.Values.Add(Attribute.Mana, 1512);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 105);
                            res.Values.Add(Attribute.Agility, 65);
                            res.Values.Add(Attribute.Stamina, 100);
                            res.Values.Add(Attribute.Intellect, 70);
                            res.Values.Add(Attribute.Spirit, 78);
                            res.Values.Add(Attribute.Health, 1381);
                            res.Values.Add(Attribute.Mana, 1510);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Priest:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 37);
                            res.Values.Add(Attribute.Agility, 36);
                            res.Values.Add(Attribute.Stamina, 53);
                            res.Values.Add(Attribute.Intellect, 119);
                            res.Values.Add(Attribute.Spirit, 124);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 35);
                            res.Values.Add(Attribute.Agility, 40);
                            res.Values.Add(Attribute.Stamina, 50);
                            res.Values.Add(Attribute.Intellect, 120);
                            res.Values.Add(Attribute.Spirit, 131);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 32);
                            res.Values.Add(Attribute.Agility, 45);
                            res.Values.Add(Attribute.Stamina, 49);
                            res.Values.Add(Attribute.Intellect, 120);
                            res.Values.Add(Attribute.Spirit, 125);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 36);
                            res.Values.Add(Attribute.Agility, 42);
                            res.Values.Add(Attribute.Stamina, 51);
                            res.Values.Add(Attribute.Intellect, 116);
                            res.Values.Add(Attribute.Spirit, 126);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 34);
                            res.Values.Add(Attribute.Agility, 38);
                            res.Values.Add(Attribute.Stamina, 51);
                            res.Values.Add(Attribute.Intellect, 118);
                            res.Values.Add(Attribute.Spirit, 130);
                            res.Values.Add(Attribute.Health, 1387);
                            res.Values.Add(Attribute.Mana, 1436);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Rogue:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 82);
                            res.Values.Add(Attribute.Agility, 126);
                            res.Values.Add(Attribute.Stamina, 78);
                            res.Values.Add(Attribute.Intellect, 34);
                            res.Values.Add(Attribute.Spirit, 49);
                            res.Values.Add(Attribute.Health, 1523);
                            break;
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 75);
                            res.Values.Add(Attribute.Agility, 133);
                            res.Values.Add(Attribute.Stamina, 74);
                            res.Values.Add(Attribute.Intellect, 40);
                            res.Values.Add(Attribute.Spirit, 50);
                            res.Values.Add(Attribute.Health, 1523);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 80);
                            res.Values.Add(Attribute.Agility, 130);
                            res.Values.Add(Attribute.Stamina, 75);
                            res.Values.Add(Attribute.Intellect, 35);
                            res.Values.Add(Attribute.Spirit, 52);
                            res.Values.Add(Attribute.Health, 1523);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 77);
                            res.Values.Add(Attribute.Agility, 135);
                            res.Values.Add(Attribute.Stamina, 74);
                            res.Values.Add(Attribute.Intellect, 35);
                            res.Values.Add(Attribute.Spirit, 50);
                            res.Values.Add(Attribute.Health, 1523);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 83);
                            res.Values.Add(Attribute.Agility, 127);
                            res.Values.Add(Attribute.Stamina, 77);
                            res.Values.Add(Attribute.Intellect, 32);
                            res.Values.Add(Attribute.Spirit, 53);
                            res.Values.Add(Attribute.Health, 1523);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 81);
                            res.Values.Add(Attribute.Agility, 132);
                            res.Values.Add(Attribute.Stamina, 76);
                            res.Values.Add(Attribute.Intellect, 31);
                            res.Values.Add(Attribute.Spirit, 51);
                            res.Values.Add(Attribute.Health, 1523);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 79);
                            res.Values.Add(Attribute.Agility, 128);
                            res.Values.Add(Attribute.Stamina, 76);
                            res.Values.Add(Attribute.Intellect, 33);
                            res.Values.Add(Attribute.Spirit, 55);
                            res.Values.Add(Attribute.Health, 1523);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Shaman:
                    switch (r)
                    {
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 88);
                            res.Values.Add(Attribute.Agility, 52);
                            res.Values.Add(Attribute.Stamina, 97);
                            res.Values.Add(Attribute.Intellect, 87);
                            res.Values.Add(Attribute.Spirit, 103);
                            res.Values.Add(Attribute.Health, 1280);
                            res.Values.Add(Attribute.Mana, 1520);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 90);
                            res.Values.Add(Attribute.Agility, 50);
                            res.Values.Add(Attribute.Stamina, 97);
                            res.Values.Add(Attribute.Intellect, 85);
                            res.Values.Add(Attribute.Spirit, 102);
                            res.Values.Add(Attribute.Health, 1280);
                            res.Values.Add(Attribute.Mana, 1520);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 86);
                            res.Values.Add(Attribute.Agility, 57);
                            res.Values.Add(Attribute.Stamina, 96);
                            res.Values.Add(Attribute.Intellect, 86);
                            res.Values.Add(Attribute.Spirit, 101);
                            res.Values.Add(Attribute.Health, 1280);
                            res.Values.Add(Attribute.Mana, 1520);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Warlock:
                    switch (r)
                    {
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 40);
                            res.Values.Add(Attribute.Agility, 53);
                            res.Values.Add(Attribute.Stamina, 64);
                            res.Values.Add(Attribute.Intellect, 119);
                            res.Values.Add(Attribute.Spirit, 115);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 45);
                            res.Values.Add(Attribute.Agility, 50);
                            res.Values.Add(Attribute.Stamina, 65);
                            res.Values.Add(Attribute.Intellect, 110);
                            res.Values.Add(Attribute.Spirit, 120);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 48);
                            res.Values.Add(Attribute.Agility, 47);
                            res.Values.Add(Attribute.Stamina, 66);
                            res.Values.Add(Attribute.Intellect, 107);
                            res.Values.Add(Attribute.Spirit, 118);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 44);
                            res.Values.Add(Attribute.Agility, 48);
                            res.Values.Add(Attribute.Stamina, 66);
                            res.Values.Add(Attribute.Intellect, 108);
                            res.Values.Add(Attribute.Spirit, 120);
                            res.Values.Add(Attribute.Health, 1414);
                            res.Values.Add(Attribute.Mana, 1373);
                            break;
                        default: break;
                    }
                    break;
                case Classes.Warrior:
                    switch (r)
                    {
                        case Races.Dwarf:
                            res.Values.Add(Attribute.Strength, 122);
                            res.Values.Add(Attribute.Agility, 76);
                            res.Values.Add(Attribute.Stamina, 113);
                            res.Values.Add(Attribute.Intellect, 29);
                            res.Values.Add(Attribute.Spirit, 44);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        case Races.Gnome:
                            res.Values.Add(Attribute.Strength, 115);
                            res.Values.Add(Attribute.Agility, 83);
                            res.Values.Add(Attribute.Stamina, 109);
                            res.Values.Add(Attribute.Intellect, 35);
                            res.Values.Add(Attribute.Spirit, 45);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        case Races.Human:
                            res.Values.Add(Attribute.Strength, 120);
                            res.Values.Add(Attribute.Agility, 80);
                            res.Values.Add(Attribute.Stamina, 110);
                            res.Values.Add(Attribute.Intellect, 30);
                            res.Values.Add(Attribute.Spirit, 47);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        case Races.NightElf:
                            res.Values.Add(Attribute.Strength, 117);
                            res.Values.Add(Attribute.Agility, 85);
                            res.Values.Add(Attribute.Stamina, 109);
                            res.Values.Add(Attribute.Intellect, 30);
                            res.Values.Add(Attribute.Spirit, 45);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        case Races.Orc:
                            res.Values.Add(Attribute.Strength, 123);
                            res.Values.Add(Attribute.Agility, 77);
                            res.Values.Add(Attribute.Stamina, 112);
                            res.Values.Add(Attribute.Intellect, 27);
                            res.Values.Add(Attribute.Spirit, 48);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        case Races.Tauren:
                            res.Values.Add(Attribute.Strength, 125);
                            res.Values.Add(Attribute.Agility, 75);
                            res.Values.Add(Attribute.Stamina, 112);
                            res.Values.Add(Attribute.Intellect, 27);
                            res.Values.Add(Attribute.Spirit, 47);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        case Races.Troll:
                            res.Values.Add(Attribute.Strength, 121);
                            res.Values.Add(Attribute.Agility, 82);
                            res.Values.Add(Attribute.Stamina, 111);
                            res.Values.Add(Attribute.Intellect, 26);
                            res.Values.Add(Attribute.Spirit, 46);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        case Races.Undead:
                            res.Values.Add(Attribute.Strength, 119);
                            res.Values.Add(Attribute.Agility, 78);
                            res.Values.Add(Attribute.Stamina, 111);
                            res.Values.Add(Attribute.Intellect, 28);
                            res.Values.Add(Attribute.Spirit, 50);
                            res.Values.Add(Attribute.Health, 1689);
                            break;
                        default: break;
                    }
                    break;
                default: break;
            }

            return res;
        }
        
        public Attributes BonusAttributesByRace(Races r, Attributes a)
        {
            Attributes res = new Attributes();

            switch(r)
            {
                case Races.Gnome:
                    res.SetValue(Attribute.Intellect, a.GetValue(Attribute.Intellect) * 0.05);
                    break;
                case Races.Human:
                    res.SetValue(Attribute.Spirit, a.GetValue(Attribute.Spirit) * 0.05);
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

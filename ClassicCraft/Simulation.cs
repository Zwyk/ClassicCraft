using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SimResult
    {
        public double FightLength;
        public List<RegisteredAction> Actions;
        public List<RegisteredEffect> Effects;

        public SimResult(double fightLength)
        {
            FightLength = fightLength;
            Actions = new List<RegisteredAction>();
            Effects = new List<RegisteredEffect>();
        }
    }

    public class Simulation
    {
        public static double RATE = 40;

        public Player Player { get; set; }
        public Boss Boss { get; set; }
        public double FightLength { get; set; }

        public SimResult Results { get; set; }
        public double Damage { get; set; }
        public double Threat { get; set; }

        public double CurrentTime { get; set; }
        public double TimeLeft
        {
            get
            {
                return FightLength - CurrentTime;
            }
        }

        public bool AutoLife { get; set; }
        public double LowLifeTime { get; set; }

        public bool UnlimitedMana { get; set; }
        public bool UnlimitedResource { get; set; }
        public bool Tanking { get; set; }
        public double TankHitEvery { get; set; }
        public double TankHitRage { get; set; }
        public int NbTargets { get; set; }
        public bool DoThreat { get; set; }

        public double LastHit { get; set; }

        public bool Ended { get; set; }

        public Simulation(Player player, Boss boss, double fightLength, bool autoBossLife = true, double lowLifeTime = 0, double fightLengthMod = 0.2, bool unlimitedMana = false, bool unlimitedResource = false, bool tanking = false, double tankHitEvery = 1, double tankHitRage = 25, int nbTargets = 1, bool doThreat = false)
        {
            Player = player;
            Boss = boss;
            player.Sim = this;
            Boss.Sim = this;
            FightLength = fightLength * (1 + fightLengthMod/2 - (Randomer.NextDouble() * fightLengthMod));
            Results = new SimResult(FightLength);
            Damage = 0;
            Threat = 0;
            CurrentTime = 0;
            AutoLife = autoBossLife;
            LowLifeTime = lowLifeTime;

            UnlimitedMana = unlimitedMana;
            UnlimitedResource = unlimitedResource;
            Tanking = tanking;
            TankHitEvery = tankHitEvery;
            TankHitRage = tankHitRage;
            NbTargets = nbTargets;
            
            DoThreat = doThreat;

            LastHit = -TankHitEvery;

            Ended = false;
        }

        public void StartSim()
        {
            Player.PrepFight();

            CurrentTime = 0;
            Boss.LifePct = 1;
            
            /*
            if (Randomer.NextDouble() < 0.002)
            {
                Program.Log(Player);
            }
            */

            while (CurrentTime < FightLength)
            {
                SimTick();

                CurrentTime += 1 / RATE;
            }
            
            Program.AddSimDps(Damage / FightLength);
            if(DoThreat)
            {
                Program.AddSimThreat(Threat / FightLength);
            }

            if (!Program.statsWeights)
            {
                Program.AddSimResult(Results);
            }

            Ended = true;
        }

        public void SimTick()
        {
            if (AutoLife)
            {
                Boss.LifePct = Math.Max(0, 1 - (CurrentTime / FightLength) * (Program.version == Version.Vanilla ? 16.0 / 17.0 : 1));
            }
            else if (CurrentTime >= LowLifeTime && Boss.LifePct == 1)
            {
                Boss.LifePct = 0.10;
            }

            foreach (Effect e in new List<Effect>(Boss.Effects.Values))
            {
                e.CheckEffect();
            }

            foreach (Effect e in new List<Effect>(Player.Effects.Values))
            {
                e.CheckEffect();
            }

            if(Tanking && TankHitEvery > 0 && TankHitRage > 0 && (Player.Class == Player.Classes.Warrior || Player.Class == Player.Classes.Paladin || Player.Form == Player.Forms.Bear) && LastHit + TankHitEvery <= CurrentTime)
            {
                Player.Resource += (int)TankHitRage;

                if (Program.logFight)
                {
                    Program.Log(string.Format("{0:N2} : Boss hits for {1} rage :  ({2} {3}/{4})", CurrentTime, (int)TankHitRage, "rage", Player.Resource, Player.MaxResource));
                }

                LastHit += TankHitEvery;

                if(Player.Class == Player.Classes.Warrior && Player.NbSet("Destroyer") >= 4 && Randomer.NextDouble() < 0.07)
                {
                    if (Player.Effects.ContainsKey("T5 4P")) Player.Effects["T5 4P"].Refresh();
                    else new CustomStatsBuff(Player, "T5 4P", 10, 1, new Dictionary<Attribute, double>() { { Attribute.Haste, 200 / Player.RatingRatios[Attribute.Haste] / 100 } }).StartEffect();
                }
            }

            if (Player.casting != null)
            {
                if (Player.casting is ChannelSpell)
                {
                    ((ChannelSpell)Player.casting).CheckTick();
                }

                if (Player.casting.CastFinish <= CurrentTime)
                {
                    Player.casting.DoAction();
                }
            }

            if (Player.Class == Player.Classes.Rogue || Player.Form == Player.Forms.Cat)
            {
                Player.CheckEnergyTick();
            }
            if(Player.Class == Player.Classes.Warrior && Player.GetTalentPoints("AM") > 0)
            {
                (Player as Warrior).CheckAngerManagementTick();
            }
            if (Player.MaxMana > 0)
            {
                Player.CheckSpiritTick();
                Player.CheckMPT();
            }

            Player.Rota();
        }

        public void RegisterAction(RegisteredAction action)
        {
            if(!Program.statsWeights)
            {
                Results.Actions.Add(action);
            }
            Damage += action.Result.Damage;
            if (DoThreat)
            {
                Threat += action.Result.Threat;
            }
        }

        public void RegisterEffect(RegisteredEffect effect)
        {
            if (!Program.statsWeights)
            {
                Results.Effects.Add(effect);
            }
            Damage += effect.Damage;
            if (DoThreat)
            {
                Threat += effect.Threat;
            }
        }

        public static double Normalization(Weapon w)
        {
            if(Program.version == Version.Vanilla)
            {
                return w.Speed;
            }
            else
            {
                if (w.Type == Weapon.WeaponType.Dagger)
                {
                    return 1.7;
                }
                else if (w.TwoHanded)
                {
                    return 3.3;
                }
                else
                {
                    return 2.4;
                }
            }
        }

        public double DamageMod(ResultType type, School school = School.Physical, bool MH = true, bool isWeapon = false)
        {
            switch (type)
            {
                // TODO BLOCK / BLOCKCRIT
                case ResultType.Crit: return (school == School.Physical || isWeapon) ? 2 : 1.5;
                case ResultType.Hit: return 1;
                case ResultType.Glance: return GlancingDamage(Player.WeaponSkill[MH ? Player.MH.Type : Player.OH.Type], Boss.Level);
                default: return 0;
            }
        }

        public double RageDamageMod(ResultType type, bool MH = true)
        {
            switch (type)
            {
                case ResultType.Crit: return 2;
                case ResultType.Glance: return GlancingDamage(Player.WeaponSkill[MH ? Player.MH.Type : Player.OH.Type], Boss.Level);
                case ResultType.Miss: return 0;
                default: return 1;
            }
        }

        public static double ArmorMitigation(int armor, int attackerLevel = 60, double armorpen = 0)
        {
            double res = 0;
            armor -= (int)Math.Round(armorpen);
            //armor = Math.Max(0, armor);

            if(Program.version == Version.Vanilla || attackerLevel < 60)
            {
                res = armor / (armor + 400 + 85.0 * attackerLevel);
            }
            else if(Program.version == Version.TBC)
            {
                res = armor / (armor + 400 + 85 * (attackerLevel + (5.5 * attackerLevel - 265.5)));
            }
            return 1 - (res > 0.75 ? 0.75 : res);
        }

        public static double MagicMitigation(Dictionary<double, double> dic)
        {
            double r = Randomer.NextDouble();
            double tot = 0;
            foreach (double d in dic.Keys)
            {
                tot += dic[d] / 100;
                if (r <= tot)
                {
                    return 1 - d;
                }
            }
            return 1;
        }

        public static ResultType MagicMitigationBinary(int resistance)
        {
            return Randomer.NextDouble() < AverageResistChance(resistance) ? ResultType.Resist : ResultType.Hit;
        }

        public static double AverageResistChance(int resistance, int attackerLevel = 60)
        {
            return Math.Min(0.75, resistance / (attackerLevel * 5.0) * 0.75);
        }

        public static Dictionary<double, double> ResistChances(int resistance)
        {
            if(resistance == 0)
            {
                return new Dictionary<double, double>()
                {
                    { 1, 0 },
                    { 0.75, 0 },
                    { 0.5, 0 },
                    { 0.25, 0 },
                };
            }
            else
            {
                Dictionary<double, double> res = new Dictionary<double, double>()
                {
                    { 1, Math.Min(100, Math.Max(0, 0.3666444 - 0.07646683*resistance + 0.002496647*Math.Pow(resistance,2) - 0.00002685504*Math.Pow(resistance,3) + 1.15313e-7*Math.Pow(resistance,4) - 1.589142e-10*Math.Pow(resistance,5))) },
                    { 0.75, Math.Min(100, Math.Max(0, -0.2315206 + 0.07957951*resistance - 0.001653912*Math.Pow(resistance,2) + 0.00001797503*Math.Pow(resistance,3) - 5.719072e-8*Math.Pow(resistance,4) + 6.51176e-11*Math.Pow(resistance,5))) },
                    { 0.5, Math.Min(100, Math.Max(0, -0.9810354 + 0.4243505*resistance - 0.008868414*Math.Pow(resistance,2) + 0.0001051921*Math.Pow(resistance,3) - 4.493317e-7*Math.Pow(resistance,4) + 6.123606e-10*Math.Pow(resistance,5))) },
                    { 0.25, Math.Min(100, Math.Max(0, 1.472044 + 0.2787054*resistance + 0.01131226*Math.Pow(resistance,2) - 0.0001453473*Math.Pow(resistance,3) + 5.678727e-7*Math.Pow(resistance,4) - 7.311312e-10*Math.Pow(resistance,5))) },
                };

                return res;
            }
        }

        public double GlancingDamage(int skill = 300, int enemyLevel = 63)
        {
            if (Program.version == Version.TBC) skill = 350;
            double low = Math.Max(0.01, Math.Min(0.91, 1.3 - 0.05 * (enemyLevel * 5 - skill)));
            double high = Math.Max(0.2, Math.Min(0.99, 1.2 - 0.03 * (enemyLevel * 5 - skill)));
            return Randomer.NextDouble() * (high - low) + low;
        }

        public static double RageGained(double damage, int level = 60, bool mh = true, bool crit = false, double speed = 1.0)
        {
            switch(Program.version)
            {
                case Version.Vanilla: return Math.Max(1, 7.5 * damage / RageConversionValue(level));
                case Version.TBC: return Math.Min(15 * damage / RageConversionValue(level), 15 * damage / (4 * RageConversionValue(level)) + (RageWhiteHitFactor(mh, crit) * speed / 2));
                default: return Math.Max(1, 7.5 * damage / RageConversionValue(level));
            }
        }

        public static double RageConversionValue(int level = 60)
        {
            return 0.0091107836 * Math.Pow(level,2) + 3.225598133 * level + 4.2652911;
        }

        public static double RageWhiteHitFactor(bool mh, bool crit)
        {
            return 1.25 * (mh ? 2 : 1) * (crit ? 2 : 1);
        }
    }
}

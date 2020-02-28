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

        public double CurrentTime { get; set; }

        public bool AutoLife { get; set; }
        public double LowLifeTime { get; set; }

        public bool Ended { get; set; }

        public Simulation(Player player, Boss boss, double fightLength, bool autoBossLife = true, double lowLifeTime = 0, double fightLengthMod = 0.2)
        {
            Player = player;
            Boss = boss;
            player.Sim = this;
            Boss.Sim = this;
            FightLength = fightLength * (1 + fightLengthMod/2 - (Randomer.NextDouble() * fightLengthMod));
            Results = new SimResult(FightLength);
            Damage = 0;
            CurrentTime = 0;
            AutoLife = autoBossLife;
            LowLifeTime = lowLifeTime;

            lastHit = -hitEvery;

            Ended = false;
        }

        public void StartSim()
        {
            Player.PrepFight();

            CurrentTime = 0;
            Boss.LifePct = 1;

            while (CurrentTime < FightLength)
            {
                SimTick();

                CurrentTime += 1 / RATE;
            }

            Program.AddSimDps(Damage / FightLength);
            Program.AddSimResult(Results);

            Ended = true;
        }

        public static bool tank = true;
        static double hitEvery = 1;
        static double hitRage = 20;
        double lastHit;

        public void SimTick()
        {
            if (AutoLife)
            {
                Boss.LifePct = Math.Max(0, 1 - (CurrentTime / FightLength) * (16.0 / 17.0));
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

            if(tank && lastHit + hitEvery <= CurrentTime)
            {
                Player.Resource += (int)hitRage;

                lastHit += hitEvery;
            }

            if (Player.casting != null && Player.casting.CastFinish <= CurrentTime)
            {
                Player.casting.DoAction();
            }

            if (Player.Class == Player.Classes.Rogue || Player.Class == Player.Classes.Druid)
            {
                Player.CheckEnergyTick();
            }
            if (Player.MaxMana > 0)
            {
                Player.CheckManaTick();
            }

            Player.Rota();
        }

        public void RegisterAction(RegisteredAction action)
        {
            Results.Actions.Add(action);
            Damage += action.Result.Damage;
        }

        public void RegisterEffect(RegisteredEffect effect)
        {
            Results.Effects.Add(effect);
            Damage += effect.Damage;
        }

        public static double Normalization(Weapon w)
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

        public double DamageMod(ResultType type, School school = School.Physical, int level = 60, int enemyLevel = 63)
        {
            switch (type)
            {
                // TODO BLOCK / BLOCKCRIT
                case ResultType.Crit: return school == School.Physical ? 2 : 1.5;
                case ResultType.Hit: return 1;
                case ResultType.Glance: return GlancingDamage(level, enemyLevel);
                default: return 0;
            }
        }

        public double RageDamageMod(ResultType type, int level = 60, int enemyLevel = 63)
        {
            switch (type)
            {
                case ResultType.Crit: return 2;
                case ResultType.Glance: return GlancingDamage(level, enemyLevel);
                case ResultType.Miss: return 0;
                default: return 1;
            }
        }

        public static double ArmorMitigation(int armor, int attackerLevel = 60)
        {
            double res = armor / (armor + 400 + 85.0 * attackerLevel);
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
            return Randomer.NextDouble() < AverageResistChance(resistance) ? ResultType.Hit : ResultType.Resist;
        }

        public static double AverageResistChance(int resistance, int attackerLevel = 60)
        {
            return 1 - Math.Min(0.75, resistance / (attackerLevel * 5) * 0.75);
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

        public double GlancingDamage(int level = 60, int enemyLevel = 63)
        {
            double low = Math.Max(0.01, Math.Min(0.91, 1.3 - 0.05 * (enemyLevel - level)));
            double high = Math.Max(0.2, Math.Min(0.99, 1.2 - 0.03 * (enemyLevel - level)));
            return Randomer.NextDouble() * (high - low) + low;
        }

        public static double RageGained(int damage, int level = 60)
        {
            return Math.Max(1, 7.5 * damage / RageConversionValue(level));
        }

        /*
        public static double RageGained2(int damage, double speed, bool mh, bool crit, int level = 60)
        {
            return Math.Max(15 * damage / RageConversionValue(level), 15 * damage / (4 * RageConversionValue(level)) + (RageWhiteHitFactor(mh, crit) * speed / 2));
        }
        */

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

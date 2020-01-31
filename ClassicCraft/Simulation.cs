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

            foreach (Effect e in Boss.Effects)
            {
                e.CheckEffect();
            }

            Boss.Effects.RemoveAll(e => e.Ended);

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

        public double DamageMod(ResultType type, int level = 60, int enemyLevel = 63)
        {
            switch (type)
            {
                // TODO BLOCK / BLOCKCRIT
                case ResultType.Crit: return 2;
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class RegisteredAction
    {
        public RegisteredAction(Action action, ActionResult result, double time)
        {
            Action = action;
            Time = time;
            Result = result;
        }

        public Action Action { get; set; }
        public double Time { get; set; }
        public ActionResult Result { get; set; }
    }

    public class RegisteredEffect
    {
        public RegisteredEffect(Effect effect, int damage, double time)
        {
            Effect = effect;
            Damage = damage;
            Time = time;
        }

        public Effect Effect { get; set; }
        public double Time { get; set; }
        public int Damage { get; set; }
    }

    class Program
    {
        public static Random random = new Random();

        public static double fightLength = 300;
        public static int nbSim = 1000;
        public static double targetErrorPct = 0;

        public static ConcurrentBag<List<RegisteredAction>> totalActions = new ConcurrentBag<List<RegisteredAction>>();
        public static ConcurrentBag<List<RegisteredEffect>> totalEffects = new ConcurrentBag<List<RegisteredEffect>>();
        public static ConcurrentBag<double> damages = new ConcurrentBag<double>();

        static void Main(string[] args)
        {

            DateTime start = DateTime.Now;

            Player player = new Player(null);
            Boss boss = new Boss(null);

            // Arms
            player.Talents.Add("IHS", 3);
            player.Talents.Add("DW", 3);
            player.Talents.Add("2HS", 3);
            player.Talents.Add("Impale", 3);

            // Fury
            player.Talents.Add("Cruelty", 5);
            player.Talents.Add("UW", 5);
            player.Talents.Add("IBS", 5);
            player.Talents.Add("DWS", 5);
            player.Talents.Add("IE", 2);
            player.Talents.Add("Flurry", 5);

            player.MH = new Weapon(229, 334, 3.5, true, Weapon.WeaponType.Sword);
            //Player.MH = new Weapon(67, 125, 2.4, false, Weapon.WeaponType.Sword);
            //Player.OH = new Weapon(67, 125, 2.4, false, Weapon.WeaponType.Sword);

            player.CritRating = 0.208;
            player.AP = 1105;
            player.HitRating = 0.05;

            List<Task> tasks = new List<Task>();

            if(targetErrorPct == 0)
            {
                for (int i = 0; i < nbSim; i++)
                {
                    tasks.Add(Task.Factory.StartNew(() => DoSim(player, boss, fightLength)));
                }

                while (!tasks.All(t => t.IsCompleted))
                {
                    Console.Clear();
                    Console.WriteLine("{0:N2}% ({1}/{2})", (double)damages.Count / nbSim * 100, damages.Count, nbSim);
                    if (damages.Count > 0)
                    {
                        Console.WriteLine("Error Percent : {0:N2}%", Stats.ErrorPct(damages.ToArray(), damages.Average()));
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
                Console.Clear();
                Console.WriteLine("{0:N2}% ({1}/{2})", (double)damages.Count / nbSim * 100, damages.Count, nbSim);
            }
            else
            {
                double errorPct;

                do
                {
                    tasks.Add(Task.Factory.StartNew(() => DoSim(player, boss, fightLength)));
                    errorPct = damages.Count > 0 ? Stats.ErrorPct(damages.ToArray(), damages.Average()) : 100;
                    //Console.WriteLine("Error Percent : {0:N2}%", Stats.ErrorPct(damages.ToArray(), damages.Average()));
                }
                while (errorPct > targetErrorPct);
                
                Task.WaitAll(tasks.ToArray());

                nbSim = tasks.Count;
            }


            double time = (DateTime.Now - start).TotalMilliseconds;
            
            Console.WriteLine("{0} simulations done in {1:N2} ms, for {2:N2} ms by sim", nbSim, time, time/nbSim);
            if (nbSim >= 1)
            {
                double avgTotalDmg = damages.Average();
                double[] dps = damages.Select(d => d / fightLength).ToArray();

                double totalAA = totalActions.Select(a => a.Where(t => t.Action is AutoAttack).Count()).Sum();
                double totalBT = totalActions.Select(a => a.Where(t => t.Action is Bloodthirst).Count()).Sum();
                double totalWW = totalActions.Select(a => a.Where(t => t.Action is Whirlwind).Count()).Sum();
                double totalHS = totalActions.Select(a => a.Where(t => t.Action is HeroicStrike).Count()).Sum();
                double totalDW = totalEffects.Select(a => a.Where(t => t.Effect is DeepWounds).Count()).Sum();
                double totalExec = totalActions.Select(a => a.Where(t => t.Action is Execute).Count()).Sum();

                Console.WriteLine("Error Percent : {0:N2}%", Stats.ErrorPct(damages.ToArray(), damages.Average()));
                Console.WriteLine("Average Damage : {0:N2} (+/- {1:N2})", avgTotalDmg, Stats.MeanStdDev(damages.ToArray()));
                Console.WriteLine("Average DPS : {0:N2} dps (+/- {1:N2})", avgTotalDmg / fightLength, Stats.MeanStdDev(dps));
                
                if (totalAA > 0)
                {
                    double avgDpsAA = totalActions.Average(a => a.Where(t => t.Action is AutoAttack).Sum(r => r.Result.Damage) / fightLength);
                    double avgUseAA = totalActions.Average(a => a.Count(t => t.Action is AutoAttack));
                    double avgDmgAA = totalActions.Sum((a => a.Where(t => t.Action is AutoAttack).Sum(r => r.Result.Damage))) / totalAA;
                    Console.WriteLine("Average DPS [Auto Attack] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsAA, avgDmgAA, avgUseAA, fightLength / avgUseAA);
                }
                if (totalBT > 0)
                {
                    double avgDpsBT = totalActions.Average(a => a.Where(t => t.Action is Bloodthirst).Sum(r => r.Result.Damage) / fightLength);
                    double avgUseBT = totalActions.Average(a => a.Count(t => t.Action is Bloodthirst));
                    double avgDmgBT = totalActions.Sum(a => a.Where(t => t.Action is Bloodthirst).Sum(r => r.Result.Damage)) / totalBT;
                    Console.WriteLine("Average DPS [Bloodthirst] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsBT, avgDmgBT, avgUseBT, fightLength / avgUseBT);
                }
                if (totalWW > 0)
                {
                    double avgDpsWW = totalActions.Average(a => a.Where(t => t.Action is Whirlwind).Sum(r => r.Result.Damage) / fightLength);
                    double avgUseWW = totalActions.Average(a => a.Count(t => t.Action is Whirlwind));
                    double avgDmgWW = totalActions.Sum(a => a.Where(t => t.Action is Whirlwind).Sum(r => r.Result.Damage)) / totalWW;
                    Console.WriteLine("Average DPS [Whirlwind] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsWW, avgDmgWW, avgUseWW, fightLength / avgUseWW);
                }
                if (totalHS > 0)
                {
                    double avgDpsHS = totalActions.Average(a => a.Where(t => t.Action is HeroicStrike).Sum(r => r.Result.Damage) / fightLength);
                    double avgUseHS = totalActions.Average(a => a.Count(t => t.Action is HeroicStrike));
                    double avgDmgHS = totalActions.Sum(a => a.Where(t => t.Action is HeroicStrike).Sum(r => r.Result.Damage)) / totalHS;
                    Console.WriteLine("Average DPS [Heroic Strike] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsHS, avgDmgHS, avgUseHS, fightLength / avgUseHS);
                }
                if (totalDW > 0)
                {
                    double avgDpsDW = totalEffects.Average(a => a.Where(t => t.Effect is DeepWounds).Sum(r => r.Damage) / fightLength);
                    double avgUseDW = totalEffects.Average(a => a.Count(t => t.Effect is DeepWounds));
                    double avgDmgDW = totalEffects.Sum(a => a.Where(t => t.Effect is DeepWounds).Sum(r => r.Damage)) / totalDW;
                    Console.WriteLine("Average DPS [Deep Wounds] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsDW, avgDmgDW, avgUseDW, fightLength / avgUseDW);
                }
                if (totalExec > 0)
                {
                    double avgDpsExec = totalActions.Average(a => a.Where(t => t.Action is Execute).Sum(r => r.Result.Damage) / fightLength);
                    double avgUseExec = totalActions.Average(a => a.Count(t => t.Action is Execute));
                    double avgDmgExec = totalActions.Sum(a => a.Where(t => t.Action is Execute).Sum(r => r.Result.Damage)) / totalExec;
                    Console.WriteLine("Average DPS [Execute] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsExec, avgDmgExec, avgUseExec, fightLength / avgUseExec);
                }
            }

            Console.ReadKey();
        }

        public static void DoSim(Player p, Boss b, double length)
        {
            Simulation s = new Simulation(p, b, length);
            s.StartSim();
        }

        public static double Normalization(Weapon w)
        {
            if(w.Type == Weapon.WeaponType.Dagger)
            {
                return 1.7;
            }
            else if(w.TwoHanded)
            {
                return 3.3;
            }
            else
            {
                return 2.4;
            }
        }

        public static double DamageMod(ResultType type, int level = 60, int enemyLevel = 63)
        {
            switch(type)
            {
                case ResultType.Crit: return 2;
                case ResultType.Hit: return 1;
                case ResultType.Glancing: return GlancingDamage(level, enemyLevel);
                default: return 0;
            }
        }

        public static double GlancingDamage(int level = 60, int enemyLevel = 63)
        {
            double low = Math.Max(0.01, Math.Min(0.91, 1.3 - 0.05 * (enemyLevel - level)));
            double high = Math.Max(0.2, Math.Min(0.99, 1.2 - 0.03 * (enemyLevel - level)));
            return random.NextDouble() * (high-low) + low;
        }

        public static double RageGained(int damage, double weaponSpeed, ResultType type, bool mh = true)
        {
            return (15 * damage) / (4 * RageConversionValue()) + (RageWhiteHitFactor(mh, type == ResultType.Crit) * weaponSpeed) / 2;
        }

        public static double RageGained2(int damage)
        {
            return (15 * damage) / RageConversionValue();
        }

        public static double RageConversionValue(int level = 60)
        {
            return 0.0091107836*level*level + 3.225598133*level + 4.2652911;
        }

        public static double RageWhiteHitFactor(bool mh, bool crit)
        {
            if(mh)
            {
                if(crit)
                {
                    return 7.0;
                }
                else
                {
                    return 3.5;
                }
            }
            else
            {
                if(crit)
                {
                    return 3.5;
                }
                else
                {
                    return 1.75;
                }
            }
        }
    }
}

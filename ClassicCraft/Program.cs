using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public static Boss Boss { get; set; }
        public static Player Player { get; set; }

        public static Random random = new Random();

        public static double RATE = 100;

        public static double currentTime = 0;

        private static List<RegisteredAction> actions;
        private static List<RegisteredEffect> effects;
        private static double damage;

        public static double fightLength = 300;
        public static int nbSim = 1;

        static void Main(string[] args)
        {
            Player = new Player();
            Boss = new Boss();

            Boss.MaxLife = 10000;
            Boss.Life = Boss.MaxLife;

            // Arms
            Player.Talents.Add("IHS", 3);
            Player.Talents.Add("DW", 3);
            Player.Talents.Add("2HS", 3);
            Player.Talents.Add("Impale", 3);

            // Fury
            Player.Talents.Add("Cruelty", 5);
            Player.Talents.Add("UW", 5);
            Player.Talents.Add("IBS", 5);
            Player.Talents.Add("DWS", 5);
            Player.Talents.Add("IE", 2);
            Player.Talents.Add("Flurry", 5);

            List<List<RegisteredAction>> totalActions = new List<List<RegisteredAction>>();
            List<List<RegisteredEffect>> totalEffects = new List<List<RegisteredEffect>>();
            List<double> damages = new List<double>();

            DateTime start = DateTime.Now;

            for (int i = 0; i < nbSim; i++)
            {
                actions = new List<RegisteredAction>();
                effects = new List<RegisteredEffect>();
                damage = 0;

                Player.Reset();

                List<AutoAttack> autos = new List<AutoAttack>();
                
                Player.MH = new Weapon(229, 334, 3.5, true, Weapon.WeaponType.Sword);
                //Player.MH = new Weapon(67, 125, 2.4, false, Weapon.WeaponType.Sword);
                //Player.OH = new Weapon(67, 125, 2.4, false, Weapon.WeaponType.Sword);
                
                Player.CritRating = 0.208;
                Player.AP = 1105;
                Player.HitRating = 0.05;

                autos.Add(new AutoAttack(Player.MH, true));
                if (Player.OH != null)
                {
                    autos.Add(new AutoAttack(Player.OH, false));
                }

                currentTime = 0;

                Whirlwind ww = new Whirlwind();
                Bloodthirst bt = new Bloodthirst();
                HeroicStrike hs = new HeroicStrike();
                hs.RessourceCost -= Player.GetTalentPoints("IHS");
                Recklessness r = new Recklessness();
                DeathWish dw = new DeathWish();
                Execute exec = new Execute();

                Boss.LifePct = 1;

                // Charge
                Player.Ressource += 15;
                Player.StartGCD();

                int rota = 1;

                while (currentTime < fightLength)
                {
                    Boss.LifePct = Math.Max(0, 1 - (currentTime / fightLength) * (16.0/17.0));

                    foreach (Effect e in Player.Effects)
                    {
                        e.CheckEffect();
                    }
                    foreach (Effect e in Boss.Effects)
                    {
                        e.CheckEffect();
                    }

                    if((Boss.LifePct > 0.5 || Boss.LifePct <= 0.2) && dw.CanUse())
                    {
                        dw.Cast();
                    }
                    if((Boss.LifePct > 0.5 || Boss.LifePct <= 0.2) && r.CanUse() && Player.Effects.Any(e => e is DeathWishBuff))
                    {
                        r.Cast();
                    }

                    if(rota == 0)
                    {

                    }
                    else if(rota == 1)
                    {
                        if(Boss.LifePct > 0.2)
                        {
                            if (ww.CanUse())
                            {
                                ww.Cast();
                            }
                            else if (bt.CanUse() && Player.Ressource >= ww.RessourceCost + bt.RessourceCost)
                            {
                                bt.Cast();
                            }

                            if (Player.Ressource >= 75)
                            {
                                hs.Cast();
                            }
                        }
                        else
                        {
                            if (exec.CanUse())
                            {
                                exec.Cast();
                            }
                        }
                    }
                    else if(rota == 2)
                    {
                        if (bt.CanUse())
                        {
                            bt.Cast();
                        }
                        else if (ww.CanUse() && Player.Ressource >= ww.RessourceCost + bt.RessourceCost)
                        {
                            ww.Cast();
                        }

                        if (Player.Ressource >= 75)
                        {
                            hs.Cast();
                        }
                    }

                    foreach (AutoAttack a in autos)
                    {
                        if (a.Available())
                        {
                            if(a.MH && Player.applyAtNextAA != null)
                            {
                                Player.applyAtNextAA.DoAction();
                                a.NextAA();
                            }
                            else
                            {
                                a.Cast();
                            }
                        }
                    }

                    Player.Effects.RemoveAll(e => e.Ended);
                    Boss.Effects.RemoveAll(e => e.Ended);

                    currentTime += 1 / RATE;
                }

                damages.Add(damage);
                totalActions.Add(actions);
                totalEffects.Add(effects);
            }

            double time = (DateTime.Now - start).TotalMilliseconds;
            Console.WriteLine("Total {0:N2} ms, {1:N2} ms by sim", time, time/nbSim);
            if (nbSim >= 1)
            {
                double avgTotalDmg = damages.Average();

                int totalAA = totalActions.Select(a => a.Where(t => t.Action is AutoAttack).Count()).Sum();
                int totalBT = totalActions.Select(a => a.Where(t => t.Action is Bloodthirst).Count()).Sum();
                int totalWW = totalActions.Select(a => a.Where(t => t.Action is Whirlwind).Count()).Sum();
                int totalHS = totalActions.Select(a => a.Where(t => t.Action is HeroicStrike).Count()).Sum();
                int totalDW = totalEffects.Select(a => a.Where(t => t.Effect is DeepWounds).Count()).Sum();
                int totalExec = totalActions.Select(a => a.Where(t => t.Action is Execute).Count()).Sum();

                Console.WriteLine("Average Damage : {0:N2}", avgTotalDmg);
                Console.WriteLine("Average DPS : {0:N2} dps", avgTotalDmg / fightLength);

                if (totalAA > 0)
                {
                    double avgDpsAA = totalActions.Select(a => a.Where(t => t.Action is AutoAttack).Sum(r => r.Result.Damage) / fightLength).Average();
                    double avgUseAA = totalActions.Select(a => a.Where(t => t.Action is AutoAttack).Count()).Average();
                    double avgDmgAA = totalActions.Select(a => a.Where(t => t.Action is AutoAttack).Sum(r => r.Result.Damage)).Sum() / totalAA;
                    Console.WriteLine("Average DPS [Auto Attack] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsAA, avgDmgAA, avgUseAA, fightLength / avgUseAA);
                }
                if (totalBT > 0)
                {
                    double avgDpsBT = totalActions.Select(a => a.Where(t => t.Action is Bloodthirst).Sum(r => r.Result.Damage) / fightLength).Average();
                    double avgUseBT = totalActions.Select(a => a.Where(t => t.Action is Bloodthirst).Count()).Average();
                    double avgDmgBT = totalActions.Select(a => a.Where(t => t.Action is Bloodthirst).Sum(r => r.Result.Damage)).Sum() / totalBT;
                    Console.WriteLine("Average DPS [Bloodthirst] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsBT, avgDmgBT, avgUseBT, fightLength / avgUseBT);
                }
                if (totalWW > 0)
                {
                    double avgDpsWW = totalActions.Select(a => a.Where(t => t.Action is Whirlwind).Sum(r => r.Result.Damage) / fightLength).Average();
                    double avgUseWW = totalActions.Select(a => a.Where(t => t.Action is Whirlwind).Count()).Average();
                    double avgDmgWW = totalActions.Select(a => a.Where(t => t.Action is Whirlwind).Sum(r => r.Result.Damage)).Sum() / totalWW;
                    Console.WriteLine("Average DPS [Whirlwind] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsWW, avgDmgWW, avgUseWW, fightLength / avgUseWW);
                }
                if (totalHS > 0)
                {
                    double avgDpsHS = totalActions.Select(a => a.Where(t => t.Action is HeroicStrike).Sum(r => r.Result.Damage) / fightLength).Average();
                    double avgUseHS = totalActions.Select(a => a.Where(t => t.Action is HeroicStrike).Count()).Average();
                    double avgDmgHS = totalActions.Select(a => a.Where(t => t.Action is HeroicStrike).Sum(r => r.Result.Damage)).Sum() / totalHS;
                    Console.WriteLine("Average DPS [Heroic Strike] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsHS, avgDmgHS, avgUseHS, fightLength / avgUseHS);
                }
                if (totalDW > 0)
                {
                    double avgDpsDW = totalEffects.Select(a => a.Where(t => t.Effect is DeepWounds).Sum(r => r.Damage) / fightLength).Average();
                    double avgUseDW = totalEffects.Select(a => a.Where(t => t.Effect is DeepWounds).Count()).Average();
                    double avgDmgDW = totalEffects.Select(a => a.Where(t => t.Effect is DeepWounds).Sum(r => r.Damage)).Sum() / totalDW;
                    Console.WriteLine("Average DPS [Deep Wounds] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsDW, avgDmgDW, avgUseDW, fightLength / avgUseDW);
                }
                if (totalExec > 0)
                {
                    double avgDpsExec = totalActions.Select(a => a.Where(t => t.Action is Execute).Sum(r => r.Result.Damage) / fightLength).Average();
                    double avgUseExec = totalActions.Select(a => a.Where(t => t.Action is Execute).Count()).Average();
                    double avgDmgExec = totalActions.Select(a => a.Where(t => t.Action is Execute).Sum(r => r.Result.Damage)).Sum() / totalExec;
                    Console.WriteLine("Average DPS [Execute] : {0:N2} dps average of {1:N2} for {2:N2} uses or 1 use every {3:N2}s", avgDpsExec, avgDmgExec, avgUseExec, fightLength / avgUseExec);
                }

                Console.WriteLine("Error Percent : {0:N2}%", Stats.ErrorPct(damages.ToArray(), damages.Average()));
            }

            Console.ReadKey();
        }

        public static async Task DoFight()
        {

        }

        public static void RegisterAction(RegisteredAction action)
        {
            actions.Add(action);
            damage += action.Result.Damage;
        }

        public static void RegisterEffect(RegisteredEffect effect)
        {
            effects.Add(effect);
            damage += effect.Damage;
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

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
        private static Random random = new Random();

        public static string simJsonFileName = "sim.json";
        public static string itemsJsonFileName = "items.json";
        public static string logsFileName = "logs";
        public static string ext = ".txt";

        public static bool debug = false;
        public static string debugPath = ".\\..\\..";

        public static Player playerBase = null;
        public static Boss bossBase = null;

        public static double fightLength = 300;
        public static double fightLengthMod = 0.2;
        public static bool bossAutoLife = true;
        public static double bossLowLifeTime = 0;

        public static int nbSim = 1000;
        public static double targetErrorPct = 0.5;
        public static bool targetError = true;
        public static bool threading = true;
        public static bool logFight = false;
        public static bool calcDpss = false;

        public static bool simWeights = true;

        //public static List<Attribute> toWeight = null;
        //public static Attribute weighted;

        public static int nbTasksForSims = 1000;

        private static List<SimResult> CurrentResults;
        private static List<double> CurrentDpsList;

        private static Dictionary<string, List<SimResult>> ResultsList = new Dictionary<string, List<SimResult>>();
        private static Dictionary<string, List<double>> DamagesList = new Dictionary<string, List<double>>();
        private static List<double> ErrorList = new List<double>();

        public static string logs = "";
        
        public static List<string> simOrder = new List<string>(){ "Base", "+1% Hit", "+1% Crit", "+50 AP" };
        public static Dictionary<string, Attributes> simBonusAttribs = new Dictionary<string, Attributes>()
                {
                    { "Base", new Attributes() },
                    { "+1% Hit", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.HitChance, 0.01 }
                            })},
                    { "+1% Crit", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.CritChance, 0.01 }
                            })},
                    { "+50 AP", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.AP, 50 }
                            })},
                };

        static void Main(string[] args)
        {
            try
            {
                string simString = File.ReadAllText(debug ? Path.Combine(debugPath, simJsonFileName) : simJsonFileName);
                JsonUtil.JsonSim jsonSim = JsonConvert.DeserializeObject<JsonUtil.JsonSim>(simString);

                fightLength = jsonSim.FightLength;
                fightLengthMod = jsonSim.FightLengthMod;
                bossAutoLife = jsonSim.BossAutoLife;
                bossLowLifeTime = jsonSim.BossLowLifeTime;

                nbSim = jsonSim.NbSim;
                targetErrorPct = jsonSim.TargetErrorPct;
                targetError = jsonSim.TargetError;
                logFight = jsonSim.LogFight;
                //toWeight = JsonUtil.ToAttributes(jsonSim.SimStatWeight);
                
                Log(string.Format("Date : {0}\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                Log(string.Format("Fight length : {0} seconds (+/- {1}%)", fightLength, fightLengthMod * 100));

                if (logFight)
                {
                    targetError = false;

                    if (nbSim > 10)
                    {
                        nbSim = 10;
                    }
                }

                playerBase = JsonUtil.JsonPlayer.ToPlayer(jsonSim.Player);

                // Talents
                playerBase.Talents = new Dictionary<string, int>();


                // WARRIOR
                // Arms
                /*
                playerBase.Talents.Add("IHS", 3);
                playerBase.Talents.Add("DW", 3);
                playerBase.Talents.Add("2HS", 3);
                playerBase.Talents.Add("Impale", 2);
                */
                // Fury
                /*
                playerBase.Talents.Add("Cruelty", 5);
                playerBase.Talents.Add("UW", 5);
                playerBase.Talents.Add("IBS", 5);
                playerBase.Talents.Add("DWS", 3); //playerBase.Talents.Add("DWS", 5);
                playerBase.Talents.Add("IE", 2);
                playerBase.Talents.Add("Flurry", 5);
                */

                // DRUID
                // Balance
                playerBase.Talents.Add("NW", 5);
                playerBase.Talents.Add("NS", 3);
                playerBase.Talents.Add("OC", 1);
                // Feral
                playerBase.Talents.Add("Fero", 5);
                playerBase.Talents.Add("FA", 5);
                playerBase.Talents.Add("SC", 3);
                playerBase.Talents.Add("IS", 2);
                playerBase.Talents.Add("PS", 3);
                playerBase.Talents.Add("BF", 2);
                playerBase.Talents.Add("SF", 2);
                playerBase.Talents.Add("HW", 5);
                // Resto
                playerBase.Talents.Add("Furor", 5);

                playerBase.CalculateAttributes();

                Log("\nPlayer :");
                Log(playerBase.ToString());

                bossBase = JsonUtil.JsonBoss.ToBoss(jsonSim.Boss, playerBase.Attributes.GetValue(Attribute.ArmorPen));

                Log("\nBoss :");
                Log(bossBase.ToString());

                if (logFight) threading = false;

                DateTime start = DateTime.Now;

                List<Task> tasks = new List<Task>();

                Enchantment we = new Enchantment(0, "Weights", new Attributes(new Dictionary<Attribute, double>()));
                playerBase.Buffs.Add(we);
                
                for(int done = 0; done < (simWeights ? simOrder.Count : 1); done++)
                {
                    CurrentDpsList = new List<double>();
                    CurrentResults = new List<SimResult>();
                    
                    if (done > 0)
                    {
                        we.Attributes = simBonusAttribs[simOrder[done]];
                        playerBase.CalculateAttributes();
                    }

                    if (!threading && !targetError)
                    {
                        for (int i = 0; i < nbSim; i++)
                        {
                            Log(string.Format("\n\n---SIM NUMBER {0}---\n", i + 1));
                            DoSim();
                        }
                    }
                    else
                    {
                        if (!targetError)
                        {
                            for (int i = 0; i < nbSim; i++)
                            {
                                tasks.Add(Task.Factory.StartNew(() => DoSim()));
                            }

                            while (!tasks.All(t => t.IsCompleted))
                            {
                                if (!logFight)
                                {
                                    Console.Clear();
                                    Console.WriteLine("{0:N2}% ({1}/{2})", (double)CurrentDpsList.Count / nbSim * 100, CurrentDpsList.Count, nbSim);
                                }

                                if (CurrentDpsList.Count > 0)
                                {
                                    Console.WriteLine("Error Percent : {0:N2}%", Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average()));
                                }
                                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                            }

                            if (!logFight)
                            {
                                Console.Clear();
                                Console.WriteLine("{0:N2}% ({1}/{2})", (double)CurrentDpsList.Count / nbSim * 100, CurrentDpsList.Count, nbSim);
                            }
                        }
                        else
                        {
                            nbSim = 0;

                            double errorPct = 100;

                            while (errorPct > targetErrorPct)
                            {
                                while (tasks.Count(t => !t.IsCompleted) < nbTasksForSims)
                                {
                                    nbSim++;
                                    tasks.Add(Task.Factory.StartNew(() => DoSim()));
                                }

                                foreach (Task t in tasks.Where(t => t.IsCompleted))
                                {
                                    t.Dispose();
                                }
                                tasks.RemoveAll(t => t.IsCompleted);

                                lock(CurrentDpsList)
                                {
                                    if (CurrentDpsList.Count > 0)
                                    {
                                        errorPct = Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average());
                                    }
                                }

                                Console.Clear();
                                Console.WriteLine("Sims done : {0:N2}", CurrentDpsList.Count);
                                Console.WriteLine("Sims running : {0:N2}", tasks.Count(t => !t.IsCompleted));
                                Console.WriteLine("Error Percent : {0:N2}%", errorPct);

                                Thread.Sleep(TimeSpan.FromSeconds(0.5));
                            }

                            Console.WriteLine("Waiting for remaining simulations to complete...");

                            Task.WaitAll(tasks.ToArray());
                        }
                    }

                    ErrorList.Add(Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average()));
                    ResultsList.Add(simOrder[done], CurrentResults);
                    DamagesList.Add(simOrder[done], CurrentDpsList);
                }

                if (!logFight)
                {
                    Console.Clear();
                }

                double time = (DateTime.Now - start).TotalMilliseconds;

                /*
                if (calcDpss)
                {
                    Dictionary<double, double> dpss = new Dictionary<double, double>();

                    int dpsOver = 10;

                    for (int i = 0; i < fightLength; i++)
                    {
                        double dmg = totalActions.Average(l => l.Where(a => a.Time >= i - dpsOver && a.Time < i).Sum(ac => (double)ac.Result.Damage / dpsOver))
                            + totalEffects.Average(l => l.Where(a => a.Time >= i - dpsOver && a.Time < i).Sum(ac => (double)ac.Damage / dpsOver));

                        dpss.Add(i, dmg);
                    }
                }
                */

                int simsDone = 0;
                foreach(string k in DamagesList.Keys)
                {
                    simsDone += DamagesList[k].Count;
                }

                string endMsg1 = string.Format("{0} simulations done in {1:N2} ms, for {2:N2} ms by sim", nbSim, time, time / nbSim);
                Console.WriteLine(endMsg1);
                Log(endMsg1);

                string endMsg2 = string.Format("Overall accuracy of results : +/- {0:N2}%", ErrorList.Average());
                Console.WriteLine(endMsg2);
                Log(endMsg2);

                if (simWeights)
                {
                    double baseDps = DamagesList["Base"].Average();
                    double apDps = DamagesList["+50 AP"].Average();
                    double hitDps = DamagesList["+1% Hit"].Average();
                    double critDps = DamagesList["+1% Crit"].Average();

                    Log("DPS :\n");
                    Log(string.Format("Base : {0:N2} DPS", baseDps));
                    Log(string.Format("+50 AP : {0:N2} DPS", apDps));
                    Log(string.Format("+1% Hit : {0:N2} DPS", hitDps));
                    Log(string.Format("+1% Crit : {0:N2} DPS", critDps));

                    double apDif = (apDps - baseDps) / 50;
                    double hitDif = hitDps - baseDps;
                    double critDif = critDps - baseDps;

                    if (apDif < 0) apDif = 0;
                    if (hitDif < 0) hitDif = 0;
                    if (critDif < 0) critDif = 0;

                    double agiDpsCalc = Player.AgiToAPRatio(playerBase.Class) * apDif + Player.AgiToCritRatio(playerBase.Class) * 100 * critDif;
                    double strDpsCalc = apDif * Player.StrToAPRatio(playerBase.Class) * (playerBase.Class == Player.Classes.Druid ? (1 + 0.04 * playerBase.GetTalentPoints("HW")) : 1);

                    Log("\nWeights by DPS :");
                    Log(string.Format("1 AP = {0:N4} DPS", apDif));
                    Log(string.Format("1% Hit = {0:N4} DPS", hitDif));
                    Log(string.Format("1% Crit = {0:N4} DPS", critDif));
                    Log(string.Format("1 Agi = {0:N4} DPS", agiDpsCalc));
                    Log(string.Format("1 Str = {0:N4} DPS", strDpsCalc));

                    Log("\nWeights ratio for 1 AP :\n");
                    Log(string.Format("1% Hit = {0:N4} AP", hitDif/apDif));
                    Log(string.Format("1% Crit = {0:N4} AP", critDif/apDif));
                    Log(string.Format("1 Agi = {0:N4} AP", agiDpsCalc/apDif));
                    Log(string.Format("1 Str = {0:N4} AP", strDpsCalc/apDif));
                }
                else if (nbSim >= 1)
                {
                    double avgDps = CurrentDpsList.Average();

                    List<List<RegisteredAction>> totalActions = ResultsList["Base"].Select(r => r.Actions).ToList();
                    List<List<RegisteredEffect>> totalEffects = ResultsList["Base"].Select(r => r.Effects).ToList();

                    Log(string.Format("Nb simulations : {0}", CurrentDpsList.Count));
                    Log(string.Format("Error Percent : {0:N2}%", Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average())));
                    Log(string.Format("Average DPS : {0:N2} dps (+/- {1:N2})", avgDps, Stats.MeanStdDev(CurrentDpsList.ToArray())));

                    double totalAA = totalActions.Select(a => a.Where(t => t.Action is AutoAttack).Count()).Sum();
                    if (totalAA > 0)
                    {
                        double avgDpsAA = totalActions.Average(a => a.Where(t => t.Action is AutoAttack).Sum(r => r.Result.Damage / fightLength));
                        double avgUseAA = totalActions.Average(a => a.Count(t => t.Action is AutoAttack));
                        double avgDmgAA = totalActions.Sum(a => a.Where(t => t.Action is AutoAttack).Sum(r => r.Result.Damage / totalAA));
                        Log(string.Format("Average DPS [Auto Attack] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgDpsAA, avgDmgAA, avgUseAA, fightLength / avgUseAA, avgDpsAA / avgDps * 100));
                    }

                    if (playerBase.Class == Player.Classes.Druid)
                    {
                        double totalSH = totalActions.Select(a => a.Where(t => t.Action is Shred).Count()).Sum();
                        double totalFB = totalActions.Select(a => a.Where(t => t.Action is FerociousBite).Count()).Sum();

                        if (totalSH > 0)
                        {
                            double avgSpellDps = totalActions.Average(a => a.Where(t => t.Action is Shred).Sum(r => r.Result.Damage / fightLength));
                            double avgUse = totalActions.Average(a => a.Count(t => t.Action is Shred));
                            double avgDmg = totalActions.Sum(a => a.Where(t => t.Action is Shred).Sum(r => r.Result.Damage / totalSH));
                            Log(string.Format("Average DPS [Shred] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgSpellDps, avgDmg, avgUse, fightLength / avgUse, avgSpellDps / avgDps * 100));
                        }
                        if (totalFB > 0)
                        {
                            double avgSpellDps = totalActions.Average(a => a.Where(t => t.Action is FerociousBite).Sum(r => r.Result.Damage / fightLength));
                            double avgUse = totalActions.Average(a => a.Count(t => t.Action is FerociousBite));
                            double avgDmg = totalActions.Sum(a => a.Where(t => t.Action is FerociousBite).Sum(r => r.Result.Damage / totalFB));
                            Log(string.Format("Average DPS [Ferocious Bite] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgSpellDps, avgDmg, avgUse, fightLength / avgUse, avgSpellDps / avgDps * 100));
                        }
                    }
                    else if(playerBase.Class == Player.Classes.Warrior)
                    {
                        double totalBT = totalActions.Select(a => a.Where(t => t.Action is Bloodthirst).Count()).Sum();
                        double totalWW = totalActions.Select(a => a.Where(t => t.Action is Whirlwind).Count()).Sum();
                        double totalHS = totalActions.Select(a => a.Where(t => t.Action is HeroicStrike).Count()).Sum();
                        double totalDW = totalEffects.Select(a => a.Where(t => t.Effect is DeepWounds).Count()).Sum();
                        double totalExec = totalActions.Select(a => a.Where(t => t.Action is Execute).Count()).Sum();
                        double totalHam = totalActions.Select(a => a.Where(t => t.Action is Hamstring).Count()).Sum();

                        if (totalBT > 0)
                        {
                            double avgDpsBT = totalActions.Average(a => a.Where(t => t.Action is Bloodthirst).Sum(r => r.Result.Damage / fightLength));
                            double avgUseBT = totalActions.Average(a => a.Count(t => t.Action is Bloodthirst));
                            double avgDmgBT = totalActions.Sum(a => a.Where(t => t.Action is Bloodthirst).Sum(r => r.Result.Damage / totalBT));
                            Log(string.Format("Average DPS [Bloodthirst] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgDpsBT, avgDmgBT, avgUseBT, fightLength / avgUseBT, avgDpsBT / avgDps * 100));
                        }
                        if (totalWW > 0)
                        {
                            double avgDpsWW = totalActions.Average(a => a.Where(t => t.Action is Whirlwind).Sum(r => r.Result.Damage / fightLength));
                            double avgUseWW = totalActions.Average(a => a.Count(t => t.Action is Whirlwind));
                            double avgDmgWW = totalActions.Sum(a => a.Where(t => t.Action is Whirlwind).Sum(r => r.Result.Damage / totalWW));
                            Log(string.Format("Average DPS [Whirlwind] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgDpsWW, avgDmgWW, avgUseWW, fightLength / avgUseWW, avgDpsWW / avgDps * 100));
                        }
                        if (totalHS > 0)
                        {
                            double avgDpsHS = totalActions.Average(a => a.Where(t => t.Action is HeroicStrike).Sum(r => r.Result.Damage / fightLength));
                            double avgUseHS = totalActions.Average(a => a.Count(t => t.Action is HeroicStrike));
                            double avgDmgHS = totalActions.Sum(a => a.Where(t => t.Action is HeroicStrike).Sum(r => r.Result.Damage / totalHS));
                            Log(string.Format("Average DPS [Heroic Strike] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgDpsHS, avgDmgHS, avgUseHS, fightLength / avgUseHS, avgDpsHS / avgDps * 100));
                        }
                        if (totalHam > 0)
                        {
                            double avgDpsHam = totalActions.Average(a => a.Where(t => t.Action is Hamstring).Sum(r => r.Result.Damage / fightLength));
                            double avgUseHam = totalActions.Average(a => a.Count(t => t.Action is Hamstring));
                            double avgDmgHam = totalActions.Sum(a => a.Where(t => t.Action is Hamstring).Sum(r => r.Result.Damage / totalExec));
                            Log(string.Format("Average DPS [Hamstring] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgDpsHam, avgDmgHam, avgUseHam, fightLength / avgUseHam, avgDpsHam / avgDps * 100));
                        }
                        if (totalExec > 0)
                        {
                            double avgDpsExec = totalActions.Average(a => a.Where(t => t.Action is Execute).Sum(r => r.Result.Damage / fightLength));
                            double avgUseExec = totalActions.Average(a => a.Count(t => t.Action is Execute));
                            double avgDmgExec = totalActions.Sum(a => a.Where(t => t.Action is Execute).Sum(r => r.Result.Damage / totalExec));
                            Log(string.Format("Average DPS [Execute] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} uses or 1 use every {3:N2}s", avgDpsExec, avgDmgExec, avgUseExec, fightLength / avgUseExec, avgDpsExec / avgDps * 100));
                        }
                        if (totalDW > 0)
                        {
                            double avgDpsDW = totalEffects.Average(a => a.Where(t => t.Effect is DeepWounds).Sum(r => r.Damage / fightLength));
                            double avgUseDW = totalEffects.Average(a => a.Count(t => t.Effect is DeepWounds));
                            double avgDmgDW = totalEffects.Sum(a => a.Where(t => t.Effect is DeepWounds).Sum(r => r.Damage / totalDW));
                            Log(string.Format("Average DPS [Deep Wounds] : {0:N2} dps ({4:N2}%), average of {1:N2} damage for {2:N2} ticks or 1 tick every {3:N2}s", avgDpsDW, avgDmgDW, avgUseDW, fightLength / avgUseDW, avgDpsDW / avgDps * 100));
                        }
                    }
                }

                string path = debug ? Path.Combine(debugPath, logsFileName + ext) : logsFileName + DateTime.Now.ToString("_yyyyMMdd-HHmmssfff") + ext;
                File.WriteAllText(path, logs);
                Console.WriteLine("Logs written in " + path);
            }
            catch(Exception e)
            {
                Console.WriteLine("Simulation failed with the following error :\n" + e);
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        public static void AddSimResult(SimResult result)
        {
            lock (CurrentResults)
            {
                CurrentResults.Add(result);
            }
        }

        public static void AddSimDps(double damage)
        {
            lock (CurrentDpsList)
            {
                CurrentDpsList.Add(damage);
            }
        }

        public static void Log(string log)
        {
            logs += log + "\n";
        }

        public static void Debug(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        public static void DoSim()
        {
            Player player = new Player(playerBase, playerBase.Equipment, playerBase.Talents);

            foreach (Item i in player.Equipment.Values.Where(i => i != null))
            {
                i.Player = player;
            }

            //player.CalculateAttributes();
            player.Attributes = playerBase.Attributes;
            player.WindfuryTotem = playerBase.WindfuryTotem;
            player.Cooldowns = playerBase.Cooldowns;

            Boss boss = new Boss(bossBase);

            /*
            if(toWeight != null)
            {
                player.Attributes.SetValue(weighted, player.Attributes.GetValue(weighted) + 10);
            }
            */

            Simulation s = new Simulation(player, boss, fightLength, bossAutoLife, bossLowLifeTime);
            s.StartSim();
        }
    }
}

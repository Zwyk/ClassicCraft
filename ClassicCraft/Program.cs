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
        public static string simJsonFileName = "sim.json";
        public static string itemsJsonFileName = "items.json";
        public static string logsFileDir = "Logs";
        public static string logsFileName = "logs";
        public static string txt = ".txt";

        public static bool debug = true;
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

        public static bool statsWeights = false;

        //public static List<Attribute> toWeight = null;
        //public static Attribute weighted;

        public static int nbTasksForSims = 1000;

        private static List<SimResult> CurrentResults;
        private static List<double> CurrentDpsList;

        private static Dictionary<string, List<SimResult>> ResultsList = new Dictionary<string, List<SimResult>>();
        private static Dictionary<string, List<double>> DamagesList = new Dictionary<string, List<double>>();
        private static List<double> ErrorList = new List<double>();

        public static string logs = "";
        
        public static List<string> simOrder = new List<string>(){ "Base", "+1% Hit", "+1% Crit", "+1% Haste", "+50 AP", "+50 Int", "+50 Spi" };
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
                    { "+1% Haste", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.Haste, 0.01 }
                            })},
                    { "+50 AP", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.AP, 50 }
                            })},
                    { "+50 Int", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.Intellect, 50 }
                            })},
                    { "+50 Spi", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.Spirit, 50 }
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
                statsWeights = jsonSim.StatsWeights;
                
                Log(string.Format("Date : {0}\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                Log(string.Format("Fight length : {0} seconds (±{1}%)", fightLength, fightLengthMod * 100));

                if (logFight)
                {
                    threading = false;
                    targetError = false;
                    statsWeights = false;

                    if (nbSim > 10)
                    {
                        nbSim = 10;
                    }
                }

                playerBase = JsonUtil.JsonPlayer.ToPlayer(jsonSim.Player);

                if(playerBase.Mana == 0)
                {
                    simOrder.Remove("+50 Int");
                    simOrder.Remove("+50 Spi");
                }

                // Talents
                playerBase.Talents = new Dictionary<string, int>();

                string ptal = jsonSim.Player.Talents;
                if (playerBase.Class == Player.Classes.Warrior)
                {
                    if (ptal == null || ptal == "")
                    {
                        if(playerBase.MH.TwoHanded)
                        {
                            // DPS Fury 2M 30305001332-05052005025010051
                            ptal = "30305001332-05052005025010051";
                        }
                        else
                        {
                            // DPS Fury 1M 30305001302-05050005525010051
                            ptal = "30305001302-05050005525010051";
                        }
                    }

                    string[] talents = ptal.Split('-');
                    string arms = talents.Length > 0 ? talents[0] : "";
                    string fury = talents.Length > 1 ? talents[1] : "";
                    string prot = talents.Length > 2 ? talents[2] : "";

                    // Arms
                    playerBase.Talents.Add("IHS", arms.Length > 0 ? (int)Char.GetNumericValue(arms[0]) : 0);
                    playerBase.Talents.Add("DW", arms.Length > 8 ? (int)Char.GetNumericValue(arms[8]) : 0);
                    playerBase.Talents.Add("2HS", arms.Length > 9 ? (int)Char.GetNumericValue(arms[9]) : 0);
                    playerBase.Talents.Add("Impale", arms.Length > 10 ? (int)Char.GetNumericValue(arms[10]) : 0);
                    // Fury
                    playerBase.Talents.Add("Cruelty", fury.Length > 1 ? (int)Char.GetNumericValue(fury[1]) : 0);
                    playerBase.Talents.Add("UW", fury.Length > 3 ? (int)Char.GetNumericValue(fury[3]) : 0);
                    playerBase.Talents.Add("IBS", fury.Length > 7 ? (int)Char.GetNumericValue(fury[7]) : 0);
                    playerBase.Talents.Add("DWS", fury.Length > 8 ? (int)Char.GetNumericValue(fury[8]) : 0);
                    playerBase.Talents.Add("IE", fury.Length > 9 ? (int)Char.GetNumericValue(fury[9]) : 0);
                    playerBase.Talents.Add("Flurry", fury.Length > 15 ? (int)Char.GetNumericValue(fury[15]) : 0);
                }
                else if(playerBase.Class == Player.Classes.Druid)
                {
                    if(ptal == null || ptal == "")
                    {
                        // DPS Feral 014005301-5500021323202151-05
                        ptal = "014005301-5500021323202151-05";
                    }
                    
                    string[] talents = ptal.Split('-');
                    string balance = talents.Length > 0 ? talents[0] : "";
                    string feral = talents.Length > 1 ? talents[1] : "";
                    string resto = talents.Length > 2 ? talents[2] : "";

                    // Balance
                    playerBase.Talents.Add("NW", balance.Length > 5 ? (int)Char.GetNumericValue(balance[5]) : 0);
                    playerBase.Talents.Add("NS", balance.Length > 6 ? (int)Char.GetNumericValue(balance[6]) : 0);
                    playerBase.Talents.Add("OC", balance.Length > 8 ? (int)Char.GetNumericValue(balance[8]) : 0);
                    // Feral
                    playerBase.Talents.Add("Fero", feral.Length > 0 ? (int)Char.GetNumericValue(feral[0]) : 0);
                    playerBase.Talents.Add("FA", feral.Length > 1 ? (int)Char.GetNumericValue(feral[1]) : 0);
                    playerBase.Talents.Add("SC", feral.Length > 7 ? (int)Char.GetNumericValue(feral[7]) : 0);
                    playerBase.Talents.Add("IS", feral.Length > 8 ? (int)Char.GetNumericValue(feral[8]) : 0);
                    playerBase.Talents.Add("PS", feral.Length > 9 ? (int)Char.GetNumericValue(feral[9]) : 0);
                    playerBase.Talents.Add("BF", feral.Length > 10 ? (int)Char.GetNumericValue(feral[10]) : 0);
                    playerBase.Talents.Add("SF", feral.Length > 12 ? (int)Char.GetNumericValue(feral[12]) : 0);
                    playerBase.Talents.Add("HW", feral.Length > 14 ? (int)Char.GetNumericValue(feral[14]) : 0);
                    // Resto
                    playerBase.Talents.Add("Furor", resto.Length > 1 ? (int)Char.GetNumericValue(resto[1]) : 0);
                }
                else if(playerBase.Class == Player.Classes.Rogue)
                {
                    if (ptal == null || ptal == "")
                    {
                        if(playerBase.MH.Type == Weapon.WeaponType.Dagger)
                        {
                            // Combat Daggers
                            ptal = "005303103-3203052020550100201-05";
                        }
                        else if (playerBase.MH.Type == Weapon.WeaponType.Fist)
                        {
                            // Combat Fists
                            ptal = "005323105-3210052020050105231";
                        }
                        else
                        {
                            // Combat Sword
                            ptal = "005323105-3210052020050150231";
                        }
                    }

                    string[] talents = ptal.Split('-');
                    string assass = talents.Length > 0 ? talents[0] : "";
                    string combat = talents.Length > 1 ? talents[1] : "";
                    string subti = talents.Length > 2 ? talents[2] : "";

                    // Assassination
                    playerBase.Talents.Add("IE", assass.Length > 0 ? (int)Char.GetNumericValue(assass[0]) : 0);
                    playerBase.Talents.Add("Malice", assass.Length > 2 ? (int)Char.GetNumericValue(assass[2]) : 0);
                    playerBase.Talents.Add("Ruth", assass.Length > 3 ? (int)Char.GetNumericValue(assass[3]) : 0);
                    playerBase.Talents.Add("Murder", (assass.Length > 4 && (jsonSim.Boss.Type == "Humanoid" || jsonSim.Boss.Type == "Giant" || jsonSim.Boss.Type == "Beast" || jsonSim.Boss.Type == "Dragonkin"))
                         ? (int)Char.GetNumericValue(assass[4]) : 0);
                    playerBase.Talents.Add("ISD", assass.Length > 5 ? (int)Char.GetNumericValue(assass[5]) : 0);
                    playerBase.Talents.Add("RS", assass.Length > 6 ? (int)Char.GetNumericValue(assass[6]) : 0);
                    playerBase.Talents.Add("Letha", assass.Length > 8 ? (int)Char.GetNumericValue(assass[8]) : 0);
                    // Combat
                    playerBase.Talents.Add("IG", combat.Length > 0 ? (int)Char.GetNumericValue(combat[0]) : 0);
                    playerBase.Talents.Add("ISS", combat.Length > 1 ? (int)Char.GetNumericValue(combat[1]) : 0);
                    playerBase.Talents.Add("IB", combat.Length > 3 ? (int)Char.GetNumericValue(combat[3]) : 0);
                    playerBase.Talents.Add("Prec", combat.Length > 5 ? (int)Char.GetNumericValue(combat[5]) : 0);
                    playerBase.Talents.Add("DS", combat.Length > 10 ? (int)Char.GetNumericValue(combat[10]) : 0);
                    playerBase.Talents.Add("DWS", combat.Length > 11 ? (int)Char.GetNumericValue(combat[11]) : 0);
                    playerBase.Talents.Add("BF", combat.Length > 13 ? (int)Char.GetNumericValue(combat[13]) : 0);
                    playerBase.Talents.Add("SS", combat.Length > 14 ? (int)Char.GetNumericValue(combat[14]) : 0);
                    playerBase.Talents.Add("FS", combat.Length > 15 ? (int)Char.GetNumericValue(combat[15]) : 0);
                    playerBase.Talents.Add("WE", combat.Length > 16 ? (int)Char.GetNumericValue(combat[16]) : 0);
                    playerBase.Talents.Add("Agg", combat.Length > 17 ? (int)Char.GetNumericValue(combat[17]) : 0);
                    playerBase.Talents.Add("AR", combat.Length > 18 ? (int)Char.GetNumericValue(combat[18]) : 0);
                    // Subtlety
                    playerBase.Talents.Add("Oppo", subti.Length > 1 ? (int)Char.GetNumericValue(subti[1]) : 0);
                }

                playerBase.CalculateAttributes();
                playerBase.CheckSets();
                playerBase.ApplySets();

                Log("\nPlayer :");
                Log(playerBase.ToString());

                bossBase = JsonUtil.JsonBoss.ToBoss(jsonSim.Boss, playerBase.Attributes.GetValue(Attribute.ArmorPen));

                Log("\nBoss (after raid debuffs) :");
                Log(bossBase.ToString());

                DateTime start = DateTime.Now;

                List<Task> tasks = new List<Task>();

                Enchantment we = new Enchantment(0, "Weights", new Attributes(new Dictionary<Attribute, double>()));
                playerBase.Buffs.Add(we);
                
                for(int done = 0; done < (statsWeights ? simOrder.Count : 1); done++)
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
                                    Console.WriteLine("Precision : ±{0:N2}%", Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average()));
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
                            if(done == 0)
                            {
                                nbSim = 0;
                            }

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
                                Console.WriteLine("Simulating {0} DPS, aiming for ±{1:N2}% precision...", simOrder[done], targetErrorPct);
                                Console.WriteLine("Sims done : {0:N2}", CurrentDpsList.Count);
                                Console.WriteLine("Sims running : {0:N2}", tasks.Count(t => !t.IsCompleted));
                                Console.WriteLine("Current precision : ±{0:N2}%", errorPct);

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

                string endMsg1 = string.Format("\n{0} simulations done in {1:N2} ms, for {2:N2} ms by sim", nbSim, time, time / nbSim);
                Console.WriteLine(endMsg1);
                Log(endMsg1);

                string endMsg2 = string.Format("Overall accuracy of results : ±{0:N2}%", ErrorList.Average());
                Console.WriteLine(endMsg2);
                Log(endMsg2);

                string endMsg3 = string.Format("\nGenerating statistics...");
                Console.WriteLine(endMsg3);

                if (statsWeights)
                {
                    double baseDps = DamagesList["Base"].Average();
                    Log(string.Format("\nBase : {0:N2} DPS", baseDps));

                    Log("\nWeights by DPS :");

                    double apDps = DamagesList["+50 AP"].Average();
                    double apDif = (apDps - baseDps) / 50;
                    if (apDif < 0) apDif = 0;
                    Log(string.Format("1 AP = {0:N4} DPS", apDif));

                    double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * (playerBase.Class == Player.Classes.Druid ? (1 + 0.04 * playerBase.GetTalentPoints("HW")) : 1);
                    Log(string.Format("1 Str = {0:N4} DPS = {1:N4} AP", strDif, strDif / apDif));

                    if (simOrder.Contains("+1% Crit"))
                    {
                        double critDps = DamagesList["+1% Crit"].Average();
                        double critDif = critDps - baseDps;
                        if (critDif < 0) critDif = 0;

                        double agiDif = Player.AgiToAPRatio(playerBase.Class) * apDif + Player.AgiToCritRatio(playerBase.Class) * 100 * critDif;
                        Log(string.Format("1 Agi = {0:N4} DPS = {1:N4} AP", agiDif, agiDif / apDif));

                        Log(string.Format("1% Crit = {0:N4} DPS = {1:N4} AP", critDif, critDif / apDif));
                    }
                    if (simOrder.Contains("+1% Hit"))
                    {
                        double hitDps = DamagesList["+1% Hit"].Average();
                        double hitDif = hitDps - baseDps;
                        if (hitDif < 0) hitDif = 0;
                        Log(string.Format("1% Hit = {0:N4} DPS = {1:N4} AP", hitDif, hitDif / apDif));
                    }
                    if (simOrder.Contains("+1% Haste"))
                    {
                        double hasteDps = DamagesList["+1% Haste"].Average();
                        double hasteDif = hasteDps - baseDps;
                        if (hasteDif < 0) hasteDif = 0;
                        Log(string.Format("1% Haste = {0:N4} DPS = {1:N4} AP", hasteDif, hasteDif / apDif));
                    }
                    if (simOrder.Contains("+50 Int"))
                    {
                        double intDps = DamagesList["+50 Int"].Average();
                        double intDif = (intDps - baseDps) / 50;
                        if (intDif < 0) intDif = 0;
                        Log(string.Format("1 Int = {0:N4} DPS = {1:N4} AP", intDif, intDif / apDif));
                    }
                    if (simOrder.Contains("+50 Spi"))
                    {
                        double spiDps = DamagesList["+50 Spi"].Average();
                        double spiDif = (spiDps - baseDps) / 50;
                        if (spiDif < 0) spiDif = 0;
                        Log(string.Format("1 Spi = {0:N4} DPS = {1:N4} AP", spiDif, spiDif / apDif));
                    }
                }
                else if (nbSim >= 1)
                {
                    double avgDps = CurrentDpsList.Average();

                    List<List<RegisteredAction>> totalActions = ResultsList["Base"].Select(r => r.Actions).ToList();
                    List<List<RegisteredEffect>> totalEffects = ResultsList["Base"].Select(r => r.Effects).ToList();

                    Log(string.Format("\nAverage DPS : {0:N2} dps (±{1:N2})", avgDps, Stats.MeanStdDev(CurrentDpsList.ToArray())));

                    //List<string> logList = totalActions.SelectMany(a => a.Select(t => t.Action.ToString()).OrderBy(b => b)).Distinct().ToList();
                    List<string> logList = new List<string>() { "AA MH", "AA OH" };
                    if (playerBase.Class == Player.Classes.Warrior)
                        logList.AddRange(new List<string>() { "Bloodthirst", "Whirlwind", "Heroic Strike", "Execute", "Hamstring" });
                    else if (playerBase.Class == Player.Classes.Druid)
                        logList.AddRange(new List<string>() { "Shred", "Ferocious Bite", "Shift" });
                    else if (playerBase.Class == Player.Classes.Rogue)
                        logList.AddRange(new List<string>() { "Sinister Strike", "Backstab", "Eviscerate", "Ambush", "Instant Poison" });

                    logList.AddRange(new List<string>() { "Deathbringer", "Vis'kag the Bloodletter" });

                    foreach (string ac in logList)
                    {
                        double totalAc = totalActions.Select(a => a.Where(t => t.Action.ToString().Equals(ac)).Count()).Sum();
                        if (totalAc > 0)
                        {
                            double avgAcUse = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac)));
                            double avgAcDps = totalActions.Average(a => a.Where(t => t.Action.ToString().Equals(ac)).Sum(r => r.Result.Damage / fightLength));
                            double avgAcDmg = totalActions.Sum(a => a.Where(t => t.Action.ToString().Equals(ac)).Sum(r => r.Result.Damage / totalAc));
                            string res = "\nAverage stats for [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)\n\tAverage of {2:N2} damage for {3:N2} uses (or 1 use every {4:N2}s)", avgAcDps, avgAcDps / avgDps * 100, avgAcDmg, avgAcUse, fightLength / avgAcUse);
                            double hitPct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Hit)) / avgAcUse * 100;
                            double critPct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Crit)) / avgAcUse * 100;
                            res += string.Format("\n\t{0:N2}% Hit, {1:N2}% Crit", hitPct, critPct);
                            if (totalActions.Any(l => l.Any(a => a.Action.ToString().Equals(ac) && a.Action.Magic)))
                            {
                                double resistPct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Resist)) / avgAcUse * 100;
                                res += string.Format(", {0:N2}% Resist", resistPct);
                            }
                            else
                            {
                                double missPct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Miss)) / avgAcUse * 100;
                                double glancePct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Glance)) / avgAcUse * 100;
                                double dodgePct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Dodge)) / avgAcUse * 100;
                                res += string.Format(", {0:N2}% Miss, {1:N2}% Glancing, {2:N2}% Dodge", missPct, glancePct, dodgePct);
                            }
                            Log(res);
                        }
                    }

                    //logList = totalEffects.SelectMany(a => a.Select(t => t.Effect.ToString()).OrderBy(b => b)).Distinct().ToList();
                    logList = new List<string>() { };
                    if (playerBase.Class == Player.Classes.Warrior)
                        logList.AddRange(new List<string>() { "Deep Wounds" });

                    foreach (string ac in logList)
                    {
                        double totalAc = totalEffects.Select(a => a.Where(t => t.Effect.ToString().Equals(ac)).Count()).Sum();
                        if(totalAc > 0)
                        {
                            double avgAcUse = totalEffects.Average(a => a.Count(t => t.Effect.ToString().Equals(ac)));
                            double avgAcDps = totalEffects.Average(a => a.Where(t => t.Effect.ToString().Equals(ac)).Sum(r => r.Damage / fightLength));
                            double avgAcDmg = totalEffects.Sum(a => a.Where(t => t.Effect.ToString().Equals(ac)).Sum(r => r.Damage / totalAc));
                            string res = "\nAverage stats for [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)\n\tAverage of {2:N2} damage for {3:N2} ticks (or 1 tick every {4:N2}s)", avgAcDps, avgAcDps / avgDps * 100, avgAcDmg, avgAcUse, fightLength / avgAcUse);
                            double uptime = 3 / (fightLength / avgAcUse) * 100;
                            res += string.Format("\n\t{0:N2}% Uptime", uptime);
                            Log(res);
                        }
                    }
                }

                if (!debug)
                {
                    Directory.CreateDirectory(logsFileDir);
                }

                string path = debug ? Path.Combine(debugPath, logsFileName + txt) : Path.Combine(logsFileDir, logsFileName + DateTime.Now.ToString("_yyyyMMdd-HHmmss-fff") + txt);
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

        public static void Debug(object str)
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
            player.BaseMana = playerBase.BaseMana;
            player.WeaponSkill = playerBase.WeaponSkill;

            player.MH.DamageMin = playerBase.MH.DamageMin;
            player.MH.DamageMax = playerBase.MH.DamageMax;
            if(player.OH != null)
            {
                player.OH.DamageMin = playerBase.OH.DamageMin;
                player.OH.DamageMax = playerBase.OH.DamageMax;
            }

            player.Sets = playerBase.Sets;
            player.ApplySets();

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

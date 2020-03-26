﻿using System;
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
        public static string playerJsonFileName = "player.json";
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
        
        public static List<string> simOrder = new List<string>(){
            "Base",
            "+50 AP",
            "+1% Hit","+1% Crit", "+1% Haste", "+50 Int", "+50 Spi",
            "+10 DPS MH", "+10 DPS OH",
            "+5 MH Skill", "+5 OH Skill"
            //"1","2","3","4","5","6","7","8","9",
        };
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
                    { "+10 DPS MH", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.WeaponDamageMH, 10 }
                            })},
                    { "+10 DPS OH", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.WeaponDamageOH, 10 }
                            })},
                    { "+5 MH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                    { "+5 OH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                    /*
                    { "1", new Attributes(new Dictionary<Attribute, double>()) },
                    { "2", new Attributes(new Dictionary<Attribute, double>()) },
                    { "3", new Attributes(new Dictionary<Attribute, double>()) },
                    { "4", new Attributes(new Dictionary<Attribute, double>()) },
                    { "5", new Attributes(new Dictionary<Attribute, double>()) },
                    { "6", new Attributes(new Dictionary<Attribute, double>()) },
                    { "7", new Attributes(new Dictionary<Attribute, double>()) },
                    { "8", new Attributes(new Dictionary<Attribute, double>()) },
                    { "9", new Attributes(new Dictionary<Attribute, double>()) },
                    */
                };

        public static JsonUtil.JsonSim jsonSim;
        public static JsonUtil.JsonPlayer jsonPlayer;

        static void Main(string[] args)
        {
            try
            {
                // Retrieving jsons

                string simString = File.ReadAllText(debug ? Path.Combine(debugPath, simJsonFileName) : simJsonFileName);
                jsonSim = JsonConvert.DeserializeObject<JsonUtil.JsonSim>(simString);
                string playerString = File.ReadAllText(debug ? Path.Combine(debugPath, playerJsonFileName) : playerJsonFileName);
                jsonPlayer = JsonConvert.DeserializeObject<JsonUtil.JsonPlayer>(playerString);

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
                
                playerBase = JsonUtil.JsonPlayer.ToPlayer(jsonPlayer);

                if (playerBase.Class == Player.Classes.Rogue || playerBase.Class == Player.Classes.Warrior)
                {
                    simOrder.Remove("+50 Int");
                    simOrder.Remove("+50 Spi");
                }
                if(!playerBase.DualWielding)
                {
                    simOrder.Remove("+10 DPS OH");
                    simOrder.Remove("+5 OH Skill");
                }
                else if(playerBase.MH.Type == playerBase.OH.Type)
                {
                    simOrder.Remove("+5 OH Skill");
                }
                if(playerBase.Class == Player.Classes.Druid)
                {
                    simOrder.Remove("+10 DPS MH");
                    simOrder.Remove("+5 MH Skill");
                }

                if (simOrder.Contains("+10 DPS MH"))
                {
                    simBonusAttribs["+10 DPS MH"].SetValue(Attribute.WeaponDamageMH, simBonusAttribs["+10 DPS MH"].GetValue(Attribute.WeaponDamageMH) * playerBase.MH.Speed);
                }
                if (simOrder.Contains("+10 DPS OH"))
                {
                    simBonusAttribs["+10 DPS OH"].SetValue(Attribute.WeaponDamageOH, simBonusAttribs["+10 DPS OH"].GetValue(Attribute.WeaponDamageOH) * playerBase.OH.Speed);
                }
                if (simOrder.Contains("+5 MH Skill"))
                {
                    simBonusAttribs["+5 MH Skill"].SetValue(AttributeUtil.FromWeaponType(playerBase.MH.Type), 5);
                }
                if (simOrder.Contains("+5 OH Skill"))
                {
                    simBonusAttribs["+5 OH Skill"].SetValue(AttributeUtil.FromWeaponType(playerBase.OH.Type), 5);
                }

                playerBase.SetupTalents(jsonPlayer.Talents);
                playerBase.CalculateAttributes();
                playerBase.CheckSets();
                playerBase.ApplySets();

                Log("\nPlayer :");
                Log(playerBase.ToString());
                Log(playerBase.MainWeaponInfo());

                if(playerBase.Class == Player.Classes.Mage)
                {
                    int reduction = playerBase.GetTalentPoints("AS") * 5;
                    if(reduction > 0)
                    {
                        List<JsonUtil.JsonBoss.SchoolResist> l = new List<JsonUtil.JsonBoss.SchoolResist>();
                        foreach (JsonUtil.JsonBoss.SchoolResist sr in jsonSim.Boss.SchoolResists)
                        {
                            l.Add(new JsonUtil.JsonBoss.SchoolResist(sr.School, Math.Max(0, sr.Resist - reduction)));
                        }
                        jsonSim.Boss.SchoolResists = l;
                    }
                }
                bossBase = JsonUtil.JsonBoss.ToBoss(jsonSim.Boss, playerBase.Attributes.GetValue(Attribute.ArmorPen));

                Log("\nBoss (after raid debuffs) :");
                Log(bossBase.ToString());

                DateTime start = DateTime.Now;

                List<Task> tasks = new List<Task>();

                Enchantment we = new Enchantment(0, "Weights", new Attributes(new Dictionary<Attribute, double>()));
                playerBase.Buffs.Add(we);
                
                // Doing simulations
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
                    //Log(simOrder[done] + " : " + CurrentDpsList.Average());
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

                string endMsg1 = string.Format("\n{0} simulations done in {1:N2} ms, for {2:N2} ms by sim", nbSim, time, time / nbSim);
                Console.WriteLine(endMsg1);
                Log(endMsg1);

                string endMsg2 = string.Format("Overall accuracy of results : ±{0:N2}%", ErrorList.Average());
                Console.WriteLine(endMsg2);
                Log(endMsg2);

                string endMsg3 = string.Format("\nGenerating results...");
                Console.WriteLine(endMsg3);

                if (statsWeights)
                {
                    double baseDps = DamagesList["Base"].Average();
                    Log(string.Format("\nBase : {0:N2} DPS", baseDps));

                    Log("\nWeights by DPS :");

                    double apDif = 0;
                    if(simOrder.Contains("+50 AP"))
                    {
                        double apDps = DamagesList["+50 AP"].Average();
                        apDif = (apDps - baseDps) / 50;
                        if (apDif < 0) apDif = 0;
                        Log(string.Format("1 AP = {0:N4} DPS", apDif));

                        double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * (playerBase.Class == Player.Classes.Druid ? (1 + 0.04 * playerBase.GetTalentPoints("HW")) : 1);
                        Log(string.Format("1 Str = {0:N4} DPS = {1:N4} AP", strDif, strDif / apDif));
                    }
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
                    if (simOrder.Contains("+10 DPS MH"))
                    {
                        double mhDps = DamagesList["+10 DPS MH"].Average();
                        double mhDif = (mhDps - baseDps) / simBonusAttribs["+10 DPS MH"].GetValue(Attribute.WeaponDamageMH);
                        if (mhDif < 0) mhDif = 0;
                        Log(string.Format("+1 MH DPS = {0:N4} DPS = {1:N4} AP", mhDif, mhDif / apDif));
                    }
                    if (simOrder.Contains("+10 DPS OH"))
                    {
                        double ohDps = DamagesList["+10 DPS OH"].Average();
                        double ohDif = (ohDps - baseDps) / simBonusAttribs["+10 DPS OH"].GetValue(Attribute.WeaponDamageOH);
                        if (ohDif < 0) ohDif = 0;
                        Log(string.Format("+1 OH DPS = {0:N4} DPS = {1:N4} AP", ohDif, ohDif / apDif));
                    }
                    if (simOrder.Contains("+5 MH Skill"))
                    {
                        double mhSkillDps = DamagesList["+5 MH Skill"].Average();
                        double mhSkillDif = mhSkillDps - baseDps;
                        if (mhSkillDif < 0) mhSkillDif = 0;
                        Log(string.Format("+5 MH Skill = {0:N4} DPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));
                    }
                    if (simOrder.Contains("+5 OH Skill"))
                    {
                        double ohSkillDps = DamagesList["+5 OH Skill"].Average();
                        double ohSkillDif = ohSkillDps - baseDps;
                        if (ohSkillDif < 0) ohSkillDif = 0;
                        Log(string.Format("+5 OH Skill = {0:N4} DPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));
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
                        logList.AddRange(new List<string>() { "Slam", "Bloodthirst", "Whirlwind", "Heroic Strike", "Execute", "Hamstring" });
                    else if (playerBase.Class == Player.Classes.Druid)
                        logList.AddRange(new List<string>() { "Shred", "Ferocious Bite", "Shift" });
                    else if (playerBase.Class == Player.Classes.Paladin)
                    {
                        logList.AddRange(new List<string>()
                        {
                            SealOfCommand_Rank1.NAME, SealOfCommand_Rank5.NAME,
                            SealOfCommandProc.NAME,
                            SealOfRighteousnessProc.NAME, JudgementOfRighteousness.NAME,
                            JudgementOfCommand_Rank1.NAME, JudgementOfCommand_Rank5.NAME,
                        });
                    }
                    else if (playerBase.Class == Player.Classes.Rogue)
                        logList.AddRange(new List<string>() { "Sinister Strike", "Backstab", "Eviscerate", "Ambush", "Instant Poison" });
                    else if (playerBase.Class == Player.Classes.Warlock)
                        logList.AddRange(new List<string>() { "Shadow Bolt" });

                    logList.AddRange(new List<string>() { "Deathbringer", "Vis'kag the Bloodletter", "Perdition's Blade" });

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
                            if (totalActions.Any(l => l.Any(a => a.Action.ToString().Equals(ac) && a.Action.School != School.Physical)))
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
                                if(Simulation.tank)
                                {
                                    double parryPct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Parry)) / avgAcUse * 100;
                                    res += string.Format(", {0:N2}% Parry", parryPct);
                                }
                            }
                            Log(res);
                        }
                    }

                    //logList = totalEffects.SelectMany(a => a.Select(t => t.Effect.ToString()).OrderBy(b => b)).Distinct().ToList();
                    logList = new List<string>() { };
                    if (playerBase.Class == Player.Classes.Warrior)
                        logList.AddRange(new List<string>() { "Deep Wounds" });
                    else if (playerBase.Class == Player.Classes.Warlock)
                        logList.AddRange(new List<string>() { "Corruption", "Malediction of Agony" });
                    else if (playerBase.Class == Player.Classes.Paladin)
                    {
                        logList.AddRange(new List<string>()
                        {
                            Vengeance.NAME,
                            ConsecrationDoT_Rank1.NAME, ConsecrationDoT_Rank5.NAME,
                        });
                    }

                    //  Non-class-specific stuff
                    logList.AddRange(new List<string>()
                        {
                            SpellVulnerability.NAME,
                        });

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
            Player player;
            switch(playerBase.Class)
            {
                case Player.Classes.Druid:
                    player = new Druid(playerBase);
                    break;
                case Player.Classes.Hunter:
                    player = new Hunter(playerBase);
                    break;
                case Player.Classes.Mage:
                    player = new Mage(playerBase);
                    break;
                case Player.Classes.Paladin:
                    player = new Paladin(playerBase);
                    break;
                case Player.Classes.Priest:
                    player = new Priest(playerBase);
                    break;
                case Player.Classes.Rogue:
                    player = new Rogue(playerBase);
                    break;
                case Player.Classes.Shaman:
                    player = new Shaman(playerBase);
                    break;
                case Player.Classes.Warlock:
                    player = new Warlock(playerBase);
                    break;
                case Player.Classes.Warrior:
                    player = new Warrior(playerBase);
                    break;
                default:
                    throw new NotSupportedException("This class isn't supported : " + playerBase.Class);
            }

            Boss boss = new Boss(bossBase);

            Simulation s = new Simulation(player, boss, fightLength, bossAutoLife, bossLowLifeTime);
            s.StartSim();
        }
    }
}

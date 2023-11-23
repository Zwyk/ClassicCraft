using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using ClassicCraftGUI;
using System.Runtime.InteropServices.ComTypes;

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
        public RegisteredEffect(Effect effect, int damage, double time, int threat)
        {
            Effect = effect;
            Damage = damage;
            Time = time;
            Threat = threat;
        }

        public Effect Effect { get; set; }
        public double Time { get; set; }
        public int Damage { get; set; }
        public int Threat { get; set; }
    }

    public enum Version
    {
        Vanilla,
        TBC,
        SoD,
    }

    public class Program
    {
        public static Version version = Version.Vanilla;

        public static JsonUtil.Config Config = null;

        public static string CONFIG_FILE = "config.json";
        public static string CONFIG_FOLDER = "Config";
        public static string PLAYER_CONFIG_FOLDER = "Player";
        public static string SIM_CONFIG_FOLDER = "Sim";
        public static string logsFileDir = "Logs";
        public static string logsFileName = "logs";
        public static string txt = ".txt";

        public static bool debug = true;
        public static string debugPath = @".\..\..";

        public static Player playerBase = null;
        public static Dictionary<string, Player> playerList = null;
        public static Boss bossBase = null;

        public static int nbSim = 1000;
        public static double targetErrorPct = 0.5;
        public static bool targetError = true;
        public static bool threading = true;
        public static bool logFight = false;
        public static bool calcDpss = false;

        public static bool statsWeights = false;
        public static bool comparing = false;

        public static int nbTargets = 1;

        //public static List<Attribute> toWeight = null;
        //public static Attribute weighted;

        public static int CONCURRENT_SIMS = 1000;
        public static int nbTasksForSims;

        public class StatsData
        {
            public string Name;
            public double AvgDmg = 0;
            public double AvgDPS = 0;
            public double AvgThreat = 0;
            public double AvgTPS = 0;
            public double AvgUses = 0;
            public double AvgHit = 0;
            public double AvgCrit = 0;
            public double AvgMiss = 0;
            public double AvgGlance = 0;
            public double AvgDodge = 0;
            public double AvgParry = 0;
            public double AvgResist = 0;

            public PlayerObject Sample;

            public StatsData(string name, PlayerObject sample)
            {
                Name = name;
                Sample = sample;
            }
        }

        public class SimData
        {
            public int NB = 0;
            public double AvgSimLength = 0;
            public Dictionary<string, StatsData> DataActions = new Dictionary<string, StatsData>();
            public Dictionary<string, StatsData> DataEffects = new Dictionary<string, StatsData>();

            public SimData()
            {
            }
        }
        
        private static SimData CurrentData;
        private static List<double> CurrentDpsList;
        private static List<double> CurrentTpsList;
        
        private static Dictionary<string, double> SimsAvgDPS;
        private static Dictionary<string, double> SimsAvgTPS;
        private static List<double> ErrorList;
        private static Dictionary<string, double> SimsDPSStDev;
        private static Dictionary<string, double> SimsTPSStDev;

        public static List<string> logListActions;
        public static List<string> logListEffects;

        public static string logs;

        public static List<string> simOrder;
        public static Dictionary<string, Attributes> simBonusAttribs;

        public static JsonUtil.JsonSim jsonSim;
        public static JsonUtil.JsonPlayer jsonPlayer;

        public enum DisplayMode
        {
            Console,
            GUI,
        }

        public static DisplayMode Display = DisplayMode.GUI;
        public static MainWindow GUI = null;
        public static bool Running = false;

        public static void ConsoleRun()
        {
            LoadConfigJsons();
            Run(null, debugPath);
        }

        public static string basePath()
        {
            return debug ? debugPath : "";
        }

        public static void Reset()
        {
            SimsAvgDPS = new Dictionary<string, double>();
            SimsAvgTPS = new Dictionary<string, double>();
            ErrorList = new List<double>();
            SimsDPSStDev = new Dictionary<string, double>();
            SimsTPSStDev = new Dictionary<string, double>();

            logs = "";

            simOrder = new List<string>(){
                "Base",
                "AP", "SP",
                "Hit","Crit", "Haste",
                "DPS MH", "DPS OH",
                "+1 MH Skill", "+1 OH Skill",
                "+5 MH Skill", "+5 OH Skill",
                "SpellHit","SpellCrit",
                "Int", "Spi", "MP5",
            };

            simBonusAttribs = new Dictionary<string, Attributes>()
            {
                { "Base", new Attributes() },
                { "AP", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, version == Version.TBC ? 100 : 50 }
                        })},
                { "SP", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.SP, version == Version.TBC ? 100 : 50 }
                        })},
                { "Hit", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.HitChance, 0.01 }
                        })},
                { "Crit", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.CritChance, 0.01 }
                        })},
                { "Haste", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.Haste, 0.01 }
                        })},
                { "Expertise", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.Expertise, 0.01 }
                        })},
                { "+500 ArPen", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.ArmorPen, 500 }
                        })},
                { "+1000 ArPen", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.ArmorPen, 1000 }
                        })},
                { "DPS MH", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.WeaponDamageMH, 10 }
                        })},
                { "DPS OH", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.WeaponDamageOH, 10 }
                        })},
                { "+1 MH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "+1 OH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "+5 MH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "+5 OH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "Block Value", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.BlockValue, 100 }
                        })},
                { "SpellHit", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.SpellHitChance, 0.01 }
                        })},
                { "SpellCrit", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.SpellCritChance, 0.01 }
                        })},
                { "Int", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.Intellect, version == Version.TBC ? 100 : 50 }
                        })},
                { "Spi", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.Spirit, version == Version.TBC ? 100 : 50 }
                        })},
                { "MP5", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.MP5, (version == Version.TBC ? 60 : 30) }
                        })},
            };
        }

        public static void LoadConfig()
        {
            string configStr = File.ReadAllText(Path.Combine(basePath(), CONFIG_FOLDER, CONFIG_FILE));
            Config = JsonConvert.DeserializeObject<JsonUtil.Config>(configStr);
        }

        public static void SaveConfig()
        {
            File.WriteAllText(Path.Combine(basePath(), CONFIG_FOLDER, CONFIG_FILE), JsonConvert.SerializeObject(Config, Formatting.Indented));
        }

        public static void LoadConfigJsons(bool player = true, bool sim = true)
        {
            if (sim)
            {
                jsonSim = ReadSimJson(Config.Sim);
            }
            if (player)
            {
                jsonPlayer = ReadPlayerJson(Config.Player);
            }
        }

        public static JsonUtil.JsonSim ReadSimJson(string fileName)
        {
            try
            {
                string simString = File.ReadAllText(Path.Combine(basePath(), CONFIG_FOLDER, SIM_CONFIG_FOLDER, fileName + ".json"));
                return JsonConvert.DeserializeObject<JsonUtil.JsonSim>(simString);
            }
            catch (Exception e)
            {
                Output("Player JSON loading failed :\n" + e);
                return null;
            }
        }

        public static JsonUtil.JsonPlayer ReadPlayerJson(string fileName)
        {
            try
            {
                string playerString = File.ReadAllText(Path.Combine(basePath(), CONFIG_FOLDER, PLAYER_CONFIG_FOLDER, fileName + ".json"));
                return JsonConvert.DeserializeObject<JsonUtil.JsonPlayer>(playerString);
            }
            catch (Exception e)
            {
                Output("Player JSON loading failed :\n" + e);
                return null;
            }
        }

        public static void SaveJsons(bool player = true, bool sim = true)
        {
            try
            {
                if(sim) File.WriteAllText(Path.Combine(basePath(), CONFIG_FOLDER, SIM_CONFIG_FOLDER, Config.Sim + ".json"), JsonConvert.SerializeObject(jsonSim, Formatting.Indented));
                if(player) File.WriteAllText(Path.Combine(basePath(), CONFIG_FOLDER, PLAYER_CONFIG_FOLDER, Config.Player + ".json"), JsonConvert.SerializeObject(jsonPlayer, Formatting.Indented));
            }
            catch (Exception e)
            {
                Output("Json saving failed :\n" + e);
            }
        }

        public static void Run(MainWindow gui = null, string customPath = null)
        {
            try
            {
                Running = true;

                Reset();

                GUI = gui;
                Display = GUI == null ? DisplayMode.Console : DisplayMode.GUI;
                GUI.SetProgress(0);
                GUISetProgressText("Setting up...");

                if(comparing && !jsonSim.Compare.Contains(Config.Player))
                {
                    jsonPlayer = ReadPlayerJson(jsonSim.Compare[0]);
                }

                if (jsonPlayer.Ver == "Vanilla") version = Version.Vanilla;
                else if (jsonPlayer.Ver == "TBC") version = Version.TBC;

                if (jsonPlayer.Level > 60) version = Version.TBC;
                else version = Version.Vanilla;

                if (customPath != null)
                {
                    debugPath = customPath;
                }

                nbSim = jsonSim.NbSim;
                targetErrorPct = jsonSim.TargetErrorPct;
                targetError = jsonSim.TargetError;
                logFight = jsonSim.LogFight;
                statsWeights = jsonSim.StatsWeights;
                nbTargets = jsonSim.NbTargets;

                if (targetError)
                {
                    if (targetErrorPct > 0.5) nbTasksForSims = CONCURRENT_SIMS / 10;
                    else if (targetErrorPct > 0.3) nbTasksForSims = CONCURRENT_SIMS / 2;
                    else nbTasksForSims = CONCURRENT_SIMS;
                }
                
                Log(string.Format("Date : {0}\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                Log(string.Format("Fight length : {0} seconds (±{1}%)", jsonSim.FightLength, jsonSim.FightLengthMod * 100));
                Log(string.Format("Number of targets : {0}", jsonSim.NbTargets));

                threading = !logFight;
                if (logFight)
                {
                    targetError = false;
                    statsWeights = false;

                    if (nbSim > 10)
                    {
                        nbSim = 10;
                    }
                }

                comparing = jsonSim.DoCompare;

                if (!comparing)
                {
                    jsonSim.Compare = new List<string>() { Config.Player };
                }
                else
                {
                    statsWeights = false;
                    logFight = false;
                }

                playerList = new Dictionary<string, Player>();
                foreach (string p in jsonSim.Compare)
                {
                    playerList.Add(p, JsonUtil.JsonPlayer.ToPlayer(ReadPlayerJson(p), jsonSim.Tanking, jsonSim.Facing));
                }
                playerBase = playerList.First().Value;

                if (statsWeights)
                {
                    if (playerBase.Class == Player.Classes.Rogue || playerBase.Class == Player.Classes.Warrior)
                    {
                        simOrder.Remove("SP");
                        simOrder.Remove("Int");
                        simOrder.Remove("Spi");
                        simOrder.Remove("MP5");
                    }
                    if (playerBase.OH == null || !playerBase.DualWielding)
                    {
                        simOrder.Remove("DPS OH");
                        simOrder.Remove("+1 OH Skill");
                        simOrder.Remove("+5 OH Skill");
                    }
                    else if (playerBase.MH.Type == playerBase.OH.Type)
                    {
                        simOrder.Remove("+1 OH Skill");
                        simOrder.Remove("+5 OH Skill");
                    }
                    if (playerBase.Class == Player.Classes.Druid
                        || (!playerBase.Tanking && playerBase.Class == Player.Classes.Warlock)
                        || playerBase.Class == Player.Classes.Mage
                        || playerBase.Class == Player.Classes.Priest
                        || playerBase.MH == null)
                    {
                        simOrder.Remove("DPS MH");
                        simOrder.Remove("+1 MH Skill");
                        simOrder.Remove("+5 MH Skill");
                    }

                    if ((!playerBase.Tanking && playerBase.Class == Player.Classes.Warlock)
                        || playerBase.Class == Player.Classes.Mage
                        || playerBase.Class == Player.Classes.Priest)
                    {
                        simOrder.Remove("AP");
                        simOrder.Remove("Hit");
                        simOrder.Remove("Crit");
                        simOrder.Remove("Haste");
                    }
                    else if(playerBase.Class !=  Player.Classes.Warlock)
                    {
                        simOrder.Remove("SP");
                        simOrder.Remove("SpellHit");
                        simOrder.Remove("SpellCrit");
                    }

                    if (version == Version.TBC)
                    {
                        simOrder.Remove("+1 MH Skill");
                        simOrder.Remove("+5 MH Skill");
                        simOrder.Remove("+1 OH Skill");
                        simOrder.Remove("+5 OH Skill");

                        if (playerBase.Class == Player.Classes.Rogue || playerBase.Class == Player.Classes.Warrior
                            || playerBase.Class == Player.Classes.Druid || playerBase.Class == Player.Classes.Shaman
                            || playerBase.Class == Player.Classes.Paladin || playerBase.Class == Player.Classes.Hunter)
                        {
                            simOrder.Insert(5, "Expertise");
                            simOrder.Insert(6, "+500 ArPen");
                            //simOrder.Insert(7, "+1000 ArPen");
                        }
                    }
                    if (jsonSim.Tanking && jsonSim.TankHitRage > 0 && jsonSim.TankHitEvery > 0 && jsonPlayer.Class == "Warrior"
                        && jsonPlayer.Weapons["OH"] != null && jsonPlayer.Weapons["OH"].Slot == "Shield")
                    {
                        simOrder.Add("Block Value");
                    }

                    if (simOrder.Contains("DPS MH"))
                    {
                        double dmg = simBonusAttribs["DPS MH"].GetValue(Attribute.WeaponDamageMH) * playerBase.MH.Speed;
                        simBonusAttribs["DPS MH"].SetValue(Attribute.WeaponDamageMH, dmg);
                    }
                    if (simOrder.Contains("DPS OH"))
                    {
                        double dmg = simBonusAttribs["DPS OH"].GetValue(Attribute.WeaponDamageOH) * playerBase.OH.Speed;
                        simBonusAttribs["DPS OH"].SetValue(Attribute.WeaponDamageOH, dmg);
                    }
                    if (simOrder.Contains("+1 MH Skill"))
                    {
                        simBonusAttribs["+1 MH Skill"].SetValue(AttributeUtil.FromWeaponType(playerBase.MH.Type), 1);
                    }
                    if (simOrder.Contains("+1 OH Skill"))
                    {
                        simBonusAttribs["+1 OH Skill"].SetValue(AttributeUtil.FromWeaponType(playerBase.OH.Type), 1);
                    }
                    if (simOrder.Contains("+5 MH Skill"))
                    {
                        simBonusAttribs["+5 MH Skill"].SetValue(AttributeUtil.FromWeaponType(playerBase.MH.Type), 5);
                    }
                    if (simOrder.Contains("+5 OH Skill"))
                    {
                        simBonusAttribs["+5 OH Skill"].SetValue(AttributeUtil.FromWeaponType(playerBase.OH.Type), 5);
                    }
                }

                /*
                simOrder.Remove("+500 ArPen");
                simOrder.Remove("+1000 ArPen");
                simOrder.Remove("Expertise");
                simOrder.Remove("Hit");
                simOrder.Remove("Crit");
                simOrder.Remove("Haste");
                simOrder.Remove("DPS MH");
                simOrder.Remove("DPS OH");
                */

                if (jsonPlayer.Class == "Mage")
                {
                    int reduction = playerBase.GetTalentPoints("AS") * 5;
                    if (reduction > 0)
                    {
                        foreach (string k in jsonSim.Boss.SchoolResists.Keys)
                        {
                            jsonSim.Boss.SchoolResists[k] = Math.Max(0, jsonSim.Boss.SchoolResists[k] - reduction);
                        }
                    }
                }

                DateTime start = DateTime.Now;

                foreach (string ps in playerList.Keys)
                {
                    playerBase = playerList[ps];
                    playerBase.SetupTalents(jsonPlayer.Talents);
                    playerBase.CalculateAttributes();
                    playerBase.CheckSets();
                    playerBase.ApplySets();

                    Log("\nPlayer :");
                    Log(playerBase.ToString());
                    Log(playerBase.MainWeaponInfo());

                    bossBase = JsonUtil.JsonBoss.ToBoss(jsonSim.Boss);
                    int bossBaseArmor = bossBase.Armor;

                    Log("\nBoss :");
                    Log(bossBase.ToString(0, jsonSim.Boss.Armor));  // TODO : base magic armor
                    Log("After raid debuffs and player penetration :");
                    Log(bossBase.ToString(playerBase.Attributes.GetValue(Attribute.ArmorPen)) + "\n");

                    if (!statsWeights && !comparing)
                    {
                        //logListActions = totalActions.SelectMany(a => a.Select(t => t.Action.ToString()).OrderBy(b => b)).Distinct().ToList();
                        logListActions = new List<string>() { "AA MH", "AA OH", "AA Ranged", "AA Wand" };
                        if (playerBase.Class == Player.Classes.Warrior)
                            logListActions.AddRange(new List<string>() { "Slam", "Bloodthirst", "Mortal Strike", "Shield Slam", "Devastate", "Sunder Armor", "Revenge", "Whirlwind", "Thunder Clap", "Sweeping Strikes", "Cleave", "Heroic Strike", "Execute", "Hamstring", "Battle Shout" });
                        else if (playerBase.Class == Player.Classes.Druid)
                            logListActions.AddRange(new List<string>() { "Shred", "Ferocious Bite", "Shift", "Maul", "Swipe" });
                        else if (playerBase.Class == Player.Classes.Priest)
                            logListActions.AddRange(new List<string>() { "Mind Blast", "Mind Flay", "SW:P", "Devouring Plague" });
                        else if (playerBase.Class == Player.Classes.Rogue)
                            logListActions.AddRange(new List<string>() { "Sinister Strike", "Backstab", "Eviscerate", "Ambush", "Blade Flurry", "Instant Poison" });
                        else if (playerBase.Class == Player.Classes.Warlock)
                            logListActions.AddRange(new List<string>() { "Shadow Bolt", "Searing Pain", "Shadow Cleave", "Shadowburn", "Curse of Agony", "Corruption", "Drain Life" });

                        logListActions.AddRange(new List<string>() { "Thunderfury", "Deathbringer", "Vis'kag the Bloodletter", "Perdition's Blade", "Romulo's Poison Vial", "Syphon of the Nathrezim" });

                        //logListEffects = totalEffects.SelectMany(a => a.Select(t => t.Effect.ToString()).OrderBy(b => b)).Distinct().ToList();
                        logListEffects = new List<string>() { };
                        if (playerBase.Class == Player.Classes.Priest)
                            logListEffects.AddRange(new List<string>() { "Mind Flay", "SW:P", "Devouring Plague" });
                        if (playerBase.Class == Player.Classes.Rogue)
                            logListEffects.AddRange(new List<string>() { "Rupture", "Deadly Poison" });
                        if (playerBase.Class == Player.Classes.Warrior)
                            logListEffects.AddRange(new List<string>() { "Deep Wounds" });
                        if (playerBase.Class == Player.Classes.Warlock)
                            logListEffects.AddRange(new List<string>() { "Curse of Agony", "Corruption", "Drain Life" });

                        CurrentData = new SimData();
                    }

                    List<Task> tasks = new List<Task>();

                    Enchantment we = new Enchantment(0, "Weights", new Attributes(new Dictionary<Attribute, double>()));
                    playerBase.Buffs.Add(we);

                    // Doing simulations
                    for (int done = 0; done < (statsWeights ? simOrder.Count : 1); done++)
                    {
                        CurrentDpsList = new List<double>();
                        CurrentTpsList = new List<double>();

                        if (done > 0)
                        {
                            we.Attributes = simBonusAttribs[simOrder[done]];
                            playerBase.CalculateAttributes();

                            //Log(simOrder[done] + "\n\t" + bossBase.ToString(playerBase.Attributes.GetValue(Attribute.ArmorPen)));
                            //Log(simOrder[done] + "\n\t" + playerBase.ToString());
                        }

                        if (!threading && !targetError)
                        {
                            for (int i = 0; i < nbSim; i++)
                            {
                                string txt = string.Format("\n\n---SIM NUMBER {0}---\n", i + 1);
                                double pct = (i + 1) / nbSim * 100;
                                GUISetProgress(pct);
                                GUISetProgressText(txt);
                                if (pct > 0.001)
                                {
                                    double t = (DateTime.Now - start).TotalMilliseconds;
                                    GUISetProgressTime(TimeSpan.FromMilliseconds(t / (pct / 100) - t));
                                }
                                Log(txt);
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
                                    double pct = (double)ErrorList.Count / playerList.Count + (double)CurrentDpsList.Count / nbSim / playerList.Count * 100;
                                    GUISetProgress(pct);
                                    GUISetProgressText(String.Format("Simulating {0}{3} - {1}/{2}", simOrder[done], (statsWeights ? done : ErrorList.Count) + 1, statsWeights ? simOrder.Count : playerList.Count, comparing ? " for " + ps : ""));
                                    if (pct > 0.001)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        GUISetProgressTime(TimeSpan.FromMilliseconds(t / (pct / 100) - t));
                                    }
                                    string outputText = "";

                                    if (!logFight)
                                    {
                                        outputText += String.Format("Simulating {0}{1}...", simOrder[done], comparing ? " for " + playerList.Count : "");
                                        outputText += String.Format("\nSims done : {0:N0}", CurrentDpsList.Count);
                                        outputText += String.Format("\nSims running : {0:N0}", tasks.Count(t => !t.IsCompleted));

                                        lock (CurrentDpsList)
                                        {
                                            if (CurrentDpsList.Count > 1)
                                            {
                                                outputText += String.Format("\nCurrent precision : ±{0:N2}%", Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average()));
                                            }
                                        }
                                        Output(outputText, true, !logFight);
                                    }

                                    Thread.Sleep(TimeSpan.FromSeconds(Display == DisplayMode.Console ? 1.0 / 2 : 1.0 / 60));
                                }

                                if (!logFight)
                                {
                                    OutputClear();
                                    Output(String.Format("{0:N2}% ({1}/{2})", (double)CurrentDpsList.Count / nbSim * 100, CurrentDpsList.Count, nbSim));
                                }
                            }
                            else
                            {
                                if (done == 0)
                                {
                                    nbSim = 0;
                                }

                                double errorPct = 100;

                                while (errorPct > targetErrorPct || tasks.Count > 0)
                                {
                                    double currentPct = Math.Min(1, Math.Pow((100 - errorPct) / (100 - targetErrorPct), 0.2 / targetErrorPct * 1000)) * 100;

                                    double working = tasks.Count(t => !t.IsCompleted);
                                    while (errorPct > targetErrorPct
                                        && working < nbTasksForSims && (currentPct == 0 ||
                                            (CurrentDpsList.Count + working * 0.9) < (100 / currentPct * CurrentDpsList.Count)))
                                    {
                                        working++;
                                        nbSim++;
                                        tasks.Add(Task.Factory.StartNew(() => DoSim()));
                                    }

                                    foreach (Task t in tasks.Where(t => t.IsCompleted))
                                    {
                                        t.Dispose();
                                    }
                                    tasks.RemoveAll(t => t.IsCompleted);

                                    lock (CurrentDpsList)
                                    {
                                        if (CurrentDpsList.Count > 10)
                                        {
                                            errorPct = Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average());
                                        }
                                    }

                                    double pct = (comparing ? (double)ErrorList.Count / playerList.Count : (double)done / (statsWeights ? simOrder.Count : 1)) * 100 + currentPct / (statsWeights ? simOrder.Count : playerList.Count);
                                    GUISetProgress(pct);
                                    GUISetProgressText(String.Format("Simulating {0}{3} - {1}/{2}", simOrder[done], (statsWeights ? done : ErrorList.Count) + 1, statsWeights ? simOrder.Count : playerList.Count, comparing ? " for " + playerList.Count : ""));
                                    if (pct > 0.001)
                                    {
                                        double t = (DateTime.Now - start).TotalMilliseconds;
                                        GUISetProgressTime(TimeSpan.FromMilliseconds(t / (pct / 100) - t));
                                    }

                                    string outputText = "";
                                    outputText += String.Format("Simulating {0}{2}, aiming for minimum ±{1:N2}% precision...", simOrder[done], targetErrorPct, comparing ? " for " + ps : "");
                                    outputText += String.Format("\nSims done : {0:N0}", CurrentDpsList.Count);
                                    outputText += String.Format("\nSims running : {0:N0}", tasks.Count(t => !t.IsCompleted));
                                    outputText += String.Format("\nCurrent precision : ±{0:N2}%", errorPct);
                                    Output(outputText, true, true);

                                    if (errorPct <= targetErrorPct)
                                    {
                                        Output("Waiting for remaining simulations to complete...");
                                    }

                                    Thread.Sleep(TimeSpan.FromSeconds(Display == DisplayMode.Console ? 1.0 / 2 : 1.0 / 60));
                                }
                            }
                        }

                        ErrorList.Add(Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average()));
                        SimsAvgDPS.Add(comparing ? ps : simOrder[done], CurrentDpsList.Average());
                        SimsDPSStDev.Add(comparing ? ps : simOrder[done], Stats.StandardError(Stats.StdDev(CurrentDpsList.ToArray())));
                        if (jsonSim.Threat)
                        {
                            SimsAvgTPS.Add(comparing ? ps : simOrder[done], CurrentTpsList.Average());
                            SimsTPSStDev.Add(comparing ? ps : simOrder[done], Stats.StandardError(Stats.StdDev(CurrentTpsList.ToArray())));
                        }

                        //Log(simOrder[done] + " : " + CurrentDpsList.Average());
                    }
                }


                GUISetProgress(100);

                if (!logFight)
                {
                    OutputClear();
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

                Output("");
                string endMsg1 = string.Format("{0} simulations done in {1:N2} ms, for {2:N2} ms by sim", nbSim, time, time / nbSim);
                Output(endMsg1);
                Log(endMsg1);

                string endMsg2 = string.Format("Overall accuracy of results : ±{0:N2}% (±{1:N2} DPS)", ErrorList.Average(), ErrorList[0] / 100 * (comparing ? SimsAvgDPS.Average(s => s.Value) : SimsAvgDPS["Base"]));
                if(jsonSim.Threat) endMsg2 += string.Format(" (±{1:N2} TPS)", ErrorList.Average(), ErrorList[0] / 100 * (comparing ? SimsAvgTPS.Average(s => s.Value) : SimsAvgTPS["Base"]));
                Output(endMsg2);
                Log(endMsg2);

                string endMsg3 = string.Format("\nGenerating results...");
                Output(endMsg3);

                if (comparing)
                {
                    GUISetProgressText(String.Format("Generating Comparison Stats..."));

                    foreach(string ps in playerList.Keys)
                    {
                        Log(string.Format("\nStats for {0}:", ps));
                        if (jsonSim.Threat)
                        {
                            Log(string.Format("Average TPS : {0:N2} TPS (±{1:N2})", SimsAvgTPS[ps], SimsTPSStDev[ps]));
                        }
                        Log(string.Format("Average DPS : {0:N2} TPS (±{1:N2})", SimsAvgDPS[ps], SimsDPSStDev[ps]));
                    }
                }
                else if (statsWeights)
                {
                    GUISetProgressText(String.Format("Generating Stats Weights..."));

                    double weightsDone = 0;
                    double baseDif = 0;
                    double apDif = 0;
                    double spDif = 0;

                    bool hybrid = false;
                    string baseName = "AP";
                    if(new List<Player.Classes> { Player.Classes.Priest, Player.Classes.Warlock, Player.Classes.Mage }.Contains(playerBase.Class))
                    {
                        baseName = "SP";
                        if (playerBase.Tanking)
                        {
                            hybrid = true;
                        }
                    }
                        
                    // TPS
                    if (jsonSim.Threat)
                    {
                        double baseTps = SimsAvgTPS["Base"];
                        Log(string.Format("\nBase : {0:N2} TPS (±{1:N2})", baseTps, SimsTPSStDev["Base"]));

                        Log("\nWeights by TPS :");


                        if (simOrder.Contains("SP"))
                        {
                            double spTps = SimsAvgTPS["SP"];
                            spDif = Math.Max(0, (spTps - baseTps) / (version == Version.TBC ? 100 : 50));
                            if (baseName == "SP")
                            {
                                baseDif = spDif;
                            }
                            Log(string.Format("1 SP = {0:N4} TPS", baseDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("AP"))
                        {
                            double apTps = SimsAvgTPS["AP"];
                            apDif = Math.Max(0, (apTps - baseTps) / (version == Version.TBC ? 100 : 50));
                            if (baseName == "AP") baseDif = apDif;

                            string endStr = hybrid ? string.Format(" = {0:N4} {1}", apDif / baseDif, baseName) : "";
                            Log(string.Format("1 AP = {0:N4} TPS{1}", apDif, endStr));

                            double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * Player.BonusStrRatio(playerBase);
                            Log(string.Format("1 Str = {0:N4} TPS = {1:N4} {2}", strDif, strDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Crit"))
                        {
                            double critTps = SimsAvgTPS["Crit"];
                            double critDif = Math.Max(0, critTps - baseTps);

                            double agiDif = Player.AgiToAPRatio(playerBase) * Player.BonusAgiRatio(playerBase) * baseDif
                                + Player.AgiToCritRatio(playerBase.Class, playerBase.Level) * Player.BonusAgiRatio(playerBase) * 100 * critDif;
                            Log(string.Format("1 Agi = {0:N4} TPS = {1:N4} {2}", agiDif, agiDif / baseDif, baseName));

                            Log(string.Format("1{2} Crit = {0:N4} TPS = {1:N4} {3}", critDif, critDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Hit"))
                        {
                            double hitTps = SimsAvgTPS["Hit"];
                            double hitDif = Math.Max(0, hitTps - baseTps);
                            Log(string.Format("1{2} Hit = {0:N4} TPS = {1:N4} {3}", hitDif, hitDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Haste"))
                        {
                            double hasteTps = SimsAvgTPS["Haste"];
                            double hasteDif = Math.Max(0, hasteTps - baseTps);
                            Log(string.Format("1{2} Haste = {0:N4} TPS = {1:N4} {3}", hasteDif, hasteDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Expertise"))
                        {
                            double expTps = SimsAvgTPS["Expertise"];
                            double expDif = Math.Max(0, expTps - baseTps);
                            Log(string.Format("1{2} Expertise = {0:N4} TPS = {1:N4} {3}", expDif, expDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+500 ArPen"))
                        {
                            double arpenTps = SimsAvgDPS["+500 ArPen"];
                            double arpenDif = Math.Max(0, (arpenTps - baseTps) / 500);
                            Log(string.Format("1 Armor Pen = {0:N4} TPS = {1:N4} {2}", arpenDif, arpenDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1000 ArPen"))
                        {
                            double arpenTps = SimsAvgDPS["+1000 ArPen"];
                            double arpenDif = Math.Max(0, (arpenTps - baseTps) / 1000);
                            Log(string.Format("1 Armor Pen (+1000) = {0:N4} TPS = {1:N4} {2}", arpenDif, arpenDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("DPS MH"))
                        {
                            double mhTps = SimsAvgTPS["DPS MH"];
                            double mhDif = Math.Max(0, (mhTps - baseTps) / 10);
                            Log(string.Format("1 MH DPS = {0:N4} TPS = {1:N4} {2}", mhDif, mhDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("DPS OH"))
                        {
                            double ohTps = SimsAvgTPS["DPS OH"];
                            double ohDif = Math.Max(0, (ohTps - baseTps) / 10);
                            Log(string.Format("1 OH DPS = {0:N4} TPS = {1:N4} {2}", ohDif, ohDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 MH Skill"))
                        {
                            double mhSkillTps = SimsAvgTPS["+1 MH Skill"];
                            double mhSkillDif = Math.Max(0, mhSkillTps - baseTps);
                            Log(string.Format("1 MH Skill = {0:N4} TPS = {1:N4} {2}", mhSkillDif, mhSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 OH Skill"))
                        {
                            double ohSkillTps = SimsAvgTPS["+1 OH Skill"];
                            double ohSkillDif = Math.Max(0, ohSkillTps - baseTps);
                            Log(string.Format("1 OH Skill = {0:N4} TPS = {1:N4} {2}", ohSkillDif, ohSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 MH Skill"))
                        {
                            double mhSkillTps = SimsAvgTPS["+5 MH Skill"];
                            double mhSkillDif = Math.Max(0, mhSkillTps - baseTps);
                            Log(string.Format("5 MH Skill = {0:N4} TPS = {1:N4} {2}", mhSkillDif, mhSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 OH Skill"))
                        {
                            double ohSkillTps = SimsAvgTPS["+5 OH Skill"];
                            double ohSkillDif = Math.Max(0, ohSkillTps - baseTps);
                            Log(string.Format("5 OH Skill = {0:N4} TPS = {1:N4} {2}", ohSkillDif, ohSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("SpellCrit"))
                        {
                            double thisTps = SimsAvgTPS["SpellCrit"];
                            double thisDif = Math.Max(0, (thisTps - baseTps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                            Log(string.Format("1{2} SpellCrit = {0:N4} TPS = {1:N4} {3}", thisDif, thisDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("SpellHit"))
                        {
                            double thisTps = SimsAvgTPS["SpellHit"];
                            double thisDif = Math.Max(0, (thisTps - baseTps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                            Log(string.Format("1{2} SpellHit = {0:N4} TPS = {1:N4} {3}", thisDif, thisDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Block Value"))
                        {
                            double val = SimsAvgTPS["Block Value"];
                            double dif = Math.Max(0, (val - baseTps) / 100);
                            Log(string.Format("1 Block Value = {0:N4} TPS = {1:N4} {2}", dif, dif/ baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Int"))
                        {
                            double intTps = SimsAvgTPS["Int"];
                            double intDif = Math.Max(0, (intTps - baseTps) / (version == Version.TBC ? 100 : 50));
                            Log(string.Format("1 Int = {0:N4} TPS = {1:N4} {2}", intDif, intDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Spi"))
                        {
                            double spiTps = SimsAvgTPS["Spi"];
                            double spiDif = Math.Max(0, (spiTps - baseTps) / (version == Version.TBC ? 100 : 50));
                            Log(string.Format("1 Spi = {0:N4} TPS = {1:N4} {2}", spiDif, spiDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("MP5"))
                        {
                            double thisTps = SimsAvgTPS["MP5"];
                            double thisDif = Math.Max(0, (thisTps - baseTps)/30);
                            Log(string.Format("1 MP5 = {0:N4} TPS = {1:N4} {2}", thisDif, thisDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                    }

                    // DPS
                    if (true)
                    {
                        double baseDps = SimsAvgDPS["Base"];
                        Log(string.Format("\nBase : {0:N2} DPS (±{1:N2})", baseDps, SimsDPSStDev["Base"]));

                        Log("\nWeights by DPS :");
                        if (simOrder.Contains("SP"))
                        {
                            double apDps = SimsAvgDPS["SP"];
                            spDif = Math.Max(0, (apDps - baseDps) / (version == Version.TBC ? 100 : 50));
                            if (baseName == "SP") baseDif = spDif;
                            Log(string.Format("1 SP = {0:N4} DPS", spDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("AP"))
                        {
                            double apDps = SimsAvgDPS["AP"];
                            apDif = Math.Max(0, (apDps - baseDps) / (version == Version.TBC ? 100 : 50));
                            if (baseName == "AP") baseDif = apDif;

                            string endStr = hybrid ? string.Format(" = {0:N4} {1}", apDif / baseDif, baseName) : "";
                            Log(string.Format("1 AP = {0:N4} DPS{1}", apDif, endStr));

                            double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * Player.BonusStrRatio(playerBase);
                            Log(string.Format("1 Str = {0:N4} DPS = {1:N4} {2}", strDif, strDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Crit"))
                        {
                            double critDps = SimsAvgDPS["Crit"];
                            double critDif = Math.Max(0, critDps - baseDps);

                            double agiDif = Player.AgiToAPRatio(playerBase) * baseDif + Player.AgiToCritRatio(playerBase.Class, playerBase.Level) * Player.BonusAgiRatio(playerBase) * 100 * critDif;
                            Log(string.Format("1 Agi = {0:N4} DPS = {1:N4} {2}", agiDif, agiDif / baseDif, baseName));

                            critDif /= (version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);

                            Log(string.Format("1{2} Crit = {0:N4} DPS = {1:N4} {3}", critDif, critDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Hit"))
                        {
                            double hitDps = SimsAvgDPS["Hit"];
                            double hitDif = Math.Max(0, (hitDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1));
                            Log(string.Format("1{2} Hit = {0:N4} DPS = {1:N4} {3}", hitDif, hitDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Haste"))
                        {
                            double hasteDps = SimsAvgDPS["Haste"];
                            double hasteDif = Math.Max(0, (hasteDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1));
                            Log(string.Format("1{2} Haste = {0:N4} DPS = {1:N4} {3}", hasteDif, hasteDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Expertise"))
                        {
                            double expDps = SimsAvgDPS["Expertise"];
                            double expDif = Math.Max(0, (expDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1));
                            Log(string.Format("1{2} Expertise = {0:N4} DPS = {1:N4} {3}", expDif, expDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+500 ArPen"))
                        {
                            double arpenDps = SimsAvgDPS["+500 ArPen"];
                            double arpenDif = Math.Max(0, (arpenDps - baseDps) / 500);
                            Log(string.Format("1 Armor Penetration = {0:N4} DPS = {1:N4} {2}", arpenDif, arpenDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1000 ArPen"))
                        {
                            double arpenDps = SimsAvgDPS["+1000 ArPen"];
                            double arpenDif = Math.Max(0, (arpenDps - baseDps) / 1000);
                            Log(string.Format("1 Armor Penetration (+1000) = {0:N4} DPS = {1:N4} {2}", arpenDif, arpenDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("DPS MH"))
                        {
                            double mhDps = SimsAvgDPS["DPS MH"];
                            double mhDif = Math.Max(0, (mhDps - baseDps) / 10);
                            Log(string.Format("1 MH DPS = {0:N4} DPS = {1:N4} {2}", mhDif, mhDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("DPS OH"))
                        {
                            double ohDps = SimsAvgDPS["DPS OH"];
                            double ohDif = Math.Max(0, (ohDps - baseDps) / 10);
                            Log(string.Format("1 OH DPS = {0:N4} DPS = {1:N4} {2}", ohDif, ohDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 MH Skill"))
                        {
                            double mhSkillDps = SimsAvgDPS["+1 MH Skill"];
                            double mhSkillDif = Math.Max(0, mhSkillDps - baseDps);
                            Log(string.Format("1 MH Skill = {0:N4} DPS = {1:N4} {2}", mhSkillDif, mhSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 OH Skill"))
                        {
                            double ohSkillDps = SimsAvgDPS["+1 OH Skill"];
                            double ohSkillDif = Math.Max(0, ohSkillDps - baseDps);
                            Log(string.Format("1 OH Skill = {0:N4} DPS = {1:N4} {2}", ohSkillDif, ohSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 MH Skill"))
                        {
                            double mhSkillDps = SimsAvgDPS["+5 MH Skill"];
                            double mhSkillDif = Math.Max(0, mhSkillDps - baseDps);
                            Log(string.Format("5 MH Skill = {0:N4} DPS = {1:N4} {2}", mhSkillDif, mhSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 OH Skill"))
                        {
                            double ohSkillDps = SimsAvgDPS["+5 OH Skill"];
                            double ohSkillDif = Math.Max(0, ohSkillDps - baseDps);
                            Log(string.Format("5 OH Skill = {0:N4} DPS = {1:N4} {2}", ohSkillDif, ohSkillDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Block Value"))
                        {
                            double val = SimsAvgDPS["Block Value"];
                            double dif = Math.Max(0, (val - baseDps) / 100);
                            Log(string.Format("1 Block Value = {0:N4} TPS = {1:N4} {2}", dif, dif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("SpellCrit"))
                        {
                            double critDps = SimsAvgDPS["SpellCrit"];
                            double critDif = Math.Max(0, (critDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1));
                            Log(string.Format("1{2} SpellCrit = {0:N4} DPS = {1:N4} {3}", critDif, critDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("SpellHit"))
                        {
                            double hitDps = SimsAvgDPS["SpellHit"];
                            double hitDif = Math.Max(0, (hitDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1));
                            Log(string.Format("1{2} SpellHit = {0:N4} DPS = {1:N4} {3}", hitDif, hitDif / baseDif, version == Version.TBC ? "" : "%", baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Int"))
                        {
                            double intDps = SimsAvgDPS["Int"];
                            double intDif = Math.Max(0, (intDps - baseDps) / (version == Version.TBC ? 100 : 50));
                            Log(string.Format("1 Int = {0:N4} DPS = {1:N4} {2}", intDif, intDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("Spi"))
                        {
                            double spiDps = SimsAvgDPS["Spi"];
                            double spiDif = Math.Max(0, (spiDps - baseDps) / (version == Version.TBC ? 100 : 50));
                            Log(string.Format("1 Spi = {0:N4} DPS = {1:N4} {2}", spiDif, spiDif / baseDif, baseName));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("MP5"))
                        {
                            double mp5Dps = SimsAvgDPS["MP5"];
                            double mp5Dif = Math.Max(0, (mp5Dps - baseDps) / 30);
                            Log(string.Format("1 MP5 = {0:N4} DPS = {1:N4} {2}", mp5Dif, mp5Dif / baseDif, baseName));

                            weightsDone += 1;
                        }
                    }
                }
                else if (nbSim >= 1)
                {
                    GUISetProgressText(String.Format("Generating Fight Stats..."));

                    double avgFightLength = CurrentData.AvgSimLength;
                    double avgDps = CurrentDpsList.Average();

                    double avgTps = 0;
                    if (jsonSim.Threat)
                    {
                        avgTps = CurrentTpsList.Average();
                        Log(string.Format("Average TPS : {0:N2} TPS (±{1:N2})", avgTps, SimsTPSStDev["Base"]));
                    }
                    else
                    {
                        Log("");
                    }
                    Log(string.Format("Average DPS : {0:N2} DPS (±{1:N2})", avgDps, SimsDPSStDev["Base"]));

                    int statsDone = 0;
                    int statsTotal = logListActions.Count + logListEffects.Count;

                    foreach (string ac in logListActions)
                    {
                        StatsData data = CurrentData.DataActions[ac];

                        if(data.AvgUses > 0)
                        {
                            double avgAcUse = data.AvgUses;
                            double avgAcDps = data.AvgDPS;
                            double avgAcDmg = data.AvgDmg;

                            double avgAcTps = 0;
                            double avgAcThreat = 0;
                            if (jsonSim.Threat)
                            {
                                avgAcTps = data.AvgTPS;
                                avgAcThreat = data.AvgThreat;
                            }

                            double dotmult = 1;

                            if (ac == "Cleave")
                            {
                                avgAcUse /= Math.Min(2, nbTargets);
                                avgAcDmg *= Math.Min(2, nbTargets);
                                avgAcThreat *= Math.Min(2, nbTargets);
                            }
                            else if (ac == "Whirlwind" && version == Version.TBC)
                            {
                                avgAcUse /= (playerBase.DualWielding ? 2 : 1) * Math.Min(4, nbTargets);
                                avgAcDmg *= (playerBase.DualWielding ? 2 : 1) * Math.Min(4, nbTargets);
                                avgAcThreat *= (playerBase.DualWielding ? 2 : 1) * Math.Min(4, nbTargets);
                            }
                            else if (ac == "Thunder Clap")
                            {
                                avgAcUse /= Math.Min(4, nbTargets);
                                avgAcDmg *= Math.Min(4, nbTargets);
                                avgAcThreat *= Math.Min(4, nbTargets);
                            }
                            else if (ac == "Shadow Cleave")
                            {
                                avgAcUse /= Math.Min(3, nbTargets);
                                avgAcDmg *= Math.Min(3, nbTargets);
                                avgAcThreat *= Math.Min(3, nbTargets);
                            }
                            else if (ac == "Curse of Agony")
                            {
                                StatsData data2 = CurrentData.DataEffects[ac];
                                dotmult = CurseOfAgonyDoT.NB_TICKS;
                                avgAcDps = data2.AvgDPS;
                                avgAcDmg = data2.AvgDmg;
                                if (jsonSim.Threat)
                                {
                                    avgAcTps = data2.AvgTPS;
                                    avgAcThreat = data2.AvgThreat;
                                }
                            }
                            else if (ac == "Corruption")
                            {
                                StatsData data2 = CurrentData.DataEffects[ac];
                                dotmult = CorruptionDoT.NB_TICKS(playerBase.Level);
                                avgAcDps = data2.AvgDPS;
                                avgAcDmg = data2.AvgDmg;
                                if (jsonSim.Threat)
                                {
                                    avgAcTps = data2.AvgTPS;
                                    avgAcThreat = data2.AvgThreat;
                                }
                            }
                            else if (ac == "Drain Life" && playerBase.Runes.Contains("Master Channeler"))
                            {
                                StatsData data2 = CurrentData.DataEffects[ac];
                                dotmult = DrainLifeDoT.NB_TICKS;
                                avgAcDps = data2.AvgDPS;
                                avgAcDmg = data2.AvgDmg;
                                if (jsonSim.Threat)
                                {
                                    avgAcTps = data2.AvgTPS;
                                    avgAcThreat = data2.AvgThreat;
                                }
                            }

                            string res = "\nAverage stats for action [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)", avgAcDps, avgAcDps / avgDps * 100);
                            if (jsonSim.Threat) res += string.Format(" / {0:N2} TPS ({1:N2}%)\n\tAverage of {2:N2} threat for {3:N2} uses (or 1 use every {4:N2}s)", avgAcTps, avgAcTps / avgTps * 100, avgAcThreat * dotmult, avgAcUse, avgFightLength / avgAcUse);
                            res += string.Format("\n\tAverage of {0:N2} damage for {1:N2} uses (or 1 use every {2:N2}s)", avgAcDmg * dotmult, avgAcUse, avgFightLength / avgAcUse);

                            double hitPct = data.AvgHit;
                            double critPct = data.AvgCrit;
                            res += string.Format("\n\t{0:N2}% Hit, {1:N2}% Crit", hitPct, critPct);
                            if ((data.Sample as Action).School != School.Physical)
                            {
                                double resistPct = data.AvgResist;
                                res += string.Format(", {0:N2}% Resist", resistPct);
                            }
                            else
                            {
                                double missPct = data.AvgMiss;
                                double glancePct = data.AvgGlance;
                                double dodgePct = data.AvgDodge;
                                res += string.Format(", {0:N2}% Miss, {1:N2}% Glancing, {2:N2}% Dodge", missPct, glancePct, dodgePct);
                                if (jsonSim.Facing)
                                {
                                    double parryPct = data.AvgParry;
                                    res += string.Format(", {0:N2}% Parry", parryPct);
                                }
                            }
                            Log(res);
                        }

                        statsDone++;
                    }
                    foreach (string ac in logListEffects)
                    {
                        StatsData data = CurrentData.DataEffects[ac];

                        if (data.AvgUses > 0)
                        {
                            double avgAcUse = data.AvgUses;
                            double avgAcDps = data.AvgDPS;
                            double avgAcDmg = data.AvgDmg;

                            double avgAcTps = 0;
                            double avgAcThreat = 0;
                            if (jsonSim.Threat)
                            {
                                avgAcTps = data.AvgTPS;
                                avgAcThreat = data.AvgThreat;
                            }

                            string res = "\nAverage stats for effect [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)", avgAcDps, avgAcDps / avgDps * 100);
                            if (jsonSim.Threat) res += string.Format(" / {0:N2} TPS ({1:N2}%)\n\tAverage of {2:N2} threat for {3:N2} ticks (or 1 tick every {4:N2}s)", avgAcTps, avgAcTps / avgTps * 100, avgAcThreat, avgAcUse, avgFightLength / avgAcUse);
                            res += string.Format("\n\tAverage of {0:N2} damage for {1:N2} ticks (or 1 tick every {2:N2}s)", avgAcDmg, avgAcUse, avgFightLength / avgAcUse);
                            double tickDelay = 3;
                            if (ac == "Mind Flay") tickDelay = MindFlay.TICK_DELAY;
                            if (ac == "Drain Life") tickDelay = DrainLifeDoT.TICK_DELAY;
                            if (ac == "Curse of Agony") tickDelay = CurseOfAgonyDoT.TICK_DELAY;
                            double uptime = avgAcUse * tickDelay / avgFightLength * 100;
                            res += string.Format("\n\t{0:N2}% Uptime", uptime);
                            Log(res);
                        }

                        statsDone++;
                    }
                }

                GUISetProgressText(String.Format("Writting logs.."));

                if (!debug)
                {
                    Directory.CreateDirectory(logsFileDir);
                }

                if(Display == DisplayMode.GUI)
                {
                    OutputClear();
                    Output(logs);
                }

                string path = debug ? Path.Combine(debugPath, logsFileName + txt) : 
                                    Path.Combine(logsFileDir, Config.Sim + " "  + Config.Player + DateTime.Now.ToString(" - yyyyMMdd_HHmmss_fff") + txt);
                File.WriteAllText(path, logs);

                Output("Logs written in " + path);

                GUISetProgressText(String.Format("Finished!"));
            }
            catch(Exception e)
            {
                Output("Simulation failed with the following error :\n" + e);
            }

            if (Display == DisplayMode.Console)
            {
                Output("Press any key to exit...");
                Console.ReadKey();
            }

            Running = false;
            GUI.Run_Enable();
        }

        public static void AddSimResult(SimResult result)
        {
            lock(CurrentData)
            {
                foreach (string s in logListActions)
                {
                    StatsData data;

                    if (!CurrentData.DataActions.ContainsKey(s))
                    {
                        data = new StatsData(s, result.Actions.Any(a => a.Action.ToString().Equals(s)) ? result.Actions.First(a => a.Action.ToString().Equals(s)).Action : null);
                        CurrentData.DataActions.Add(s, data);
                    }
                    else
                    {
                        data = CurrentData.DataActions[s];

                        if(data.Sample == null && result.Actions.Any(a => a.Action.ToString().Equals(s)))
                        {
                            data.Sample = result.Actions.First(a => a.Action.ToString().Equals(s)).Action;
                        }
                    }

                    double avgUses = result.Actions.Count(t => t.Action.ToString().Equals(s));
                    data.AvgUses = (CurrentData.NB * data.AvgUses + avgUses) / (CurrentData.NB + 1);

                    if (avgUses > 0)
                    {
                        double avgDmg = result.Actions.Where(t => t.Action.ToString().Equals(s)).Average(a => a.Result.Damage);
                        double avgDPS = avgDmg * avgUses / result.FightLength;
                        double avgHit = result.Actions.Count(t => t.Action.ToString().Equals(s) && t.Result.Type == ResultType.Hit) / avgUses * 100;
                        double avgCrit = result.Actions.Count(t => t.Action.ToString().Equals(s) && t.Result.Type == ResultType.Crit) / avgUses * 100;
                        double avgMiss = result.Actions.Count(t => t.Action.ToString().Equals(s) && t.Result.Type == ResultType.Miss) / avgUses * 100;
                        double avgGlance = result.Actions.Count(t => t.Action.ToString().Equals(s) && t.Result.Type == ResultType.Glance) / avgUses * 100;
                        double avgDodge = result.Actions.Count(t => t.Action.ToString().Equals(s) && t.Result.Type == ResultType.Dodge) / avgUses * 100;
                        double avgParry = result.Actions.Count(t => t.Action.ToString().Equals(s) && t.Result.Type == ResultType.Parry) / avgUses * 100;
                        double avgResist = result.Actions.Count(t => t.Action.ToString().Equals(s) && t.Result.Type == ResultType.Resist) / avgUses * 100;

                        data.AvgDmg = (CurrentData.NB * data.AvgDmg + avgDmg) / (CurrentData.NB + 1);
                        data.AvgDPS = (CurrentData.NB * data.AvgDPS + avgDPS) / (CurrentData.NB + 1);
                        data.AvgHit = (CurrentData.NB * data.AvgHit + avgHit) / (CurrentData.NB + 1);
                        data.AvgCrit = (CurrentData.NB * data.AvgCrit + avgCrit) / (CurrentData.NB + 1);
                        data.AvgMiss = (CurrentData.NB * data.AvgMiss + avgMiss) / (CurrentData.NB + 1);
                        data.AvgGlance = (CurrentData.NB * data.AvgGlance + avgGlance) / (CurrentData.NB + 1);
                        data.AvgDodge = (CurrentData.NB * data.AvgDodge + avgDodge) / (CurrentData.NB + 1);
                        data.AvgParry = (CurrentData.NB * data.AvgParry + avgParry) / (CurrentData.NB + 1);
                        data.AvgResist = (CurrentData.NB * data.AvgResist + avgResist) / (CurrentData.NB + 1);

                        if (jsonSim.Threat)
                        {
                            double avgThreat = result.Actions.Where(t => t.Action.ToString().Equals(s)).Average(a => a.Result.Threat);
                            double avgTPS = avgThreat * avgUses / result.FightLength;
                            data.AvgThreat = (CurrentData.NB * data.AvgThreat + avgThreat) / (CurrentData.NB + 1);
                            data.AvgTPS = (CurrentData.NB * data.AvgTPS + avgTPS) / (CurrentData.NB + 1);
                        }
                    }
                }

                foreach (string s in logListEffects)
                {
                    StatsData data;

                    if (!CurrentData.DataEffects.ContainsKey(s))
                    {
                        data = new StatsData(s, result.Effects.Any(a => a.Effect.ToString().Equals(s)) ? result.Effects.First(a => a.Effect.ToString().Equals(s)).Effect : null);
                        CurrentData.DataEffects.Add(s, data);
                    }
                    else
                    {
                        data = CurrentData.DataEffects[s];

                        if (data.Sample == null && result.Effects.Any(a => a.Effect.ToString().Equals(s)))
                        {
                            data.Sample = result.Effects.First(a => a.Effect.ToString().Equals(s)).Effect;
                        }
                    }

                    double avgUses = result.Effects.Count(t => t.Effect.ToString().Equals(s));
                    data.AvgUses = (CurrentData.NB * data.AvgUses + avgUses) / (CurrentData.NB + 1);

                    if (avgUses > 0)
                    {
                        double avgDmg = result.Effects.Where(t => t.Effect.ToString().Equals(s)).Average(a => a.Damage);
                        double avgDPS = avgDmg * avgUses / result.FightLength;

                        data.AvgDmg = (CurrentData.NB * data.AvgDmg + avgDmg) / (CurrentData.NB + 1);
                        data.AvgDPS = (CurrentData.NB * data.AvgDPS + avgDPS) / (CurrentData.NB + 1);

                        if (jsonSim.Threat)
                        {
                            double avgThreat = result.Effects.Where(t => t.Effect.ToString().Equals(s)).Average(a => a.Threat);
                            double avgTPS = avgThreat * avgUses / result.FightLength;

                            data.AvgThreat = (CurrentData.NB * data.AvgThreat + avgThreat) / (CurrentData.NB + 1);
                            data.AvgTPS = (CurrentData.NB * data.AvgTPS + avgTPS) / (CurrentData.NB + 1);
                        }
                    }
                }

                CurrentData.AvgSimLength = (CurrentData.NB * CurrentData.AvgSimLength + result.FightLength) / (CurrentData.NB + 1);

                CurrentData.NB++;
            }
        }

        public static void AddSimDps(double damage)
        {
            lock (CurrentDpsList)
            {
                CurrentDpsList.Add(damage);
            }
        }

        public static void AddSimThreat(double damage)
        {
            lock (CurrentTpsList)
            {
                CurrentTpsList.Add(damage);
            }
        }

        public static void Log(object log, bool newLine = true)
        {
            logs += log.ToString() + (newLine ? "\n" : "");
        }

        public static void Debug(object str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        public static void Output(object str, bool newLine = true, bool erase = false)
        {
            if(Display == DisplayMode.Console)
            {
                if (erase) Console.Clear();
                if (newLine) Console.WriteLine(str.ToString());
                else Console.Write(str.ToString());
            }
            else if(GUI != null)
            {
                if(erase) GUI.ConsoleTextSet(str.ToString());
                else GUI.ConsoleTextAdd(str.ToString(), newLine);
            }
        }

        public static void OutputClear()
        {
            if (Display == DisplayMode.Console)
            {
                Console.Clear();
            }
            else if(GUI != null)
            {
                GUI.ConsoleTextSet("");
            }
        }

        public static void GUISetProgress(double pct)
        {
            if(GUI != null)
            {
                GUI.SetProgress(pct);
            }
        }

        public static void GUISetProgressText(string str)
        {
            if (GUI != null)
            {
                GUI.SetProgressText(str);
            }
        }

        public static void GUISetProgressTime(TimeSpan time)
        {
            if (GUI != null)
            {
                GUI.SetProgressTime(time);
            }
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

            Simulation s = new Simulation(player, boss, jsonSim.FightLength, jsonSim.BossAutoLife, jsonSim.BossLowLifeTime, jsonSim.FightLengthMod, jsonSim.UnlimitedMana, jsonSim.UnlimitedResource, jsonSim.Tanking, jsonSim.TankHitEvery, jsonSim.TankHitRage, jsonSim.NbTargets, jsonSim.Threat);
            s.StartSim();
        }
    }
}

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

    public enum Version
    {
        Vanilla,
        TBC
    }

    public class Program
    {
        public static Version version = Version.TBC;

        public static string simJsonFileName = "sim.json";
        public static string playerJsonFileName = "player.json";
        public static string itemsJsonFileName = "items.json";
        public static string logsFileDir = "Logs";
        public static string logsFileName = "logs";
        public static string txt = ".txt";

        public static bool debug = true;
        public static string debugPath = @".\..\..";

        public static Player playerBase = null;
        public static Boss bossBase = null;

        public static int nbSim = 1000;
        public static double targetErrorPct = 0.5;
        public static bool targetError = true;
        public static bool threading = true;
        public static bool logFight = false;
        public static bool calcDpss = false;

        public static bool statsWeights = false;

        public static int nbTargets = 1;

        //public static List<Attribute> toWeight = null;
        //public static Attribute weighted;

        public static int nbTasksForSims = 1000;

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
            public Dictionary<string, StatsData> Data = new Dictionary<string, StatsData>();

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
                "+100 AP", "+100 SP",
                "+1% Hit","+1% Crit", "+1% Haste",
                "+10 DPS MH", "+10 DPS OH",
                "+1 MH Skill", "+1 OH Skill",
                "+5 MH Skill", "+5 OH Skill",
                "+1% SpellHit","+1% SpellCrit",
                "+50 Int", "+50 Spi", "+30 MP5",
                //"1","2","3","4","5","6","7","8","9",
            };

            simBonusAttribs = new Dictionary<string, Attributes>()
            {
                { "Base", new Attributes() },
                { "+100 AP", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.AP, version == Version.TBC ? 100 : 50 }
                        })},
                { "+100 SP", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.SP, version == Version.TBC ? 100 : 50 }
                        })},
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
                { "+1% Expertise", new Attributes(new Dictionary<Attribute, double>()
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
                { "+10 DPS MH", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.WeaponDamageMH, 10 }
                        })},
                { "+10 DPS OH", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.WeaponDamageOH, 10 }
                        })},
                { "+1 MH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "+1 OH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "+5 MH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "+5 OH Skill", new Attributes(new Dictionary<Attribute, double>()) },
                { "+1% SpellHit", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.SpellHitChance, 0.01 }
                        })},
                { "+1% SpellCrit", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.SpellCritChance, 0.01 }
                        })},
                { "+50 Int", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.Intellect, version == Version.TBC ? 100 : 50 }
                        })},
                { "+50 Spi", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.Spirit, version == Version.TBC ? 100 : 50 }
                        })},
                { "+30 MP5", new Attributes(new Dictionary<Attribute, double>()
                        {
                            { Attribute.MP5, (version == Version.TBC ? 60 : 30) }
                        })},
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
        }

        public static void LoadConfigJsons(bool player = true, bool sim = true)
        {
            try
            {
                if(sim)
                {
                    string simString = File.ReadAllText(debug ? Path.Combine(debugPath, simJsonFileName) : simJsonFileName);
                    jsonSim = JsonConvert.DeserializeObject<JsonUtil.JsonSim>(simString);
                }
                if(player)
                {
                    string playerString = File.ReadAllText(debug ? Path.Combine(debugPath, playerJsonFileName) : playerJsonFileName);
                    jsonPlayer = JsonConvert.DeserializeObject<JsonUtil.JsonPlayer>(playerString);
                }
            }
            catch(Exception e)
            {
                Output("Json loading failed :\n" + e);
            }
        }

        public static void SaveJsons(bool player = true, bool sim = true)
        {
            try
            {
                if(sim) File.WriteAllText(debug ? Path.Combine(debugPath, simJsonFileName) : simJsonFileName, JsonConvert.SerializeObject(jsonSim, Formatting.Indented));
                if(player) File.WriteAllText(debug ? Path.Combine(debugPath, playerJsonFileName) : playerJsonFileName, JsonConvert.SerializeObject(jsonPlayer, Formatting.Indented));
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
                
                playerBase = JsonUtil.JsonPlayer.ToPlayer(jsonPlayer, jsonSim.Tanking);

                if (playerBase.Level > 60) version = Version.TBC;
                else version = Version.Vanilla;

                if(playerBase.MH == null)
                {
                    playerBase.MH = new Weapon();
                }

                if (playerBase.Class == Player.Classes.Rogue || playerBase.Class == Player.Classes.Warrior || jsonSim.Tanking)
                {
                    simOrder.Remove("+100 SP");
                    simOrder.Remove("+50 Int");
                    simOrder.Remove("+50 Spi");
                    simOrder.Remove("+30 MP5");
                }
                if(playerBase.OH == null || !playerBase.DualWielding)
                {
                    simOrder.Remove("+10 DPS OH");
                    simOrder.Remove("+1 OH Skill");
                    simOrder.Remove("+5 OH Skill");
                }
                else if(playerBase.MH.Type == playerBase.OH.Type)
                {
                    simOrder.Remove("+1 OH Skill");
                    simOrder.Remove("+5 OH Skill");
                }
                if(playerBase.Class == Player.Classes.Druid
                    || playerBase.Class == Player.Classes.Warlock
                    || playerBase.Class == Player.Classes.Mage
                    || playerBase.Class == Player.Classes.Priest
                    || playerBase.MH == null)
                {
                    simOrder.Remove("+10 DPS MH");
                    simOrder.Remove("+1 MH Skill");
                    simOrder.Remove("+5 MH Skill");
                }

                if (playerBase.Class == Player.Classes.Warlock
                    || playerBase.Class == Player.Classes.Mage
                    || playerBase.Class == Player.Classes.Priest)
                {
                    simOrder.Remove("+100 AP");
                    simOrder.Remove("+1% Hit");
                    simOrder.Remove("+1% Crit");
                    simOrder.Remove("+1% Haste");
                }
                else
                {
                    simOrder.Remove("+100 SP");
                    simOrder.Remove("+1% SpellHit");
                    simOrder.Remove("+1% SpellCrit");
                }

                if(version == Version.TBC)
                {
                    simOrder.Remove("+1 MH Skill");
                    simOrder.Remove("+5 MH Skill");
                    simOrder.Remove("+1 OH Skill");
                    simOrder.Remove("+5 OH Skill");

                    if(playerBase.Class == Player.Classes.Rogue || playerBase.Class == Player.Classes.Warrior
                        || playerBase.Class == Player.Classes.Druid || playerBase.Class == Player.Classes.Shaman
                        || playerBase.Class == Player.Classes.Paladin || playerBase.Class == Player.Classes.Hunter)
                    {
                        simOrder.Insert(5, "+1% Expertise");
                        simOrder.Insert(6, "+500 ArPen");
                        //simOrder.Insert(7, "+1000 ArPen");
                    }
                }

                if (simOrder.Contains("+10 DPS MH"))
                {
                    double dmg = simBonusAttribs["+10 DPS MH"].GetValue(Attribute.WeaponDamageMH) * playerBase.MH.Speed;
                    simBonusAttribs["+10 DPS MH"].SetValue(Attribute.WeaponDamageMH, dmg);
                }
                if (simOrder.Contains("+10 DPS OH"))
                {
                    double dmg = simBonusAttribs["+10 DPS OH"].GetValue(Attribute.WeaponDamageOH) * playerBase.OH.Speed;
                    simBonusAttribs["+10 DPS OH"].SetValue(Attribute.WeaponDamageOH, dmg);
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

                /*
                simOrder.Remove("+500 ArPen");
                simOrder.Remove("+1000 ArPen");
                simOrder.Remove("+1% Expertise");
                simOrder.Remove("+1% Hit");
                simOrder.Remove("+1% Crit");
                simOrder.Remove("+1% Haste");
                simOrder.Remove("+10 DPS MH");
                simOrder.Remove("+10 DPS OH");
                */

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
                        foreach(string k in jsonSim.Boss.SchoolResists.Keys)
                        {
                            jsonSim.Boss.SchoolResists[k] = Math.Max(0, jsonSim.Boss.SchoolResists[k] - reduction);
                        }
                    }
                }
                bossBase = JsonUtil.JsonBoss.ToBoss(jsonSim.Boss);
                int bossBaseArmor = bossBase.Armor;

                Log("\nBoss :");
                Log(bossBase.ToString(0, jsonSim.Boss.Armor));  // TODO : base magic armor
                Log("After raid debuffs and player penetration :");
                Log(bossBase.ToString(playerBase.Attributes.GetValue(Attribute.ArmorPen)) + "\n");

                if(!statsWeights)
                {
                    //logListActions = totalActions.SelectMany(a => a.Select(t => t.Action.ToString()).OrderBy(b => b)).Distinct().ToList();
                    logListActions = new List<string>() { "AA MH", "AA OH", "AA Ranged", "AA Wand" };
                    if (playerBase.Class == Player.Classes.Warrior)
                        logListActions.AddRange(new List<string>() { "Slam", "Bloodthirst", "Mortal Strike", "Sunder Armor", "Revenge", "Whirlwind", "Sweeping Strikes", "Cleave", "Heroic Strike", "Execute", "Hamstring", "Battle Shout" });
                    else if (playerBase.Class == Player.Classes.Druid)
                        logListActions.AddRange(new List<string>() { "Shred", "Ferocious Bite", "Shift", "Maul", "Swipe" });
                    else if (playerBase.Class == Player.Classes.Priest)
                        logListActions.AddRange(new List<string>() { "Mind Blast", "Mind Flay", "SW:P", "Devouring Plague" });
                    else if (playerBase.Class == Player.Classes.Rogue)
                        logListActions.AddRange(new List<string>() { "Sinister Strike", "Backstab", "Eviscerate", "Ambush", "Instant Poison" });
                    else if (playerBase.Class == Player.Classes.Warlock)
                        logListActions.AddRange(new List<string>() { "Shadow Bolt" });

                    logListActions.AddRange(new List<string>() { "Thunderfury", "Deathbringer", "Vis'kag the Bloodletter", "Perdition's Blade", "Romulo's Poison Vial" });

                    //logListEffects = totalEffects.SelectMany(a => a.Select(t => t.Effect.ToString()).OrderBy(b => b)).Distinct().ToList();
                    logListEffects = new List<string>() { };
                    if (playerBase.Class == Player.Classes.Priest)
                        logListEffects.AddRange(new List<string>() { "Mind Flay", "SW:P", "Devouring Plague" });
                    if (playerBase.Class == Player.Classes.Warrior)
                        logListEffects.AddRange(new List<string>() { "Deep Wounds" });
                    if (playerBase.Class == Player.Classes.Warlock)
                        logListEffects.AddRange(new List<string>() { "Corruption", "Malediction of Agony" });

                    CurrentData = new SimData();
                }

                DateTime start = DateTime.Now;

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
                            GUISetProgressText(txt);
                            GUISetProgress((i + 1) / nbSim * 100);
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
                                double pct = (double)CurrentDpsList.Count / nbSim * 100;
                                GUISetProgress(pct);
                                GUISetProgressText(String.Format("Simulating {0} - {1}/{2}", simOrder[done], done + 1, statsWeights ? simOrder.Count : 1));
                                string outputText = "";

                                if (!logFight)
                                {
                                    outputText += String.Format("Simulating {0}...", simOrder[done]);
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

                                Thread.Sleep(TimeSpan.FromSeconds(Display == DisplayMode.Console ? 1.0/2 : 1.0/60));
                            }

                            if (!logFight)
                            {
                                OutputClear();
                                Output(String.Format("{0:N2}% ({1}/{2})", (double)CurrentDpsList.Count / nbSim * 100, CurrentDpsList.Count, nbSim));
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
                                    if (CurrentDpsList.Count > 10)
                                    {
                                        errorPct = Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average());
                                    }
                                }

                                double currentPct = Math.Min(1, Math.Pow((100 - errorPct) / (100 - targetErrorPct), 0.2/targetErrorPct*1000)) * 100;
                                GUISetProgress(done / simOrder.Count * 100 + currentPct);
                                GUISetProgressText(String.Format("Simulating {0} - {1}/{2}", simOrder[done], done + 1, statsWeights ? simOrder.Count : 1));
                                
                                string outputText = "";
                                outputText += String.Format("Simulating {0}, aiming for minimum ±{1:N2}% precision...", simOrder[done], targetErrorPct);
                                outputText += String.Format("\nSims done : {0:N0}", CurrentDpsList.Count);
                                outputText += String.Format("\nSims running : {0:N0}", tasks.Count(t => !t.IsCompleted));
                                outputText += String.Format("\nCurrent precision : ±{0:N2}%", errorPct);
                                Output(outputText, true, true);

                                Thread.Sleep(TimeSpan.FromSeconds(Display == DisplayMode.Console ? 1.0 / 2 : 1.0 / 60));
                            }
                            
                            Output("Waiting for remaining simulations to complete...");
                            
                            Task.WaitAll(tasks.ToArray());
                        }
                    }

                    ErrorList.Add(Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average()));
                    SimsAvgDPS.Add(simOrder[done], CurrentDpsList.Average());
                    SimsDPSStDev.Add(simOrder[done], Stats.StandardError(Stats.StdDev(CurrentDpsList.ToArray())));
                    if(jsonSim.Tanking)
                    {
                        SimsAvgTPS.Add(simOrder[done], CurrentTpsList.Average());
                        SimsTPSStDev.Add(simOrder[done], Stats.StandardError(Stats.StdDev(CurrentTpsList.ToArray())));
                    }

                    //Log(simOrder[done] + " : " + CurrentDpsList.Average());
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

                string endMsg2 = string.Format("Overall accuracy of results : ±{0:N2}% (±{1:N2} DPS)", ErrorList.Average(), ErrorList[0] / 100 * SimsAvgDPS["Base"]);
                if(jsonSim.Tanking) endMsg2 += string.Format(" (±{1:N2} TPS)", ErrorList.Average(), ErrorList[0] / 100 * SimsAvgTPS["Base"]);
                Output(endMsg2);
                Log(endMsg2);

                string endMsg3 = string.Format("\nGenerating results...");
                Output(endMsg3);

                if (statsWeights)
                {
                    GUISetProgressText(String.Format("Generating Stats Weights..."));

                    double weightsDone = 0;
                    double apDif = 0;
                    
                    // TPS
                    if (jsonSim.Tanking)
                    {
                        double baseTps = SimsAvgTPS["Base"];
                        Log(string.Format("\nBase : {0:N2} TPS (±{1:N2})", baseTps, SimsTPSStDev["Base"]));

                        Log("\nWeights by TPS :");
                        
                        if (simOrder.Contains("+100 AP"))
                        {
                            double apTps = SimsAvgTPS["+100 AP"];
                            apDif = (apTps - baseTps) / (version == Version.TBC ? 100 : 50);
                            if (apDif < 0) apDif = 0;
                            Log(string.Format("1 AP = {0:N4} TPS", apDif));

                            double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * Player.BonusStrToAPRatio(playerBase);
                            Log(string.Format("1 Str = {0:N4} TPS = {1:N4} AP", strDif, strDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Crit"))
                        {
                            double critTps = SimsAvgTPS["+1% Crit"];
                            double critDif = critTps - baseTps;
                            if (critDif < 0) critDif = 0;

                            double agiDif = Player.AgiToAPRatio(playerBase) * apDif + Player.AgiToCritRatio(playerBase.Class) * Player.BonusAgiToCritRatio(playerBase) * 100 * critDif;
                            Log(string.Format("1 Agi = {0:N4} TPS = {1:N4} AP", agiDif, agiDif / apDif));

                            Log(string.Format("1{2} Crit = {0:N4} TPS = {1:N4} AP", critDif, critDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Hit"))
                        {
                            double hitTps = SimsAvgTPS["+1% Hit"];
                            double hitDif = hitTps - baseTps;
                            if (hitDif < 0) hitDif = 0;
                            Log(string.Format("1{2} Hit = {0:N4} TPS = {1:N4} AP", hitDif, hitDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Haste"))
                        {
                            double hasteTps = SimsAvgTPS["+1% Haste"];
                            double hasteDif = hasteTps - baseTps;
                            if (hasteDif < 0) hasteDif = 0;
                            Log(string.Format("1{2} Haste = {0:N4} TPS = {1:N4} AP", hasteDif, hasteDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Expertise"))
                        {
                            double expTps = SimsAvgTPS["+1% Expertise"];
                            double expDif = expTps - baseTps;
                            if (expDif < 0) expDif = 0;
                            Log(string.Format("1{2} Expertise = {0:N4} TPS = {1:N4} AP", expDif, expDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+500 ArPen"))
                        {
                            double arpenTps = SimsAvgDPS["+500 ArPen"];
                            double arpenDif = (arpenTps - baseTps) / 500;
                            if (arpenDif < 0) arpenDif = 0;
                            Log(string.Format("1 Armor Pen = {0:N4} TPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1000 ArPen"))
                        {
                            double arpenTps = SimsAvgDPS["+1000 ArPen"];
                            double arpenDif = (arpenTps - baseTps) / 1000;
                            if (arpenDif < 0) arpenDif = 0;
                            Log(string.Format("1 Armor Pen (+1000) = {0:N4} TPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+10 DPS MH"))
                        {
                            double mhTps = SimsAvgTPS["+10 DPS MH"];
                            double mhDif = (mhTps - baseTps) / 10;
                            if (mhDif < 0) mhDif = 0;
                            Log(string.Format("1 MH DPS = {0:N4} TPS = {1:N4} AP", mhDif, mhDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+10 DPS OH"))
                        {
                            double ohTps = SimsAvgTPS["+10 DPS OH"];
                            double ohDif = (ohTps - baseTps) / 10;
                            if (ohDif < 0) ohDif = 0;
                            Log(string.Format("1 OH DPS = {0:N4} TPS = {1:N4} AP", ohDif, ohDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 MH Skill"))
                        {
                            double mhSkillTps = SimsAvgTPS["+1 MH Skill"];
                            double mhSkillDif = mhSkillTps - baseTps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("1 MH Skill = {0:N4} TPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 OH Skill"))
                        {
                            double ohSkillTps = SimsAvgTPS["+1 OH Skill"];
                            double ohSkillDif = ohSkillTps - baseTps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("1 OH Skill = {0:N4} TPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 MH Skill"))
                        {
                            double mhSkillTps = SimsAvgTPS["+5 MH Skill"];
                            double mhSkillDif = mhSkillTps - baseTps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("5 MH Skill = {0:N4} TPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 OH Skill"))
                        {
                            double ohSkillTps = SimsAvgTPS["+5 OH Skill"];
                            double ohSkillDif = ohSkillTps - baseTps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("5 OH Skill = {0:N4} TPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+50 Int"))
                        {
                            double intTps = SimsAvgTPS["+50 Int"];
                            double intDif = (intTps - baseTps) / (version == Version.TBC ? 100 : 50);
                            if (intDif < 0) intDif = 0;
                            Log(string.Format("1 Int = {0:N4} TPS = {1:N4} AP", intDif, intDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+50 Spi"))
                        {
                            double spiTps = SimsAvgTPS["+50 Spi"];
                            double spiDif = (spiTps - baseTps) / (version == Version.TBC ? 100 : 50);
                            if (spiDif < 0) spiDif = 0;
                            Log(string.Format("1 Spi = {0:N4} TPS = {1:N4} AP", spiDif, spiDif / apDif));

                            weightsDone += 1;
                        }
                    }

                    // DPS
                    if (true)
                    {
                        double baseDps = SimsAvgDPS["Base"];
                        Log(string.Format("\nBase : {0:N2} DPS (±{1:N2})", baseDps, SimsDPSStDev["Base"]));

                        Log("\nWeights by DPS :");
                        if (simOrder.Contains("+100 AP"))
                        {
                            double apDps = SimsAvgDPS["+100 AP"];
                            apDif = (apDps - baseDps) / (version == Version.TBC ? 100 : 50);
                            if (apDif < 0) apDif = 0;
                            Log(string.Format("1 AP = {0:N4} DPS", apDif));

                            double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * Player.BonusStrToAPRatio(playerBase);
                            Log(string.Format("1 Str = {0:N4} DPS = {1:N4} AP", strDif, strDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+100 SP"))
                        {
                            double apDps = SimsAvgDPS["+100 SP"];
                            apDif = (apDps - baseDps) / (version == Version.TBC ? 100 : 50);
                            if (apDif < 0) apDif = 0;
                            Log(string.Format("1 SP = {0:N4} DPS", apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Crit"))
                        {
                            double critDps = SimsAvgDPS["+1% Crit"];
                            double critDif = critDps - baseDps;
                            if (critDif < 0) critDif = 0;

                            double agiDif = Player.AgiToAPRatio(playerBase) * apDif + Player.AgiToCritRatio(playerBase.Class) * Player.BonusAgiToCritRatio(playerBase) * 100 * critDif;
                            Log(string.Format("1 Agi = {0:N4} DPS = {1:N4} AP", agiDif, agiDif / apDif));

                            critDif /= (version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);

                            Log(string.Format("1{2} Crit = {0:N4} DPS = {1:N4} AP", critDif, critDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Hit"))
                        {
                            double hitDps = SimsAvgDPS["+1% Hit"];
                            double hitDif = (hitDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1);
                            if (hitDif < 0) hitDif = 0;
                            Log(string.Format("1{2} Hit = {0:N4} DPS = {1:N4} AP", hitDif, hitDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Haste"))
                        {
                            double hasteDps = SimsAvgDPS["+1% Haste"];
                            double hasteDif = (hasteDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1);
                            if (hasteDif < 0) hasteDif = 0;
                            Log(string.Format("1{2} Haste = {0:N4} DPS = {1:N4} AP", hasteDif, hasteDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Expertise"))
                        {
                            double expDps = SimsAvgDPS["+1% Expertise"];
                            double expDif = (expDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1);
                            if (expDif < 0) expDif = 0;
                            Log(string.Format("1{2} Expertise = {0:N4} DPS = {1:N4} AP", expDif, expDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+500 ArPen"))
                        {
                            double arpenDps = SimsAvgDPS["+500 ArPen"];
                            double arpenDif = (arpenDps - baseDps) / 500;
                            if (arpenDif < 0) arpenDif = 0;
                            Log(string.Format("1 Armor Penetration = {0:N4} DPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1000 ArPen"))
                        {
                            double arpenDps = SimsAvgDPS["+1000 ArPen"];
                            double arpenDif = (arpenDps - baseDps) / 1000;
                            if (arpenDif < 0) arpenDif = 0;
                            Log(string.Format("1 Armor Penetration (+1000) = {0:N4} DPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+10 DPS MH"))
                        {
                            double mhDps = SimsAvgDPS["+10 DPS MH"];
                            double mhDif = (mhDps - baseDps) / 10;
                            if (mhDif < 0) mhDif = 0;
                            Log(string.Format("1 MH DPS = {0:N4} DPS = {1:N4} AP", mhDif, mhDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+10 DPS OH"))
                        {
                            double ohDps = SimsAvgDPS["+10 DPS OH"];
                            double ohDif = (ohDps - baseDps) / 10;
                            if (ohDif < 0) ohDif = 0;
                            Log(string.Format("1 OH DPS = {0:N4} DPS = {1:N4} AP", ohDif, ohDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 MH Skill"))
                        {
                            double mhSkillDps = SimsAvgDPS["+1 MH Skill"];
                            double mhSkillDif = mhSkillDps - baseDps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("1 MH Skill = {0:N4} DPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 OH Skill"))
                        {
                            double ohSkillDps = SimsAvgDPS["+1 OH Skill"];
                            double ohSkillDif = ohSkillDps - baseDps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("1 OH Skill = {0:N4} DPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 MH Skill"))
                        {
                            double mhSkillDps = SimsAvgDPS["+5 MH Skill"];
                            double mhSkillDif = mhSkillDps - baseDps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("5 MH Skill = {0:N4} DPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 OH Skill"))
                        {
                            double ohSkillDps = SimsAvgDPS["+5 OH Skill"];
                            double ohSkillDif = ohSkillDps - baseDps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("5 OH Skill = {0:N4} DPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% SpellCrit"))
                        {
                            double critDps = SimsAvgDPS["+1% SpellCrit"];
                            double critDif = (critDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1);
                            if (critDif < 0) critDif = 0;
                            Log(string.Format("1% SpellCrit = {0:N4} DPS = {1:N4} SP", critDif, critDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% SpellHit"))
                        {
                            double hitDps = SimsAvgDPS["+1% SpellHit"];
                            double hitDif = (hitDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1);
                            if (hitDif < 0) hitDif = 0;
                            Log(string.Format("1{2} SpellHit = {0:N4} DPS = {1:N4} SP", hitDif, hitDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+50 Int"))
                        {
                            double intDps = SimsAvgDPS["+50 Int"];
                            double intDif = (intDps - baseDps) / (version == Version.TBC ? 100 : 50);
                            if (intDif < 0) intDif = 0;
                            string comp = simOrder.Contains("+100 AP") ? "AP" : "SP";
                            Log(string.Format("1 Int = {0:N4} DPS = {1:N4} {2}", intDif, intDif / apDif, comp));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+50 Spi"))
                        {
                            double spiDps = SimsAvgDPS["+50 Spi"];
                            double spiDif = (spiDps - baseDps) / (version == Version.TBC ? 100 : 50);
                            if (spiDif < 0) spiDif = 0;
                            string comp = simOrder.Contains("+100 AP") ? "AP" : "SP";
                            Log(string.Format("1 Spi = {0:N4} DPS = {1:N4} {2}", spiDif, spiDif / apDif, comp));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+30 MP5"))
                        {
                            double mp5Dps = SimsAvgDPS["+30 MP5"];
                            double mp5Dif = (mp5Dps - baseDps) / 30;
                            if (mp5Dif < 0) mp5Dif = 0;
                            string comp = simOrder.Contains("+100 AP") ? "AP" : "SP";
                            Log(string.Format("1 MP5 = {0:N4} DPS = {1:N4} {2}", mp5Dif, mp5Dif / apDif, comp));

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
                    if (jsonSim.Tanking)
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
                        StatsData data = CurrentData.Data[ac];

                        if(data.AvgUses > 0)
                        {
                            double avgAcUse = data.AvgUses;
                            double avgAcDps = data.AvgDPS;
                            double avgAcDmg = data.AvgDmg;
                            string res = "\nAverage stats for [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)", avgAcDps, avgAcDps / avgDps * 100);

                            if (jsonSim.Tanking)
                            {
                                double avgAcTps = data.AvgTPS;
                                double avgAcThreat = data.AvgTPS;
                                res += string.Format(" / {0:N2} TPS ({1:N2}%)\n\tAverage of {2:N2} threat for {3:N2} uses (or 1 use every {4:N2}s)", avgAcTps, avgAcTps / avgTps * 100, avgAcThreat, avgAcUse, jsonSim.FightLength / avgAcUse);
                            }
                            if (ac == "Cleave")
                            {
                                avgAcUse /= Math.Min(2, nbTargets);
                                avgAcDmg *= Math.Min(2, nbTargets);
                            }
                            else if (ac == "Whirlwind" && version == Version.TBC)
                            {
                                avgAcUse /= (playerBase.DualWielding ? 2 : 1) * nbTargets;
                                avgAcDmg *= (playerBase.DualWielding ? 2 : 1) * nbTargets;
                            }
                            res += string.Format("\n\tAverage of {0:N2} damage for {1:N2} uses (or 1 use every {2:N2}s)", avgAcDmg, avgAcUse, avgFightLength / avgAcUse);

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
                                if (jsonSim.Tanking)
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
                        StatsData data = CurrentData.Data[ac];

                        if (data.AvgUses > 0)
                        {
                            double avgAcUse = data.AvgUses;
                            double avgAcDps = data.AvgDPS;
                            double avgAcDmg = data.AvgDmg;
                            string res = "\nAverage stats for [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)\n\tAverage of {2:N2} damage for {3:N2} ticks (or 1 tick every {4:N2}s)", avgAcDps, avgAcDps / avgDps * 100, avgAcDmg, avgAcUse, jsonSim.FightLength / avgAcUse);
                            double tickDelay = 3;
                            if (ac == "Mind Flay") tickDelay = 1;
                            double uptime = tickDelay / (avgFightLength / avgAcUse) * 100;
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

                string path = debug ? Path.Combine(debugPath, logsFileName + txt) : Path.Combine(logsFileDir, logsFileName + DateTime.Now.ToString("_yyyyMMdd-HHmmss-fff") + txt);
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

                    if (!CurrentData.Data.ContainsKey(s))
                    {
                        data = new StatsData(s, result.Actions.Any(a => a.Action.ToString().Equals(s)) ? result.Actions.First(a => a.Action.ToString().Equals(s)).Action : null);
                        CurrentData.Data.Add(s, data);
                    }
                    else
                    {
                        data = CurrentData.Data[s];

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
                    }
                }

                foreach (string s in logListEffects)
                {
                    StatsData data;

                    if (!CurrentData.Data.ContainsKey(s))
                    {
                        data = new StatsData(s, result.Effects.Any(a => a.Effect.ToString().Equals(s)) ? result.Effects.First(a => a.Effect.ToString().Equals(s)).Effect : null);
                        CurrentData.Data.Add(s, data);
                    }
                    else
                    {
                        data = CurrentData.Data[s];

                        if (data.Sample == null && result.Effects.Any(a => a.Effect.ToString().Equals(s)))
                        {
                            data.Sample = result.Effects.First(a => a.Effect.ToString().Equals(s)).Effect;
                        }
                    }

                    double avgUses = result.Effects.Count(t => t.Effect.ToString().Equals(s));
                    data.AvgUses = (CurrentData.NB * data.AvgUses + avgUses) / (CurrentData.NB + 1);

                    if(avgUses > 0)
                    {
                        double avgDmg = result.Effects.Where(t => t.Effect.ToString().Equals(s)).Average(a => a.Damage);
                        double avgDPS = avgDmg * avgUses / result.FightLength;

                        data.AvgDmg = (CurrentData.NB * data.AvgDmg + avgDmg) / (CurrentData.NB + 1);
                        data.AvgDPS = (CurrentData.NB * data.AvgDPS + avgDPS) / (CurrentData.NB + 1);
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

            Simulation s = new Simulation(player, boss, jsonSim.FightLength, jsonSim.BossAutoLife, jsonSim.BossLowLifeTime, jsonSim.FightLengthMod, jsonSim.UnlimitedMana, jsonSim.UnlimitedResource, jsonSim.Tanking, jsonSim.TankHitEvery, jsonSim.TankHitRage, jsonSim.NbTargets);
            s.StartSim();
        }
    }
}

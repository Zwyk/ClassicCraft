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

        //public static List<Attribute> toWeight = null;
        //public static Attribute weighted;

        public static int nbTasksForSims = 1000;

        private static List<SimResult> CurrentResults;
        private static List<double> CurrentDpsList;
        private static List<double> CurrentTpsList;

        private static Dictionary<string, List<SimResult>> ResultsList;
        private static Dictionary<string, List<double>> DamagesList;
        private static Dictionary<string, List<double>> ThreatsList;
        private static List<double> ErrorList;

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
            Run(null, debugPath);
        }

        public static string basePath()
        {
            return debug ? debugPath : "";
        }

        public static void Reset()
        {

            ResultsList = new Dictionary<string, List<SimResult>>();
            DamagesList = new Dictionary<string, List<double>>();
            ThreatsList = new Dictionary<string, List<double>>();
            ErrorList = new List<double>();

            logs = "";

            simOrder = new List<string>(){
                "Base",
                "+50 AP", "+50 SP",
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
                        { "+50 AP", new Attributes(new Dictionary<Attribute, double>()
                                {
                                    { Attribute.AP, version == Version.TBC ? 100 : 50 }
                                })},
                        { "+50 SP", new Attributes(new Dictionary<Attribute, double>()
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
                        { "+100 ArPen", new Attributes(new Dictionary<Attribute, double>()
                                {
                                    { Attribute.ArmorPen, 100 }
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

        public static void LoadConfigJsons()
        {
            try
            {
                string simString = File.ReadAllText(debug ? Path.Combine(debugPath, simJsonFileName) : simJsonFileName);
                jsonSim = JsonConvert.DeserializeObject<JsonUtil.JsonSim>(simString);
                string playerString = File.ReadAllText(debug ? Path.Combine(debugPath, playerJsonFileName) : playerJsonFileName);
                jsonPlayer = JsonConvert.DeserializeObject<JsonUtil.JsonPlayer>(playerString);
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
                
                Log(string.Format("Date : {0}\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                Log(string.Format("Fight length : {0} seconds (±{1}%)", jsonSim.FightLength, jsonSim.FightLengthMod * 100));

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
                
                playerBase = JsonUtil.JsonPlayer.ToPlayer(jsonPlayer, jsonSim.Tanking);

                if(playerBase.MH == null)
                {
                    playerBase.MH = new Weapon();
                }

                if (playerBase.Class == Player.Classes.Rogue || playerBase.Class == Player.Classes.Warrior || jsonSim.Tanking)
                {
                    simOrder.Remove("+50 SP");
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
                    simOrder.Remove("+50 AP");
                    simOrder.Remove("+1% Hit");
                    simOrder.Remove("+1% Crit");
                    simOrder.Remove("+1% Haste");
                }
                else
                {
                    simOrder.Remove("+50 SP");
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
                        simOrder.Insert(6, "+100 ArPen");
                        simOrder.Insert(7, "+1000 ArPen");
                    }
                }

                if (simOrder.Contains("+10 DPS MH"))
                {
                    simBonusAttribs["+10 DPS MH"].SetValue(Attribute.WeaponDamageMH, simBonusAttribs["+10 DPS MH"].GetValue(Attribute.WeaponDamageMH) * playerBase.MH.Speed);
                }
                if (simOrder.Contains("+10 DPS OH"))
                {
                    simBonusAttribs["+10 DPS OH"].SetValue(Attribute.WeaponDamageOH, simBonusAttribs["+10 DPS OH"].GetValue(Attribute.WeaponDamageOH) * playerBase.OH.Speed);
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
                bossBase = JsonUtil.JsonBoss.ToBoss(jsonSim.Boss, playerBase.Attributes.GetValue(Attribute.ArmorPen));
                int bossBaseArmor = bossBase.Armor;

                Log("\nBoss (after raid debuffs) :");
                Log(bossBase.ToString());

                DateTime start = DateTime.Now;

                List<Task> tasks = new List<Task>();

                Enchantment we = new Enchantment(0, "Weights", new Attributes(new Dictionary<Attribute, double>()));
                playerBase.Buffs.Add(we);

                // Doing simulations
                for (int done = 0; done < (statsWeights ? simOrder.Count : 1); done++)
                {
                    CurrentDpsList = new List<double>();
                    CurrentTpsList = new List<double>();
                    CurrentResults = new List<SimResult>();
                    
                    if (done > 0)
                    {
                        we.Attributes = simBonusAttribs[simOrder[done]];
                        playerBase.CalculateAttributes();

                        bossBase.Armor = bossBaseArmor - (int)we.Attributes.GetValue(Attribute.ArmorPen);
                        
                        //Log(simOrder[done] + "\n\t" + playerBase.ToString() + "\n\t" + bossBase.ToString());
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
                                GUISetProgressText(String.Format("Simulations done : {0}/{1}", CurrentDpsList.Count, nbSim));

                                if (!logFight)
                                {
                                    OutputClear();
                                    Output(String.Format("{0:N2}% ({1}/{2})", pct, CurrentDpsList.Count, nbSim));
                                }

                                if (CurrentDpsList.Count > 0)
                                {
                                    Output(String.Format("Precision : ±{0:N2}%", Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average())));
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
                                    if (CurrentDpsList.Count > 0)
                                    {
                                        errorPct = Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average());
                                    }
                                }

                                double currentPct = Math.Min(1, Math.Pow((100 - errorPct) / (100 - targetErrorPct), 1000)) * 100;
                                GUISetProgress(done / simOrder.Count * 100 + currentPct);
                                GUISetProgressText(String.Format("Simulating {0} - {2}/{3}", simOrder[done], currentPct, done + 1, statsWeights ? simOrder.Count : 1));
                                
                                OutputClear();
                                Output(String.Format("Simulating {0}, aiming for ±{1:N2}% precision...", simOrder[done], targetErrorPct));
                                Output(String.Format("Sims done : {0:N0}", CurrentDpsList.Count));
                                Output(String.Format("Sims running : {0:N0}", tasks.Count(t => !t.IsCompleted)));
                                Output(String.Format("Current precision : ±{0:N2}%", errorPct));

                                Thread.Sleep(TimeSpan.FromSeconds(Display == DisplayMode.Console ? 1.0 / 2 : 1.0 / 60));
                            }
                            
                            Output("Waiting for remaining simulations to complete...");
                            

                            Task.WaitAll(tasks.ToArray());
                        }
                    }

                    ErrorList.Add(Stats.ErrorPct(CurrentDpsList.ToArray(), CurrentDpsList.Average()));
                    ResultsList.Add(simOrder[done], CurrentResults);
                    DamagesList.Add(simOrder[done], CurrentDpsList);
                    if(jsonSim.Tanking)
                    {
                        ThreatsList.Add(simOrder[done], CurrentTpsList);
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

                string endMsg2 = string.Format("Overall accuracy of results : ±{0:N2}%", ErrorList.Average());
                Output(endMsg2);
                Log(endMsg2);

                string endMsg3 = string.Format("\nGenerating results...");
                Output(endMsg3);

                if (statsWeights)
                {
                    GUISetProgressText(String.Format("Generating Stats Weights..."));

                    double weightsDone = 0;

                    double baseDps = DamagesList["Base"].Average();
                    Log(string.Format("\nBase : {0:N2} DPS", baseDps));

                    Log("\nWeights by DPS :");

                    double apDif = 0;
                    if (simOrder.Contains("+50 AP"))
                    {
                        double apDps = DamagesList["+50 AP"].Average();
                        apDif = (apDps - baseDps) / (version == Version.TBC ? 100 : 50);
                        if (apDif < 0) apDif = 0;
                        Log(string.Format("1 AP = {0:N4} DPS", apDif));

                        double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * Player.BonusStrToAPRatio(playerBase);
                        Log(string.Format("1 Str = {0:N4} DPS = {1:N4} AP", strDif, strDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+50 SP"))
                    {
                        double apDps = DamagesList["+50 SP"].Average();
                        apDif = (apDps - baseDps) / (version == Version.TBC ? 100 : 50);
                        if (apDif < 0) apDif = 0;
                        Log(string.Format("1 SP = {0:N4} DPS", apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1% Crit"))
                    {
                        double critDps = DamagesList["+1% Crit"].Average();
                        double critDif = critDps - baseDps;
                        if (critDif < 0) critDif = 0;

                        double agiDif = Player.AgiToAPRatio(playerBase) * apDif + Player.AgiToCritRatio(playerBase.Class) * 100 * critDif;
                        Log(string.Format("1 Agi = {0:N4} DPS = {1:N4} AP", agiDif, agiDif / apDif));

                        critDif /= (version == Version.TBC ? Player.RatingRatios[Attribute.CritChance] : 1);

                        Log(string.Format("1{2} Crit = {0:N4} DPS = {1:N4} AP", critDif, critDif / apDif, version == Version.TBC ? "" : "%"));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1% Hit"))
                    {
                        double hitDps = DamagesList["+1% Hit"].Average();
                        double hitDif = (hitDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.HitChance] : 1);
                        if (hitDif < 0) hitDif = 0;
                        Log(string.Format("1{2} Hit = {0:N4} DPS = {1:N4} AP", hitDif, hitDif / apDif, version == Version.TBC ? "" : "%"));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1% Haste"))
                    {
                        double hasteDps = DamagesList["+1% Haste"].Average();
                        double hasteDif = (hasteDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.Haste] : 1);
                        if (hasteDif < 0) hasteDif = 0;
                        Log(string.Format("1{2} Haste = {0:N4} DPS = {1:N4} AP", hasteDif, hasteDif / apDif, version == Version.TBC ? "" : "%"));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1% Expertise"))
                    {
                        double expDps = DamagesList["+1% Expertise"].Average();
                        double expDif = (expDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.Expertise] : 1);
                        if (expDif < 0) expDif = 0;
                        Log(string.Format("1{2} Expertise = {0:N4} DPS = {1:N4} AP", expDif, expDif / apDif, version == Version.TBC ? "" : "%"));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+10 ArPen"))
                    {
                        double arpenDps = DamagesList["+10 ArPen"].Average();
                        double arpenDif = (arpenDps - baseDps) / 10;
                        if (arpenDif < 0) arpenDif = 0;
                        Log(string.Format("1 Armor Penetration (+10) = {0:N4} DPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+100 ArPen"))
                    {
                        double arpenDps = DamagesList["+100 ArPen"].Average();
                        double arpenDif = (arpenDps - baseDps) / 100;
                        if (arpenDif < 0) arpenDif = 0;
                        Log(string.Format("1 Armor Penetration (+100) = {0:N4} DPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1000 ArPen"))
                    {
                        double arpenDps = DamagesList["+1000 ArPen"].Average();
                        double arpenDif = (arpenDps - baseDps) / 1000;
                        if (arpenDif < 0) arpenDif = 0;
                        Log(string.Format("1 Armor Penetration (+1000) = {0:N4} DPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+10 DPS MH"))
                    {
                        double mhDps = DamagesList["+10 DPS MH"].Average();
                        double mhDif = (mhDps - baseDps) / simBonusAttribs["+10 DPS MH"].GetValue(Attribute.WeaponDamageMH);
                        if (mhDif < 0) mhDif = 0;
                        Log(string.Format("1 MH DPS = {0:N4} DPS = {1:N4} AP", mhDif, mhDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+10 DPS OH"))
                    {
                        double ohDps = DamagesList["+10 DPS OH"].Average();
                        double ohDif = (ohDps - baseDps) / simBonusAttribs["+10 DPS OH"].GetValue(Attribute.WeaponDamageOH);
                        if (ohDif < 0) ohDif = 0;
                        Log(string.Format("1 OH DPS = {0:N4} DPS = {1:N4} AP", ohDif, ohDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1 MH Skill"))
                    {
                        double mhSkillDps = DamagesList["+1 MH Skill"].Average();
                        double mhSkillDif = mhSkillDps - baseDps;
                        if (mhSkillDif < 0) mhSkillDif = 0;
                        Log(string.Format("1 MH Skill = {0:N4} DPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1 OH Skill"))
                    {
                        double ohSkillDps = DamagesList["+1 OH Skill"].Average();
                        double ohSkillDif = ohSkillDps - baseDps;
                        if (ohSkillDif < 0) ohSkillDif = 0;
                        Log(string.Format("1 OH Skill = {0:N4} DPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+5 MH Skill"))
                    {
                        double mhSkillDps = DamagesList["+5 MH Skill"].Average();
                        double mhSkillDif = mhSkillDps - baseDps;
                        if (mhSkillDif < 0) mhSkillDif = 0;
                        Log(string.Format("5 MH Skill = {0:N4} DPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+5 OH Skill"))
                    {
                        double ohSkillDps = DamagesList["+5 OH Skill"].Average();
                        double ohSkillDif = ohSkillDps - baseDps;
                        if (ohSkillDif < 0) ohSkillDif = 0;
                        Log(string.Format("5 OH Skill = {0:N4} DPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1% SpellCrit"))
                    {
                        double critDps = DamagesList["+1% SpellCrit"].Average();
                        double critDif = (critDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellCritChance] : 1);
                        if (critDif < 0) critDif = 0;
                        Log(string.Format("1% SpellCrit = {0:N4} DPS = {1:N4} SP", critDif, critDif / apDif, version == Version.TBC ? "" : "%"));
                        
                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+1% SpellHit"))
                    {
                        double hitDps = DamagesList["+1% SpellHit"].Average();
                        double hitDif = (hitDps - baseDps) / (version == Version.TBC ? Player.RatingRatios[Attribute.SpellHitChance] : 1);
                        if (hitDif < 0) hitDif = 0;
                        Log(string.Format("1{2} SpellHit = {0:N4} DPS = {1:N4} SP", hitDif, hitDif / apDif, version == Version.TBC ? "" : "%"));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+50 Int"))
                    {
                        double intDps = DamagesList["+50 Int"].Average();
                        double intDif = (intDps - baseDps) / (version == Version.TBC ? 100 : 50);
                        if (intDif < 0) intDif = 0;
                        string comp = simOrder.Contains("+50 AP") ? "AP" : "SP";
                        Log(string.Format("1 Int = {0:N4} DPS = {1:N4} {2}", intDif, intDif / apDif, comp));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+50 Spi"))
                    {
                        double spiDps = DamagesList["+50 Spi"].Average();
                        double spiDif = (spiDps - baseDps) / (version == Version.TBC ? 100 : 50);
                        if (spiDif < 0) spiDif = 0;
                        string comp = simOrder.Contains("+50 AP") ? "AP" : "SP";
                        Log(string.Format("1 Spi = {0:N4} DPS = {1:N4} {2}", spiDif, spiDif / apDif, comp));

                        weightsDone += 1;
                    }
                    if (simOrder.Contains("+30 MP5"))
                    {
                        double mp5Dps = DamagesList["+30 MP5"].Average();
                        double mp5Dif = (mp5Dps - baseDps) / 30;
                        if (mp5Dif < 0) mp5Dif = 0;
                        string comp = simOrder.Contains("+50 AP") ? "AP" : "SP";
                        Log(string.Format("1 MP5 = {0:N4} DPS = {1:N4} {2}", mp5Dif, mp5Dif / apDif, comp));

                        weightsDone += 1;
                    }

                    if (jsonSim.Tanking)
                    {
                        double baseTps = ThreatsList["Base"].Average();
                        Log(string.Format("\nBase : {0:N2} TPS", baseTps));

                        Log("\nWeights by TPS :");

                        apDif = 0;
                        if (simOrder.Contains("+50 AP"))
                        {
                            double apTps = ThreatsList["+50 AP"].Average();
                            apDif = (apTps - baseTps) / (version == Version.TBC ? 100 : 50);
                            if (apDif < 0) apDif = 0;
                            Log(string.Format("1 AP = {0:N4} TPS", apDif));

                            double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * (playerBase.Class == Player.Classes.Druid ? (1 + 0.04 * playerBase.GetTalentPoints("HW")) : 1);
                            Log(string.Format("1 Str = {0:N4} TPS = {1:N4} AP", strDif, strDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Crit"))
                        {
                            double critTps = ThreatsList["+1% Crit"].Average();
                            double critDif = critTps - baseTps;
                            if (critDif < 0) critDif = 0;

                            double agiDif = Player.AgiToAPRatio(playerBase) * apDif + Player.AgiToCritRatio(playerBase.Class) * 100 * critDif;
                            Log(string.Format("1 Agi = {0:N4} TPS = {1:N4} AP", agiDif, agiDif / apDif));

                            Log(string.Format("1{2} Crit = {0:N4} TPS = {1:N4} AP", critDif, critDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Hit"))
                        {
                            double hitTps = ThreatsList["+1% Hit"].Average();
                            double hitDif = hitTps - baseTps;
                            if (hitDif < 0) hitDif = 0;
                            Log(string.Format("1{2} Hit = {0:N4} TPS = {1:N4} AP", hitDif, hitDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Haste"))
                        {
                            double hasteTps = ThreatsList["+1% Haste"].Average();
                            double hasteDif = hasteTps - baseTps;
                            if (hasteDif < 0) hasteDif = 0;
                            Log(string.Format("1{2} Haste = {0:N4} TPS = {1:N4} AP", hasteDif, hasteDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1% Expertise"))
                        {
                            double expTps = ThreatsList["+1% Expertise"].Average();
                            double expDif = expTps - baseTps;
                            if (expDif < 0) expDif = 0;
                            Log(string.Format("1{2} Expertise = {0:N4} TPS = {1:N4} AP", expDif, expDif / apDif, version == Version.TBC ? "" : "%"));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+10 ArPen"))
                        {
                            double arpenTps = DamagesList["+10 ArPen"].Average();
                            double arpenDif = (arpenTps - baseTps) / 10;
                            if (arpenDif < 0) arpenDif = 0;
                            Log(string.Format("1 Armor Penetration (+10) = {0:N4} TPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+100 ArPen"))
                        {
                            double arpenTps = DamagesList["+100 ArPen"].Average();
                            double arpenDif = (arpenTps - baseTps) / 100;
                            if (arpenDif < 0) arpenDif = 0;
                            Log(string.Format("1 Armor Penetration (+100) = {0:N4} TPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1000 ArPen"))
                        {
                            double arpenTps = DamagesList["+1000 ArPen"].Average();
                            double arpenDif = (arpenTps - baseTps) / 1000;
                            if (arpenDif < 0) arpenDif = 0;
                            Log(string.Format("1 Armor Penetration (+1000) = {0:N4} TPS = {1:N4} AP", arpenDif, arpenDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+10 DPS MH"))
                        {
                            double mhTps = ThreatsList["+10 DPS MH"].Average();
                            double mhDif = (mhTps - baseTps) / simBonusAttribs["+10 DPS MH"].GetValue(Attribute.WeaponDamageMH);
                            if (mhDif < 0) mhDif = 0;
                            Log(string.Format("1 MH DPS = {0:N4} TPS = {1:N4} AP", mhDif, mhDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+10 DPS OH"))
                        {
                            double ohTps = ThreatsList["+10 DPS OH"].Average();
                            double ohDif = (ohTps - baseTps) / simBonusAttribs["+10 DPS OH"].GetValue(Attribute.WeaponDamageOH);
                            if (ohDif < 0) ohDif = 0;
                            Log(string.Format("1 OH DPS = {0:N4} TPS = {1:N4} AP", ohDif, ohDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 MH Skill"))
                        {
                            double mhSkillTps = ThreatsList["+1 MH Skill"].Average();
                            double mhSkillDif = mhSkillTps - baseTps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("1 MH Skill = {0:N4} TPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+1 OH Skill"))
                        {
                            double ohSkillTps = ThreatsList["+1 OH Skill"].Average();
                            double ohSkillDif = ohSkillTps - baseTps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("1 OH Skill = {0:N4} TPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 MH Skill"))
                        {
                            double mhSkillTps = ThreatsList["+5 MH Skill"].Average();
                            double mhSkillDif = mhSkillTps - baseTps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("5 MH Skill = {0:N4} TPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+5 OH Skill"))
                        {
                            double ohSkillTps = ThreatsList["+5 OH Skill"].Average();
                            double ohSkillDif = ohSkillTps - baseTps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("5 OH Skill = {0:N4} TPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+50 Int"))
                        {
                            double intTps = ThreatsList["+50 Int"].Average();
                            double intDif = (intTps - baseTps) / (version == Version.TBC ? 100 : 50);
                            if (intDif < 0) intDif = 0;
                            Log(string.Format("1 Int = {0:N4} TPS = {1:N4} AP", intDif, intDif / apDif));

                            weightsDone += 1;
                        }
                        if (simOrder.Contains("+50 Spi"))
                        {
                            double spiTps = ThreatsList["+50 Spi"].Average();
                            double spiDif = (spiTps - baseTps) / (version == Version.TBC ? 100 : 50);
                            if (spiDif < 0) spiDif = 0;
                            Log(string.Format("1 Spi = {0:N4} TPS = {1:N4} AP", spiDif, spiDif / apDif));

                            weightsDone += 1;
                        }
                    }
                }
                else if (nbSim >= 1)
                {
                    GUISetProgressText(String.Format("Generating Fight Stats..."));

                    double avgDps = CurrentDpsList.Average();

                    List<List<RegisteredAction>> totalActions = ResultsList["Base"].Select(r => r.Actions).ToList();
                    List<List<RegisteredEffect>> totalEffects = ResultsList["Base"].Select(r => r.Effects).ToList();

                    if (jsonSim.Tanking)
                    {
                        Log(string.Format("Average TPS : {0:N2} tps (±{1:N2})", CurrentTpsList.Average(), Stats.MeanStdDev(CurrentTpsList.ToArray())));
                    }
                    else
                    {
                        Log("");
                    }
                    Log(string.Format("Average DPS : {0:N2} dps (±{1:N2})", avgDps, Stats.MeanStdDev(CurrentDpsList.ToArray())));

                    //List<string> logList = totalActions.SelectMany(a => a.Select(t => t.Action.ToString()).OrderBy(b => b)).Distinct().ToList();
                    List<string> logListActions = new List<string>() { "AA MH", "AA OH", "AA Ranged", "AA Wand" };
                    if (playerBase.Class == Player.Classes.Warrior)
                        logListActions.AddRange(new List<string>() { "Slam", "Bloodthirst", "Mortal Strike", "Sunder Armor", "Revenge", "Whirlwind", "Heroic Strike", "Execute", "Hamstring", "Battle Shout" });
                    else if (playerBase.Class == Player.Classes.Druid)
                        logListActions.AddRange(new List<string>() { "Shred", "Ferocious Bite", "Shift", "Maul", "Swipe" });
                    else if (playerBase.Class == Player.Classes.Priest)
                        logListActions.AddRange(new List<string>() { "Mind Blast", "Mind Flay", "SW:P", "Devouring Plague" });
                    else if (playerBase.Class == Player.Classes.Rogue)
                        logListActions.AddRange(new List<string>() { "Sinister Strike", "Backstab", "Eviscerate", "Ambush", "Instant Poison" });
                    else if (playerBase.Class == Player.Classes.Warlock)
                        logListActions.AddRange(new List<string>() { "Shadow Bolt" });

                    logListActions.AddRange(new List<string>() { "Thunderfury", "Deathbringer", "Vis'kag the Bloodletter", "Perdition's Blade" });

                    //logList = totalEffects.SelectMany(a => a.Select(t => t.Effect.ToString()).OrderBy(b => b)).Distinct().ToList();
                    List<string> logListEffects = new List<string>() { };
                    if (playerBase.Class == Player.Classes.Priest)
                        logListEffects.AddRange(new List<string>() { "Mind Flay", "SW:P", "Devouring Plague" });
                    if (playerBase.Class == Player.Classes.Warrior)
                        logListEffects.AddRange(new List<string>() { "Deep Wounds" });
                    if (playerBase.Class == Player.Classes.Warlock)
                        logListEffects.AddRange(new List<string>() { "Corruption", "Malediction of Agony" });

                    int statsDone = 0;
                    int statsTotal = logListActions.Count + logListEffects.Count;
                    foreach (string ac in logListActions)
                    {
                        double totalAc = totalActions.Select(a => a.Where(t => t.Action.ToString().Equals(ac)).Count()).Sum();
                        if (totalAc > 0)
                        {
                            double avgAcUse = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac)));
                            double avgAcDps = totalActions.Average(a => a.Where(t => t.Action.ToString().Equals(ac)).Sum(r => r.Result.Damage / jsonSim.FightLength));
                            double avgAcDmg = totalActions.Sum(a => a.Where(t => t.Action.ToString().Equals(ac)).Sum(r => r.Result.Damage / totalAc));
                            string res = "\nAverage stats for [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)", avgAcDps, avgAcDps / avgDps * 100);
                            if (jsonSim.Tanking)
                            {
                                double avgTps = CurrentTpsList.Average();
                                double avgAcTps = totalActions.Average(a => a.Where(t => t.Action.ToString().Equals(ac)).Sum(r => r.Result.Threat / jsonSim.FightLength));
                                double avgAcThreat = totalActions.Sum(a => a.Where(t => t.Action.ToString().Equals(ac)).Sum(r => r.Result.Threat / totalAc));
                                res += string.Format(" / {0:N2} TPS ({1:N2}%)\n\tAverage of {2:N2} threat for {3:N2} uses (or 1 use every {4:N2}s)", avgAcTps, avgAcTps / avgTps * 100, avgAcThreat, avgAcUse, jsonSim.FightLength / avgAcUse);
                            }
                            if(ac == "Whirlwind")
                            {
                                avgAcUse /= 2;
                                avgAcDmg *= 2;
                            }
                            res += string.Format("\n\tAverage of {0:N2} damage for {1:N2} uses (or 1 use every {2:N2}s)", avgAcDmg, avgAcUse, jsonSim.FightLength / avgAcUse);

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
                                if(jsonSim.Tanking)
                                {
                                    double parryPct = totalActions.Average(a => a.Count(t => t.Action.ToString().Equals(ac) && t.Result.Type == ResultType.Parry)) / avgAcUse * 100;
                                    res += string.Format(", {0:N2}% Parry", parryPct);
                                }
                            }
                            Log(res);
                        }

                        statsDone++;
                    }

                    foreach (string ac in logListEffects)
                    {
                        double totalAc = totalEffects.Select(a => a.Where(t => t.Effect.ToString().Equals(ac)).Count()).Sum();
                        if(totalAc > 0)
                        {
                            double avgAcUse = totalEffects.Average(a => a.Count(t => t.Effect.ToString().Equals(ac)));
                            double avgAcDps = totalEffects.Average(a => a.Where(t => t.Effect.ToString().Equals(ac)).Sum(r => r.Damage / jsonSim.FightLength));
                            double avgAcDmg = totalEffects.Sum(a => a.Where(t => t.Effect.ToString().Equals(ac)).Sum(r => r.Damage / totalAc));
                            string res = "\nAverage stats for [" + ac + "] : ";
                            res += string.Format("{0:N2} DPS ({1:N2}%)\n\tAverage of {2:N2} damage for {3:N2} ticks (or 1 tick every {4:N2}s)", avgAcDps, avgAcDps / avgDps * 100, avgAcDmg, avgAcUse, jsonSim.FightLength / avgAcUse);
                            double tickDelay = 3;
                            if (ac == "Mind Flay") tickDelay = 1;
                            double uptime = tickDelay / (jsonSim.FightLength / avgAcUse) * 100;
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

                GUISetProgressText(String.Format("Finished !"));
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

        public static void AddSimThreat(double damage)
        {
            lock (CurrentTpsList)
            {
                CurrentTpsList.Add(damage);
            }
        }

        public static void Log(object log)
        {
            logs += log.ToString() + "\n";
        }

        public static void Debug(object str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        public static void Output(object str, bool newLine = true)
        {
            if(Display == DisplayMode.Console)
            {
                if (newLine) Console.WriteLine(str.ToString());
                else Console.Write(str.ToString());
            }
            else if(GUI != null)
            {
                GUI.ConsoleTextAdd(str.ToString(), newLine);
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

            Simulation s = new Simulation(player, boss, jsonSim.FightLength, jsonSim.BossAutoLife, jsonSim.BossLowLifeTime, jsonSim.FightLengthMod, jsonSim.UnlimitedMana, jsonSim.UnlimitedResource, jsonSim.Tanking, jsonSim.TankHitEvery, jsonSim.TankHitRage);
            s.StartSim();
        }
    }
}

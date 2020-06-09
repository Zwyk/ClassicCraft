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

        private static Dictionary<string, List<SimResult>> ResultsList = new Dictionary<string, List<SimResult>>();
        private static Dictionary<string, List<double>> DamagesList = new Dictionary<string, List<double>>();
        private static Dictionary<string, List<double>> ThreatsList = new Dictionary<string, List<double>>();
        private static List<double> ErrorList = new List<double>();

        public static string logs = "";

        public static List<string> simOrder = new List<string>(){
            "Base",
            "+50 AP", "+50 SP",
            "+1% Hit","+1% Crit", "+1% Haste",
            "+1% SpellHit","+1% SpellCrit",
            "+50 Int", "+50 Spi", "+30 MP5",
            "+10 DPS MH", "+10 DPS OH",
            "+1 MH Skill", "+1 OH Skill",
            "+5 MH Skill", "+5 OH Skill",
            //"1","2","3","4","5","6","7","8","9",
        };
        public static Dictionary<string, Attributes> simBonusAttribs = new Dictionary<string, Attributes>()
                {
                    { "Base", new Attributes() },
                    { "+50 AP", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.AP, 50 }
                            })},
                    { "+50 SP", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.SP, 50 }
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
                                { Attribute.Intellect, 50 }
                            })},
                    { "+50 Spi", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.Spirit, 50 }
                            })},
                    { "+30 MP5", new Attributes(new Dictionary<Attribute, double>()
                            {
                                { Attribute.MP5, 30 }
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
                    CurrentTpsList = new List<double>();
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
                    if(jsonSim.Tanking)
                    {
                        ThreatsList.Add(simOrder[done], CurrentTpsList);
                    }
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
                    if (simOrder.Contains("+50 AP"))
                    {
                        double apDps = DamagesList["+50 AP"].Average();
                        apDif = (apDps - baseDps) / 50;
                        if (apDif < 0) apDif = 0;
                        Log(string.Format("1 AP = {0:N4} DPS", apDif));

                        double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * Player.BonusStrToAPRatio(playerBase);
                        Log(string.Format("1 Str = {0:N4} DPS = {1:N4} AP", strDif, strDif / apDif));
                    }
                    if (simOrder.Contains("+50 SP"))
                    {
                        double apDps = DamagesList["+50 SP"].Average();
                        apDif = (apDps - baseDps) / 50;
                        if (apDif < 0) apDif = 0;
                        Log(string.Format("1 SP = {0:N4} DPS", apDif));
                    }
                    if (simOrder.Contains("+1% Crit"))
                    {
                        double critDps = DamagesList["+1% Crit"].Average();
                        double critDif = critDps - baseDps;
                        if (critDif < 0) critDif = 0;

                        double agiDif = Player.AgiToAPRatio(playerBase) * apDif + Player.AgiToCritRatio(playerBase.Class) * 100 * critDif;
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
                    if (simOrder.Contains("+1% SpellCrit"))
                    {
                        double critDps = DamagesList["+1% SpellCrit"].Average();
                        double critDif = critDps - baseDps;
                        if (critDif < 0) critDif = 0;
                        Log(string.Format("1% SpellCrit = {0:N4} DPS = {1:N4} SP", critDif, critDif / apDif));
                    }
                    if (simOrder.Contains("+1% SpellHit"))
                    {
                        double hitDps = DamagesList["+1% SpellHit"].Average();
                        double hitDif = hitDps - baseDps;
                        if (hitDif < 0) hitDif = 0;
                        Log(string.Format("1% SpellHit = {0:N4} DPS = {1:N4} SP", hitDif, hitDif / apDif));
                    }
                    if (simOrder.Contains("+50 Int"))
                    {
                        double intDps = DamagesList["+50 Int"].Average();
                        double intDif = (intDps - baseDps) / 50;
                        if (intDif < 0) intDif = 0;
                        string comp = simOrder.Contains("+50 AP") ? "AP" : "SP";
                        Log(string.Format("1 Int = {0:N4} DPS = {1:N4} {2}", intDif, intDif / apDif, comp));
                    }
                    if (simOrder.Contains("+50 Spi"))
                    {
                        double spiDps = DamagesList["+50 Spi"].Average();
                        double spiDif = (spiDps - baseDps) / 50;
                        if (spiDif < 0) spiDif = 0;
                        string comp = simOrder.Contains("+50 AP") ? "AP" : "SP";
                        Log(string.Format("1 Spi = {0:N4} DPS = {1:N4} {2}", spiDif, spiDif / apDif, comp));
                    }
                    if (simOrder.Contains("+30 MP5"))
                    {
                        double mp5Dps = DamagesList["+30 MP5"].Average();
                        double mp5Dif = (mp5Dps - baseDps) / 30;
                        if (mp5Dif < 0) mp5Dif = 0;
                        string comp = simOrder.Contains("+50 AP") ? "AP" : "SP";
                        Log(string.Format("1 MP5 = {0:N4} DPS = {1:N4} {2}", mp5Dif, mp5Dif / apDif, comp));
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
                    if (simOrder.Contains("+1 MH Skill"))
                    {
                        double mhSkillDps = DamagesList["+1 MH Skill"].Average();
                        double mhSkillDif = mhSkillDps - baseDps;
                        if (mhSkillDif < 0) mhSkillDif = 0;
                        Log(string.Format("+1 MH Skill = {0:N4} DPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));
                    }
                    if (simOrder.Contains("+1 OH Skill"))
                    {
                        double ohSkillDps = DamagesList["+1 OH Skill"].Average();
                        double ohSkillDif = ohSkillDps - baseDps;
                        if (ohSkillDif < 0) ohSkillDif = 0;
                        Log(string.Format("+1 OH Skill = {0:N4} DPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));
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

                    if (jsonSim.Tanking)
                    {
                        double baseTps = ThreatsList["Base"].Average();
                        Log(string.Format("\nBase : {0:N2} TPS", baseTps));

                        Log("\nWeights by TPS :");

                        apDif = 0;
                        if (simOrder.Contains("+50 AP"))
                        {
                            double apTps = ThreatsList["+50 AP"].Average();
                            apDif = (apTps - baseTps) / 50;
                            if (apDif < 0) apDif = 0;
                            Log(string.Format("1 AP = {0:N4} TPS", apDif));

                            double strDif = apDif * Player.StrToAPRatio(playerBase.Class) * (playerBase.Class == Player.Classes.Druid ? (1 + 0.04 * playerBase.GetTalentPoints("HW")) : 1);
                            Log(string.Format("1 Str = {0:N4} TPS = {1:N4} AP", strDif, strDif / apDif));
                        }
                        if (simOrder.Contains("+1% Crit"))
                        {
                            double critTps = ThreatsList["+1% Crit"].Average();
                            double critDif = critTps - baseTps;
                            if (critDif < 0) critDif = 0;

                            double agiDif = Player.AgiToAPRatio(playerBase) * apDif + Player.AgiToCritRatio(playerBase.Class) * 100 * critDif;
                            Log(string.Format("1 Agi = {0:N4} TPS = {1:N4} AP", agiDif, agiDif / apDif));

                            Log(string.Format("1% Crit = {0:N4} TPS = {1:N4} AP", critDif, critDif / apDif));
                        }
                        if (simOrder.Contains("+1% Hit"))
                        {
                            double hitTps = ThreatsList["+1% Hit"].Average();
                            double hitDif = hitTps - baseTps;
                            if (hitDif < 0) hitDif = 0;
                            Log(string.Format("1% Hit = {0:N4} TPS = {1:N4} AP", hitDif, hitDif / apDif));
                        }
                        if (simOrder.Contains("+1% Haste"))
                        {
                            double hasteTps = ThreatsList["+1% Haste"].Average();
                            double hasteDif = hasteTps - baseTps;
                            if (hasteDif < 0) hasteDif = 0;
                            Log(string.Format("1% Haste = {0:N4} TPS = {1:N4} AP", hasteDif, hasteDif / apDif));
                        }
                        if (simOrder.Contains("+50 Int"))
                        {
                            double intTps = ThreatsList["+50 Int"].Average();
                            double intDif = (intTps - baseTps) / 50;
                            if (intDif < 0) intDif = 0;
                            Log(string.Format("1 Int = {0:N4} TPS = {1:N4} AP", intDif, intDif / apDif));
                        }
                        if (simOrder.Contains("+50 Spi"))
                        {
                            double spiTps = ThreatsList["+50 Spi"].Average();
                            double spiDif = (spiTps - baseTps) / 50;
                            if (spiDif < 0) spiDif = 0;
                            Log(string.Format("1 Spi = {0:N4} TPS = {1:N4} AP", spiDif, spiDif / apDif));
                        }
                        if (simOrder.Contains("+10 DPS MH"))
                        {
                            double mhTps = ThreatsList["+10 DPS MH"].Average();
                            double mhDif = (mhTps - baseTps) / simBonusAttribs["+10 DPS MH"].GetValue(Attribute.WeaponDamageMH);
                            if (mhDif < 0) mhDif = 0;
                            Log(string.Format("+1 MH DPS = {0:N4} TPS = {1:N4} AP", mhDif, mhDif / apDif));
                        }
                        if (simOrder.Contains("+10 DPS OH"))
                        {
                            double ohTps = ThreatsList["+10 DPS OH"].Average();
                            double ohDif = (ohTps - baseTps) / simBonusAttribs["+10 DPS OH"].GetValue(Attribute.WeaponDamageOH);
                            if (ohDif < 0) ohDif = 0;
                            Log(string.Format("+1 OH DPS = {0:N4} TPS = {1:N4} AP", ohDif, ohDif / apDif));
                        }
                        if (simOrder.Contains("+1 MH Skill"))
                        {
                            double mhSkillTps = ThreatsList["+1 MH Skill"].Average();
                            double mhSkillDif = mhSkillTps - baseTps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("+1 MH Skill = {0:N4} TPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));
                        }
                        if (simOrder.Contains("+1 OH Skill"))
                        {
                            double ohSkillTps = ThreatsList["+1 OH Skill"].Average();
                            double ohSkillDif = ohSkillTps - baseTps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("+1 OH Skill = {0:N4} TPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));
                        }
                        if (simOrder.Contains("+5 MH Skill"))
                        {
                            double mhSkillTps = ThreatsList["+5 MH Skill"].Average();
                            double mhSkillDif = mhSkillTps - baseTps;
                            if (mhSkillDif < 0) mhSkillDif = 0;
                            Log(string.Format("+5 MH Skill = {0:N4} TPS = {1:N4} AP", mhSkillDif, mhSkillDif / apDif));
                        }
                        if (simOrder.Contains("+5 OH Skill"))
                        {
                            double ohSkillTps = ThreatsList["+5 OH Skill"].Average();
                            double ohSkillDif = ohSkillTps - baseTps;
                            if (ohSkillDif < 0) ohSkillDif = 0;
                            Log(string.Format("+5 OH Skill = {0:N4} TPS = {1:N4} AP", ohSkillDif, ohSkillDif / apDif));
                        }
                    }
                }
                else if (nbSim >= 1)
                {
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
                    List<string> logList = new List<string>() { "AA MH", "AA OH", "AA Ranged", "AA Wand" };
                    if (playerBase.Class == Player.Classes.Warrior)
                        logList.AddRange(new List<string>() { "Slam", "Bloodthirst", "Sunder Armor", "Revenge", "Whirlwind", "Heroic Strike", "Execute", "Hamstring" });
                    else if (playerBase.Class == Player.Classes.Druid)
                        logList.AddRange(new List<string>() { "Shred", "Ferocious Bite", "Shift", "Maul", "Swipe" });
                    else if (playerBase.Class == Player.Classes.Priest)
                        logList.AddRange(new List<string>() { "Mind Blast", "Mind Flay", "SW:P", "Devouring Plague" });
                    else if (playerBase.Class == Player.Classes.Rogue)
                        logList.AddRange(new List<string>() { "Sinister Strike", "Backstab", "Eviscerate", "Ambush", "Instant Poison" });
                    else if (playerBase.Class == Player.Classes.Warlock)
                        logList.AddRange(new List<string>() { "Shadow Bolt" });

                    logList.AddRange(new List<string>() { "Thunderfury", "Deathbringer", "Vis'kag the Bloodletter", "Perdition's Blade" });

                    foreach (string ac in logList)
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
                    }

                    //logList = totalEffects.SelectMany(a => a.Select(t => t.Effect.ToString()).OrderBy(b => b)).Distinct().ToList();
                    logList = new List<string>() { };
                    if (playerBase.Class == Player.Classes.Priest)
                        logList.AddRange(new List<string>() { "Mind Flay", "SW:P", "Devouring Plague" });
                    if (playerBase.Class == Player.Classes.Warrior)
                        logList.AddRange(new List<string>() { "Deep Wounds" });
                    if (playerBase.Class == Player.Classes.Warlock)
                        logList.AddRange(new List<string>() { "Corruption", "Malediction of Agony" });

                    foreach (string ac in logList)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Warrior : Player
    {
        private Whirlwind ww = null;
        private Bloodthirst bt = null;
        private HeroicStrike hs = null;
        private Execute exec = null;
        private Bloodrage br = null;
        private BattleShout bs = null;
        private Hamstring ham = null;
        private Slam slam = null;
        private Revenge rev = null;
        private SunderArmor sa = null;
        private Rampage ramp = null;
        private MortalStrike ms = null;
        private Cleave cl = null;
        private SweepingStrikes ss = null;
        private ShieldSlam shslam = null;
        private Devastate dev = null;
        private Thunderclap tc = null;

        #region Constructors

        public Warrior(Player p)
            : base(p)
        {
        }

        public Warrior(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Warrior(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null, bool tanking = false, bool facing = false, List<string> cooldowns = null)
            : base(s, Classes.Warrior, r, level, items, talents, buffs, tanking, facing, cooldowns)
        {
        }

        #endregion

        #region Talents

        public override void SetupTalents(string ptal)
        {
            if (ptal == null || ptal == "")
            {
                if (MH.TwoHanded)
                {
                    // DPS Fury 2H
                    switch (Program.version)
                    {
                        case Version.Vanilla: ptal = "30305001332-05052005025010051"; break;
                        case Version.TBC: ptal = "32024001302-050500054050120531251"; break; // TODO
                    }
                }
                else
                {
                    // DPS Fury 1H
                    switch (Program.version)
                    {
                        case Version.Vanilla: ptal = "30305001302-05050005525010051"; break;
                        case Version.TBC: ptal = "32024001302-050500054050120531251"; break;
                    }
                }
            }

            string[] talents = ptal.Split('-');
            string arms = talents.Length > 0 ? talents[0] : "";
            string fury = talents.Length > 1 ? talents[1] : "";
            string prot = talents.Length > 2 ? talents[2] : "";

            Talents = new Dictionary<string, int>();

            switch (Program.version)
            {
                case Version.Vanilla:
                    // Arms
                    Talents.Add("IHS", arms.Length > 0 ? (int)Char.GetNumericValue(arms[0]) : 0);
                    Talents.Add("DW", arms.Length > 8 ? (int)Char.GetNumericValue(arms[8]) : 0);
                    Talents.Add("2HS", arms.Length > 9 ? (int)Char.GetNumericValue(arms[9]) : 0);
                    Talents.Add("Impale", arms.Length > 10 ? (int)Char.GetNumericValue(arms[10]) : 0);
                    Talents.Add("Sword", arms.Length > 14 ? (int)Char.GetNumericValue(arms[14]) : 0);
                    Talents.Add("MS", arms.Length > 19 ? (int)Char.GetNumericValue(arms[17]) : 0);
                    // Fury
                    Talents.Add("Cruelty", fury.Length > 1 ? (int)Char.GetNumericValue(fury[1]) : 0);
                    Talents.Add("UW", fury.Length > 3 ? (int)Char.GetNumericValue(fury[3]) : 0);
                    Talents.Add("IBS", fury.Length > 7 ? (int)Char.GetNumericValue(fury[7]) : 0);
                    Talents.Add("DWS", fury.Length > 8 ? (int)Char.GetNumericValue(fury[8]) : 0);
                    Talents.Add("IE", fury.Length > 9 ? (int)Char.GetNumericValue(fury[9]) : 0);
                    Talents.Add("IS", fury.Length > 11 ? (int)Char.GetNumericValue(fury[11]) : 0);
                    Talents.Add("DeathWish", fury.Length > 12 ? (int)Char.GetNumericValue(fury[12]) : 0);
                    Talents.Add("Flurry", fury.Length > 15 ? (int)Char.GetNumericValue(fury[15]) : 0);
                    Talents.Add("BT", fury.Length > 16 ? (int)Char.GetNumericValue(fury[16]) : 0);
                    // Protection
                    Talents.Add("IBR", prot.Length > 2 ? (int)Char.GetNumericValue(prot[2]) : 0);
                    Talents.Add("Defiance", prot.Length > 8 ? (int)Char.GetNumericValue(prot[8]) : 0);
                    Talents.Add("ISA", prot.Length > 9 ? (int)Char.GetNumericValue(prot[9]) : 0);
                    break;
                case Version.TBC:
                    // Arms
                    Talents.Add("IHS", arms.Length > 0 ? (int)Char.GetNumericValue(arms[0]) : 0);
                    Talents.Add("ITC", arms.Length > 5 ? (int)Char.GetNumericValue(arms[5]) : 0);
                    Talents.Add("AM", arms.Length > 7 ? (int)Char.GetNumericValue(arms[7]) : 0);
                    Talents.Add("DW", arms.Length > 8 ? (int)Char.GetNumericValue(arms[8]) : 0);
                    Talents.Add("2HS", arms.Length > 9 ? (int)Char.GetNumericValue(arms[9]) : 0);
                    Talents.Add("Impale", arms.Length > 10 ? (int)Char.GetNumericValue(arms[10]) : 0);
                    Talents.Add("Poleaxe", arms.Length > 11 ? (int)Char.GetNumericValue(arms[11]) : 0);
                    Talents.Add("DeathWish", arms.Length > 12 ? (int)Char.GetNumericValue(arms[12]) : 0);
                    Talents.Add("Mace", arms.Length > 13 ? (int)Char.GetNumericValue(arms[13]) : 0);
                    Talents.Add("Sword", arms.Length > 14 ? (int)Char.GetNumericValue(arms[14]) : 0);
                    Talents.Add("ID", arms.Length > 17 ? (int)Char.GetNumericValue(arms[17]) : 0);
                    Talents.Add("MS", arms.Length > 19 ? (int)Char.GetNumericValue(arms[19]) : 0);
                    Talents.Add("IMS", arms.Length > 21 ? (int)Char.GetNumericValue(arms[21]) : 0);
                    Talents.Add("ER", arms.Length > 22 ? (int)Char.GetNumericValue(arms[22]) : 0);
                    // Fury
                    Talents.Add("Cruelty", fury.Length > 1 ? (int)Char.GetNumericValue(fury[1]) : 0);
                    Talents.Add("UW", fury.Length > 3 ? (int)Char.GetNumericValue(fury[3]) : 0);
                    Talents.Add("IBS", fury.Length > 7 ? (int)Char.GetNumericValue(fury[7]) : 0);
                    Talents.Add("DWS", fury.Length > 8 ? (int)Char.GetNumericValue(fury[8]) : 0);
                    Talents.Add("IE", fury.Length > 9 ? (int)Char.GetNumericValue(fury[9]) : 0);
                    Talents.Add("IS", fury.Length > 11 ? (int)Char.GetNumericValue(fury[11]) : 0);
                    Talents.Add("SS", fury.Length > 12 ? (int)Char.GetNumericValue(fury[12]) : 0);
                    Talents.Add("WM", fury.Length > 13 ? (int)Char.GetNumericValue(fury[13]) : 0);
                    Talents.Add("Flurry", fury.Length > 15 ? (int)Char.GetNumericValue(fury[15]) : 0);
                    Talents.Add("Precision", fury.Length > 16 ? (int)Char.GetNumericValue(fury[16]) : 0);
                    Talents.Add("BT", fury.Length > 17 ? (int)Char.GetNumericValue(fury[17]) : 0);
                    Talents.Add("IWW", fury.Length > 18 ? (int)Char.GetNumericValue(fury[18]) : 0);
                    Talents.Add("IBStance", fury.Length > 19 ? (int)Char.GetNumericValue(fury[19]) : 0);
                    Talents.Add("Rampage", fury.Length > 20 ? (int)Char.GetNumericValue(fury[20]) : 0);
                    // Protection
                    Talents.Add("IBR", prot.Length > 0 ? (int)Char.GetNumericValue(prot[0]) : 0);
                    Talents.Add("TM", prot.Length > 1 ? (int)Char.GetNumericValue(prot[1]) : 0);
                    Talents.Add("Defiance", prot.Length > 8 ? (int)Char.GetNumericValue(prot[8]) : 0);
                    Talents.Add("ISA", prot.Length > 9 ? (int)Char.GetNumericValue(prot[9]) : 0);
                    Talents.Add("1HS", prot.Length > 16 ? (int)Char.GetNumericValue(prot[16]) : 0);
                    Talents.Add("ShieldSlam", prot.Length > 18 ? (int)Char.GetNumericValue(prot[18]) : 0);
                    Talents.Add("FR", prot.Length > 19 ? (int)Char.GetNumericValue(prot[19]) : 0);
                    Talents.Add("Vitality", prot.Length > 20 ? (int)Char.GetNumericValue(prot[20]) : 0);
                    Talents.Add("Devastate", prot.Length > 21 ? (int)Char.GetNumericValue(prot[21]) : 0);
                    break;
            }
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            br = new Bloodrage(this);
            bs = new BattleShout(this);
            hs = new HeroicStrike(this);
            cl = new Cleave(this);
            if (Talents["BT"] > 0) bt = new Bloodthirst(this);
            if (Talents["MS"] > 0) ms = new MortalStrike(this);
            if (GetTalentPoints("SS") > 0) ss = new SweepingStrikes(this);
            if (Talents["IS"] > 0) slam = new Slam(this);

            if (Sim.Tanking && Sim.TankHitRage > 0 && Sim.TankHitEvery > 0)
            {
                sa = new SunderArmor(this);
                rev = new Revenge(this);
                dev = new Devastate(this);
                tc = new Thunderclap(this);
                if(!DualWielding) shslam = new ShieldSlam(this);
            }
            else
            {
                ham = new Hamstring(this);
                ww = new Whirlwind(this);
                exec = new Execute(this);

                if (Program.version == Version.TBC && Talents["Rampage"] > 0)
                {
                    ramp = new Rampage(this);
                }
            }

            HasteMod = CalcHaste();

            if (Cooldowns != null)
            {
                foreach (string s in Cooldowns)
                {
                    switch (s)
                    {
                        case "Death Wish": if (Talents["DeathWish"] > 0) cds.Add(new DeathWish(this), DeathWishBuff.LENGTH); break;
                        case "Juju Flurry": cds.Add(new JujuFlurry(this), JujuFlurryBuff.LENGTH); break;
                        case "Mighty Rage Potion": cds.Add(new MightyRage(this), MightyRageBuff.LENGTH); break;
                        case "Recklessness": cds.Add(new Recklessness(this), RecklessnessBuff.LENGTH); break;
                        case "Shield Block": cds.Add(new ShieldBlock(this), 5); break;
                        case "Racial":
                            if (Race == Races.Orc)
                            {
                                cds.Add(new BloodFury(this), BloodFuryBuff.LENGTH);
                            }
                            else if (Race == Races.Troll)
                            {
                                cds.Add(new Berserking(this), BerserkingBuff.LENGTH);
                            }
                            break;
                    }
                }
            }
            
            if (Program.version == Version.TBC)
            {
                /*
                if (Sim.Tanking)                                    // Tank
                {
                    rota = 12;   // Fury Tank
                }
                else if (GetTalentPoints("BT") > 0)                  // Fury
                {
                    if (GetTalentPoints("IS") > 0)
                    {
                        rota = 101;   // Fury Slam
                    }
                    else
                    {
                        rota = 10;   // Fury regular
                    }
                }
                else if (GetTalentPoints("MS") > 0)                 // Arms
                {
                    if (GetTalentPoints("IS") > 0)
                    {
                        rota = 11;  // Arms Slam
                    }
                    else
                    {
                        rota = 111; // Arms no slam
                    }
                }
                */

                rota = Tanking && Sim.TankHitRage > 0 && Sim.TankHitEvery > 0 ? 21 : 20;
                Spells = new List<Action>();
                if (bt != null) Spells.Add(bt);
                if (ms != null) Spells.Add(ms);
                if (slam != null) Spells.Add(slam);
                if(Tanking && Sim.TankHitRage > 0 && Sim.TankHitEvery > 0)
                {
                    Spells.Add(sa);
                    Spells.Add(rev);
                }
                else
                {
                    Spells.Add(ww);
                    Spells.Add(exec);
                }
            }
            else
            {
                if (Sim.Tanking && Sim.TankHitRage > 0 && Sim.TankHitEvery > 0)
                {
                    rota = 2;   // Fury Tank
                    // rota = 3 // Fury Tank Battle Shout
                }
                else if (GetTalentPoints("IS") > 0)
                {
                    rota = 1;   // Fury Slam
                }
                else
                {
                    rota = 0;   // Fury
                }
            }
        }

        public override void Rota()
        {
            if (br.CanUse() && Resource < 90)
            {
                br.Cast();
            }

            /*
            if (bs.CanUse() && (!Effects.Any(e => e is BattleShoutBuff) || ((BattleShoutBuff)Effects.Where(e => e is BattleShoutBuff).First()).RemainingTime() < GCD))
            {
                bs.Cast();
            }
            */

            if (cds != null)
            {
                foreach (Skill cd in cds.Keys)
                {
                    if (cd.CanUse() &&
                        (Sim.FightLength - Sim.CurrentTime <= cds[cd]
                        || Sim.FightLength - Sim.CurrentTime >= cd.BaseCD + cds[cd]))
                    {
                        if (!(cd is MightyRage) || Sim.FightLength - Sim.CurrentTime >= cd.BaseCD + cds[cd] || ((!Tanking || Sim.TankHitRage == 0 || Sim.TankHitEvery == 0) && Resource < exec.Cost))
                        {
                            cd.Cast();
                        }
                    }
                }
            }

            // VANILLA
            if (rota == 0) //BT > WW > HAM + HS + EXEC
            {
                if (Sim.Boss.LifePct > 0.2)
                {
                    if (Sim.NbTargets > 1 && ww.CanUse())
                    {
                        ww.Cast();
                    }
                    else if (bt.CanUse())
                    {
                        bt.Cast();
                    }
                    else if (ww.CanUse() && Resource >= ww.Cost + bt.Cost && bt.RemainingCD() >= GCD)
                    {
                        ww.Cast();
                    }
                    else if (ham.CanUse() && Resource >= bt.Cost + ww.Cost + hs.Cost && ww.RemainingCD() >= GCD && bt.RemainingCD() >= GCD && bt.RemainingCD() >= GCD && (!Effects.ContainsKey(Flurry.NAME) || ((Flurry)Effects[Flurry.NAME]).CurrentStacks < 3))
                    {
                        ham.Cast();
                    }

                    if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                    {
                        cl.Cast();
                    }
                    else if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
                    {
                        hs.Cast();
                    }
                    else if (!MH.TwoHanded && Resource < bt.Cost + ww.Cost + hs.Cost)
                    {
                        applyAtNextAA = null;
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
            else if (rota == 1) //SLAM + EXEC
            {
                if (Sim.Boss.LifePct > 0.2)
                {
                    if (slam.CanUse())
                    {
                        slam.Cast();
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
            else if (rota == 2) //BT > REVENGE > SA + HS
            {
                if (bt.CanUse())
                {
                    bt.Cast();
                }
                else if (rev.CanUse())
                {
                    rev.Cast();
                }
                else if (sa.CanUse())
                {
                    sa.Cast();
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast();
                }
                else if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + sa.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast();
                }
                else if (!MH.TwoHanded && applyAtNextAA != null && Resource < bt.Cost + sa.Cost + hs.Cost)
                {
                    applyAtNextAA = null;
                }
            }
            else if (rota == 3) //BT > REVENGE > BS + HS
            {
                if (bt.CanUse())
                {
                    bt.Cast();
                }
                else if (rev.CanUse())
                {
                    rev.Cast();
                }
                else if (bs.CanUse())
                {
                    bs.Cast();
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast();
                }
                else if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + sa.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast();
                }
                else if (!MH.TwoHanded && applyAtNextAA != null && Resource < bt.Cost + sa.Cost + hs.Cost)
                {
                    applyAtNextAA = null;
                }
            }
            // TBC
            else if (rota == 10) // RAMPAGE > WW > BT > EXEC > HS
            {
                if (ss != null && Sim.NbTargets > 1 && ss.CanUse())
                {
                    ss.Cast();
                }

                if (ramp != null && (!Effects.ContainsKey(RampageBuff.NAME) || Effects[RampageBuff.NAME].RemainingTime() < GCD_Hasted() * 2) && ramp.CanUse())
                {
                    ramp.Cast();
                }
                else if (ww.CanUse())
                {
                    ww.Cast();
                }
                else if (bt.CanUse())
                {
                    bt.Cast();
                }
                else if (Sim.Boss.LifePct <= 0.2 && exec.CanUse())
                {
                    exec.Cast();
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast();
                }
                else if (applyAtNextAA == null && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast();
                }
            }
            else if (rota == 11) // Slam > MS > WW + HS + Exec
            {
                if (ss != null && Sim.NbTargets > 1 && ss.CanUse())
                {
                    ss.Cast();
                }


                if (Sim.NbTargets > 1 && ww.CanUse())
                {
                    ww.Cast();
                }
                else if (mh.LockedUntil - Sim.CurrentTime >= mh.CurrentSpeed() * 0.95 && slam.CanUse())
                {
                    slam.Cast();
                }
                else if (ms.CanUse())
                {
                    ms.Cast();
                }
                else if (ww.CanUse())
                {
                    ww.Cast();
                }
                else if (Sim.Boss.LifePct <= 0.2 && exec.CanUse())
                {
                    exec.Cast();
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast();
                }
                else if (applyAtNextAA == null && Resource >= ms.Cost + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast();
                }
            }
            else if (rota == 111) // MS > WW > HS + Exec
            {
                if (ss != null && Sim.NbTargets > 1 && ss.CanUse())
                {
                    ss.Cast();
                }

                if (Sim.NbTargets > 1 && ww.CanUse())
                {
                    ww.Cast();
                }
                else if (Sim.Boss.LifePct <= 0.2 && exec.CanUse())
                {
                    exec.Cast();
                }
                else if (ms.CanUse())
                {
                    ms.Cast();
                }
                else if (ww.CanUse())
                {
                    ww.Cast();
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast();
                }
                else if (applyAtNextAA == null && Resource >= ms.Cost + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast();
                }
            }
            // AUTO DPS BY DPR
            else if (rota == 20)
            {
                if (HasGCD() && 
                    (DPRAtAP != AP || (Sim.Boss.LifePct <= 0.2 && DPRAtRage != Resource) || (slam != null && slam.CanUse())))
                {
                    CalcDPR();
                }

                if (ss != null && Sim.NbTargets > 1 && ss.CanUse())
                {
                    ss.Cast();
                }

                if(HasGCD())
                {
                    if (ramp != null && (!Effects.ContainsKey(RampageBuff.NAME) || Effects[RampageBuff.NAME].RemainingTime() < GCD_Hasted() * 2) && ramp.CanUse())
                    {
                        ramp.Cast();
                    }
                    else
                    {
                        foreach (KeyValuePair<Action, double> a in SpellsDPR)
                        {
                            if (a.Key.CanUse() && a.Value > 0)
                            {
                                if (a.Key is Slam)
                                {
                                    if (a.Value == SpellsDPR.Max(v => v.Value))
                                    {
                                        a.Key.Cast();
                                    }
                                }
                                else
                                {
                                    a.Key.Cast();
                                }
                            }
                        }
                    }
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast();
                }
                else if (DualWielding && applyAtNextAA == null && Resource >= (bt != null ? bt.Cost : 0) + (ms != null ? ms.Cost : 0) + (slam != null ? slam.Cost : 0) + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast();
                }
            }
            // TANKING
            else if(rota == 21)
            {
                if (tc != null && Sim.NbTargets > 1 && tc.CanUse())
                {
                    tc.Cast();
                }
                else if (bt != null && bt.CanUse())
                {
                    bt.Cast();
                }
                else if (ms != null && ms.CanUse())
                {
                    ms.Cast();
                }
                else if (shslam != null && shslam.CanUse())
                {
                    shslam.Cast();
                }
                else if (rev.CanUse())
                {
                    rev.Cast();
                }
                else if (dev != null && dev.CanUse())
                {
                    dev.Cast();
                }
                else if (dev == null && sa.CanUse())
                {
                    sa.Cast();
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= 50 && cl.CanUse())
                {
                    cl.Cast();
                }
                else if (applyAtNextAA == null && !MH.TwoHanded && Resource >= 50 && hs.CanUse())
                {
                    hs.Cast();
                }
            }

            CheckAAs();
        }

        #endregion

        public List<Action> Spells;
        public List<KeyValuePair<Action, double>> SpellsDPR;
        public List<KeyValuePair<Action, double>> SpellsDmg;
        int DPRAtRage = -1;
        double DPRAtAP = -1;

        // TODO : SweepingStrikes
        public void CalcDPR()
        {
            SpellsDmg = new List<KeyValuePair<Action, double>>();
            SpellsDPR = new List<KeyValuePair<Action, double>>();
            DPRAtRage = Resource;
            DPRAtAP = AP;
            double aaDmg = AvgAADmg();
            double aaRage = AvgAARage(aaDmg);

            double dmg;
            foreach (Action a in Spells)
            {
                if (a is Bloodthirst)
                {
                    dmg = 0.45 * AP;
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / bt.Cost));
                }
                else if (a is Whirlwind)
                {
                    dmg = ((MH.DamageMin + MH.DamageMax) / 2 + Simulation.Normalization(MH) * AP / 14
                        + (Program.version == Version.Vanilla || !DualWielding ? 0 : ((OH.DamageMin + OH.DamageMax) / 2 + Simulation.Normalization(OH) * AP / 14) * 0.5 * (1 + 0.05 * GetTalentPoints("DWS"))))
                    * Math.Min(4, Sim.NbTargets);
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / ww.Cost));
                }
                else if (a is MortalStrike)
                {
                    dmg = (MH.DamageMin + MH.DamageMax) / 2 + Simulation.Normalization(MH) * AP / 14 + MortalStrike.BASE_DMG;
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / ms.Cost));
                }
                else if (a is Execute && Sim.Boss.LifePct <= 0.2)
                {
                    dmg = Execute.BASE_DMG + Execute.DMG_BY_RAGE * (Resource - exec.Cost);
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / Resource));
                }
                else if (a is HeroicStrike)
                {
                    dmg = (MH.DamageMin + MH.DamageMax) / 2 + MH.Speed * AP / 14 + HeroicStrike.BONUS_DMG - aaDmg;
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / (hs.Cost + aaRage)));
                }
                else if (a is Cleave)
                {
                    dmg = (MH.DamageMin + MH.DamageMax) / 2 + MH.Speed * AP / 14 + HeroicStrike.BONUS_DMG - aaDmg
                    * Math.Min(2, Sim.NbTargets);
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / (cl.Cost + aaRage)));
                }
                else if (a is Slam)
                {
                    dmg = (MH.DamageMin + MH.DamageMax) / 2 + MH.Speed * AP / 14 + Slam.BASE_DMG - (1 - ((mh.LockedUntil - Sim.CurrentTime) / mh.CurrentSpeed())) * aaDmg;
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / (slam.Cost + (1 - ((mh.LockedUntil - Sim.CurrentTime) / mh.CurrentSpeed())) * aaRage)));
                }
                // TODO : Prot spells
            }

            SpellsDmg.Sort((x, y) => y.Value.CompareTo(x.Value));
            SpellsDPR.Sort((x,y) => y.Value.CompareTo(x.Value));

            /*
            foreach (KeyValuePair<Action, double> a in SpellsDPR)
            {
                Program.Log(a.Key + " : " + a.Value);
            }
            */
        }

        public double AvgAADmg()
        {
            return ((MH.DamageMin + MH.DamageMax) / 2 + MH.Speed * AP / 14)
                * 1;    // TODO : properly mitigate using dmg lost from glancing blows (+ crit% lost)
        }

        public double AvgAARage(double avgDmg)
        {
            return Simulation.RageGained(avgDmg, Level, true, false, MH.Speed)
                * 2;    // TODO : properly estimate with Crit, glancing etc.
        }

        public double AngerManagementTick { get; set; }

        public void CheckAngerManagementTick()
        {
            if (Sim.CurrentTime >= AngerManagementTick + 3)
            {
                AngerManagementTick = AngerManagementTick + 3;
                Resource += 1;
                if (Program.logFight)
                {
                    Program.Log(string.Format("{0:N2} : Anger Management ticks ({1}/{2})", Sim.CurrentTime, Resource, MaxResource));
                }
            }
        }

        public override void Reset()
        {
            base.Reset();

            AngerManagementTick = 0;
        }
    }
}

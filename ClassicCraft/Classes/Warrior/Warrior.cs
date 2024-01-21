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
        private Overpower pow = null;
        private QuickStrike qs = null;
        private RagingBlow rb = null;
        private Rend rend = null;
        private BerserkerRage brage = null;

        #region Constructors

        public Warrior(Player p)
            : base(p)
        {
        }

        public Warrior(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Warrior(Simulation s, Races r, int level, Dictionary<Slot, Item> items, Dictionary<string, int> talents, List<Enchantment> buffs, bool tanking, bool facing, List<string> cooldowns, List<string> runes, string prepull)
            : base(s, Classes.Warrior, r, level, items, talents, buffs, tanking, facing, cooldowns, runes, null, prepull)
        {
        }

        #endregion

        #region Talents

        public static Dictionary<string, int> TalentsFromString(string ptal, bool twoHanded = false)
        {
            if (ptal == null || ptal == "")
            {
                if (twoHanded)
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

            var Talents = new Dictionary<string, int>();

            switch (Program.version)
            {
                case Version.SoD:
                case Version.Vanilla:
                    // Arms
                    Talents.Add("IHS", arms.Length > 0 ? (int)Char.GetNumericValue(arms[0]) : 0);
                    Talents.Add("IO", arms.Length > 6 ? (int)Char.GetNumericValue(arms[6]) : 0);
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
                    Talents.Add("IO", arms.Length > 6 ? (int)Char.GetNumericValue(arms[6]) : 0);
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

            return Talents;
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
            if (GetTalentPoints("BT") > 0) bt = new Bloodthirst(this);
            if (GetTalentPoints("MS") > 0) ms = new MortalStrike(this);
            if (GetTalentPoints("SS") > 0) ss = new SweepingStrikes(this);
            if (GetTalentPoints("IS") > 0) slam = new Slam(this);
            rend = new Rend(this);
            brage = new BerserkerRage(this);
            pow = new Overpower(this);

            if (Sim.Tanking)
            {
                sa = new SunderArmor(this);
                rev = new Revenge(this);
                dev = new Devastate(this);
                tc = new Thunderclap(this);
                if(!DualWielding && GetTalentPoints("ShieldSlam") > 0) shslam = new ShieldSlam(this);
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
                else if(Program.version == Version.SoD)
                {
                    qs = new QuickStrike(this);
                    rb = new RagingBlow(this);
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
                /*
                Spells.Add(hs);
                if (Sim.MaxTargets > 1) Spells.Add(cl);
                */
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
            else if(Program.version == Version.SoD)
            {
                if(Tanking)
                {
                    rota = 300; // Tank
                }
                else if(DualWielding)
                {
                    rota = 301; // 2x1H
                }
                else
                {
                    rota = 302; // 2H
                }
            }
            else
            {
                if (Sim.Tanking)
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
            if (br.CanUse() && Resource < 88)
            {
                if(Sim.FightLength - Sim.CurrentTime <= BloodrageBuff.DURATION && Resource <= exec.Cost)
                {
                    br.Cast(this);
                }
                else
                {
                    if (Program.version == Version.SoD && Runes.Contains(ConsumedByRage.NAME))
                    {
                        if (Resource < 80 && !Effects.ContainsKey(ConsumedByRage.NAME))
                        {
                            br.Cast(this);
                        }
                    }
                    else if(Sim.FightLength - Sim.CurrentTime >= br.BaseCD + BloodrageBuff.DURATION)
                    {
                        br.Cast(this);
                    }
                }
            }

            /*
            if (bs.CanUse() && (!Effects.Any(e => e is BattleShoutBuff) || ((BattleShoutBuff)Effects.Where(e => e is BattleShoutBuff).First()).RemainingTime() < GCD))
            {
                bs.Cast(target);
            }
            */

            if (cds != null)
            {
                foreach (Spell cd in cds.Keys)
                {
                    if (cd.CanUse() &&
                        (Sim.FightLength - Sim.CurrentTime <= cds[cd]
                        || Sim.FightLength - Sim.CurrentTime >= cd.BaseCD + cds[cd]))
                    {
                        if (!(cd is MightyRage) || Sim.FightLength - Sim.CurrentTime >= cd.BaseCD + cds[cd] || ((!Tanking || Sim.TankHitRage == 0 || Sim.TankHitEvery == 0) && Resource < exec.Cost))
                        {
                            cd.Cast(Target);
                        }
                    }
                }
            }

            // VANILLA
            if (rota == 0) //BT > WW > HAM + HS + EXEC
            {
                if (Target.LifePct > 0.2)
                {
                    if (Sim.NbTargets > 1 && ww.CanUse())
                    {
                        ww.Cast(Target);
                    }
                    else if (bt.CanUse())
                    {
                        bt.Cast(Target);
                    }
                    else if (ww.CanUse() && Resource >= ww.Cost + bt.Cost && bt.RemainingCD() >= GCD)
                    {
                        ww.Cast(Target);
                    }
                    else if (ham.CanUse() && Resource >= bt.Cost + ww.Cost + hs.Cost && ww.RemainingCD() >= GCD && bt.RemainingCD() >= GCD && bt.RemainingCD() >= GCD && (!Effects.ContainsKey(Flurry.NAME) || ((Flurry)Effects[Flurry.NAME]).CurrentStacks < 3))
                    {
                        ham.Cast(Target);
                    }

                    if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                    {
                        cl.Cast(Target);
                    }
                    else if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
                    {
                        hs.Cast(Target);
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
                        exec.Cast(Target);
                    }
                }
            }
            else if (rota == 1) //SLAM + EXEC
            {
                if (Target.LifePct > 0.2)
                {
                    if (slam.CanUse())
                    {
                        slam.Cast(Target);
                    }
                }
                else
                {
                    if (exec.CanUse())
                    {
                        exec.Cast(Target);
                    }
                }
            }
            else if (rota == 2) //BT > REVENGE > SA + HS
            {
                if (bt.CanUse())
                {
                    bt.Cast(Target);
                }
                else if (rev.CanUse())
                {
                    rev.Cast(Target);
                }
                else if (sa.CanUse())
                {
                    sa.Cast(Target);
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast(Target);
                }
                else if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + sa.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast(Target);
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
                    bt.Cast(Target);
                }
                else if (rev.CanUse())
                {
                    rev.Cast(Target);
                }
                else if (bs.CanUse())
                {
                    bs.Cast(Target);
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast(Target);
                }
                else if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + sa.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast(Target);
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
                    ss.Cast(Target);
                }

                if (ramp != null && (!Effects.ContainsKey(RampageBuff.NAME) || Effects[RampageBuff.NAME].RemainingTime() < GCD_Hasted() * 2) && ramp.CanUse())
                {
                    ramp.Cast(Target);
                }
                else if (ww.CanUse())
                {
                    ww.Cast(Target);
                }
                else if (bt.CanUse())
                {
                    bt.Cast(Target);
                }
                else if (Target.LifePct <= 0.2 && exec.CanUse())
                {
                    exec.Cast(Target);
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast(Target);
                }
                else if (applyAtNextAA == null && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast(Target);
                }
            }
            else if (rota == 11) // Slam > MS > WW + HS + Exec
            {
                if (ss != null && Sim.NbTargets > 1 && ss.CanUse())
                {
                    ss.Cast(Target);
                }


                if (Sim.NbTargets > 1 && ww.CanUse())
                {
                    ww.Cast(Target);
                }
                else if (mh.LockedUntil - Sim.CurrentTime >= mh.CurrentSpeed() * 0.95 && slam.CanUse())
                {
                    slam.Cast(Target);
                }
                else if (ms.CanUse())
                {
                    ms.Cast(Target);
                }
                else if (ww.CanUse())
                {
                    ww.Cast(Target);
                }
                else if (Target.LifePct <= 0.2 && exec.CanUse())
                {
                    exec.Cast(Target);
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast(Target);
                }
                else if (applyAtNextAA == null && Resource >= ms.Cost + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast(Target);
                }
            }
            else if (rota == 111) // MS > WW > HS + Exec
            {
                if (ss != null && Sim.NbTargets > 1 && ss.CanUse())
                {
                    ss.Cast(Target);
                }

                if (Sim.NbTargets > 1 && ww.CanUse())
                {
                    ww.Cast(Target);
                }
                else if (Target.LifePct <= 0.2 && exec.CanUse())
                {
                    exec.Cast(Target);
                }
                else if (ms.CanUse())
                {
                    ms.Cast(Target);
                }
                else if (ww.CanUse())
                {
                    ww.Cast(Target);
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast(Target);
                }
                else if (applyAtNextAA == null && Resource >= ms.Cost + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast(Target);
                }
            }
            // AUTO DPS BY DPR
            else if (rota == 20)
            {
                if (HasGCD() && 
                    (DPRAtAP != AP || (Target.LifePct <= 0.2 && DPRAtRage != Resource) || (slam != null && slam.CanUse())))
                {
                    CalcDPR();
                }

                if (ss != null && Sim.NbTargets > 1 && ss.CanUse())
                {
                    ss.Cast(Target);
                }

                if(HasGCD())
                {
                    if (ramp != null && (!Effects.ContainsKey(RampageBuff.NAME) || Effects[RampageBuff.NAME].RemainingTime() < GCD_Hasted() * 2) && ramp.CanUse())
                    {
                        ramp.Cast(Target);
                    }
                    else
                    {
                        foreach (KeyValuePair<Action, double> a in SpellsDPR)
                        {
                            //Program.Log(a.Key.ToString() + " : " + a.Value);
                            if (a.Key.CanUse() && a.Value > 0)
                            {
                                if (a.Key is Slam)
                                {
                                    if (a.Value == SpellsDPR.Max(v => v.Value))
                                    {
                                        a.Key.Cast(Target);
                                    }
                                }
                                else
                                {
                                    a.Key.Cast(Target);
                                }
                            }
                        }
                    }
                }
                
                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= ww.Cost + cl.Cost && cl.CanUse())
                {
                    cl.Cast(Target);
                }
                else if (DualWielding && applyAtNextAA == null && Resource >= (bt != null ? bt.Cost : 0) + (ms != null ? ms.Cost : 0) + (slam != null ? slam.Cost : 0) + ww.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast(Target);
                }
            }
            // TANKING
            else if(rota == 21)
            {
                if (tc != null && Sim.NbTargets > 1 && tc.CanUse())
                {
                    tc.Cast(Target);
                }
                else if (bt != null && bt.CanUse())
                {
                    bt.Cast(Target);
                }
                else if (ms != null && ms.CanUse())
                {
                    ms.Cast(Target);
                }
                else if (shslam != null && shslam.CanUse())
                {
                    shslam.Cast(Target);
                }
                else if (rev.CanUse())
                {
                    rev.Cast(Target);
                }
                else if (dev != null && dev.CanUse())
                {
                    dev.Cast(Target);
                }
                else if (dev == null && sa.CanUse())
                {
                    sa.Cast(Target);
                }

                if (applyAtNextAA == null && Sim.NbTargets > 1 && Resource >= 50 && cl.CanUse())
                {
                    cl.Cast(Target);
                }
                else if (applyAtNextAA == null && !MH.TwoHanded && Resource >= 50 && hs.CanUse())
                {
                    hs.Cast(Target);
                }
            }
            else if(rota == 301)
            {
                if(rb.CanUse())
                {
                    rb.Cast(Target);
                }
                else if(Resource > 80 && Cooldowns.Contains(Rend.NAME) && !Target.Effects.ContainsKey(Rend.NAME) && rend.CanUse())
                {
                    rend.Cast(Target);
                }
                else if(Resource > 80 && ham.CanUse())
                {
                    ham.Cast(Target);
                }

                if (applyAtNextAA == null && Resource >= 95)
                {
                    if (Sim.NbTargets > 1)
                    {
                        cl.Cast(Target);
                    }
                    else
                    {
                        hs.Cast(Target);
                    }
                }
                else if (Resource < 80)
                {
                    applyAtNextAA = null;
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
                else if (a is Execute && Target.LifePct <= 0.2)
                {
                    dmg = Execute.BASE_DMG + Execute.DMG_BY_RAGE * (Resource - exec.Cost);
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / Resource));
                }
                else if (a is HeroicStrike)
                {
                    dmg = (MH.DamageMin + MH.DamageMax) / 2 + MH.Speed * AP / 14 + HeroicStrike.BONUS_DMG - ((1 - (WindfuryTotem ? 0.2 : 0)) * aaDmg);
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / (hs.Cost + ((1 - (WindfuryTotem ? 0.2 : 0)) * aaRage))));
                }
                else if (a is Cleave)
                {
                    dmg = ((MH.DamageMin + MH.DamageMax) / 2 + MH.Speed * AP / 14 + HeroicStrike.BONUS_DMG - ((1 - (WindfuryTotem ? 0.2 : 0)) * aaDmg))
                    * Math.Min(2, Sim.NbTargets);
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / (cl.Cost + ((1 - (WindfuryTotem ? 0.2 : 0)) * aaRage))));
                }
                else if (a is Slam)
                {
                    dmg = (MH.DamageMin + MH.DamageMax) / 2 + MH.Speed * AP / 14 + Slam.BASE_DMG - (1 - ((mh.LockedUntil - Sim.CurrentTime) / mh.CurrentSpeed())) * aaDmg;
                    SpellsDmg.Add(new KeyValuePair<Action, double>(a, dmg));
                    SpellsDPR.Add(new KeyValuePair<Action, double>(a, dmg / (slam.Cost + (1 - ((mh.LockedUntil - Sim.CurrentTime) / mh.CurrentSpeed())) * aaRage)));
                }
                // TODO : Prot spells
            }

            SpellsDmg.Sort((x,y) => y.Value.CompareTo(x.Value));
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
                //* (1 + CritChance)
                * (1 + (WindfuryTotem ? 0.2 : 0))
                ;
            
            // TODO : properly mitigate using dmg lost from glancing blows (+ crit% lost)
        }

        public double AvgAARage(double avgDmg)
        {
            return Simulation.RageGained(avgDmg, Level, true, false, MH.Speed)
                //* (1 + CritChance)
                * (1 + (WindfuryTotem ? 0.2 : 0))
                * (Runes.Contains("Endless Rage") ? 1.25 : 1);    
            
            // TODO : properly estimate with Crit, glancing etc.
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

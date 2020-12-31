﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Warrior : Player
    {
        private Whirlwind ww;
        private Bloodthirst bt;
        private HeroicStrike hs;
        private Execute exec;
        private Bloodrage br;
        private BattleShout bs;
        private Hamstring ham;
        private Slam slam;
        private Revenge rev;
        private SunderArmor sa;
        private Rampage ramp;
        private MortalStrike ms;

        #region Constructors

        public Warrior(Player p)
            : base(p)
        {
        }

        public Warrior(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Warrior(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null, bool tanking = false)
            : base(s, Classes.Warrior, r, level, items, talents, buffs, tanking)
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
                    Talents.Add("DW", arms.Length > 8 ? (int)Char.GetNumericValue(arms[8]) : 0);
                    Talents.Add("2HS", arms.Length > 9 ? (int)Char.GetNumericValue(arms[9]) : 0);
                    Talents.Add("Impale", arms.Length > 10 ? (int)Char.GetNumericValue(arms[10]) : 0);
                    Talents.Add("Poleaxe", arms.Length > 11 ? (int)Char.GetNumericValue(arms[11]) : 0);
                    Talents.Add("DeathWish", arms.Length > 12 ? (int)Char.GetNumericValue(arms[12]) : 0);
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
            bt = new Bloodthirst(this);
            hs = new HeroicStrike(this);
            ms = new MortalStrike(this);

            if (Sim.Tanking)
            {
                sa = new SunderArmor(this);
                rev = new Revenge(this);
            }
            else
            {
                ham = new Hamstring(this);
                ww = new Whirlwind(this);
                exec = new Execute(this);

                if (GetTalentPoints("IS") > 0)
                {
                    slam = new Slam(this);
                }

                if(Program.version == Version.TBC)
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
                        case "Death Wish": cds.Add(new DeathWish(this), DeathWishBuff.LENGTH); break;
                        case "Juju Flurry": cds.Add(new JujuFlurry(this), JujuFlurryBuff.LENGTH); break;
                        case "Mighty Rage": cds.Add(new MightyRage(this), MightyRageBuff.LENGTH); break;
                        case "Recklessness": cds.Add(new Recklessness(this), RecklessnessBuff.LENGTH); break;
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
                if (Sim.Tanking)
                {
                    rota = 11;   // Fury Tank
                }
                else if (GetTalentPoints("IS") > 0)
                {
                    rota = 12;   // Fury Slam
                }
                else
                {
                    rota = 10;   // Fury
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
                        if(!(cd is MightyRage) || Sim.FightLength - Sim.CurrentTime >= cd.BaseCD + cds[cd] || (!Tanking && Resource < exec.Cost))
                        {
                            cd.Cast();
                        }
                    }
                }
            }

            if (rota == 0) //BT > WW > HAM + HS + EXEC
            {
                if (Sim.Boss.LifePct > 0.2)
                {
                    if (bt.CanUse())
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

                    if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
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

                if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + sa.Cost + hs.Cost && hs.CanUse())
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

                if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + sa.Cost + hs.Cost && hs.CanUse())
                {
                    hs.Cast();
                }
                else if (!MH.TwoHanded && applyAtNextAA != null && Resource < bt.Cost + sa.Cost + hs.Cost)
                {
                    applyAtNextAA = null;
                }
            }
            else if (rota == 10) //RAMPAGE > BT > WW > HAM + HS + EXEC
            {
                if ((!Effects.ContainsKey(RampageBuff.NAME) || Effects[RampageBuff.NAME].RemainingTime() < GCD_Hasted()) && ramp.CanUse())
                {
                    ramp.Cast();
                }

                if (Sim.Boss.LifePct > 0.2)
                {
                    if (bt.CanUse())
                    {
                        bt.Cast();
                    }
                    else if (ww.CanUse() && bt.RemainingCD() >= GCD_Hasted() / 2)
                    {
                        ww.Cast();
                    }
                    else if (ham.CanUse() && Resource >= bt.Cost + ww.Cost + hs.Cost && ww.RemainingCD() >= GCD_Hasted() && bt.RemainingCD() >= GCD_Hasted() && ww.RemainingCD() >= GCD_Hasted() && (!Effects.ContainsKey(Flurry.NAME) || ((Flurry)Effects[Flurry.NAME]).CurrentStacks < 3))
                    {
                        ham.Cast();
                    }

                    if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
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
                    // Exec as filler
                    if (AvgBTDmg() > AvgExecDmg() && bt.CanUse())
                    {
                        bt.Cast();
                    }
                    else if (AvgWWDmg() > AvgExecDmg() && ww.CanUse())
                    {
                        ww.Cast();
                    }
                    else if (exec.CanUse())
                    {
                        exec.Cast();
                    }

                    if (!MH.TwoHanded && applyAtNextAA == null && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
                    {
                        hs.Cast();
                    }
                    else if (!MH.TwoHanded && Resource < bt.Cost + ww.Cost + hs.Cost)
                    {
                        applyAtNextAA = null;
                    }

                    // Spam Exec (seems less efficient)
                    /*
                    if (exec.CanUse())
                    {
                        exec.Cast();
                    }
                    */
                }
            }

            CheckAAs();
        }

        public double AvgBTDmg()
        {
            double res = 1;

            // TODO

            return res;
        }

        public double AvgWWDmg()
        {
            double res = 1;

            // TODO

            return res;
        }

        public double AvgExecDmg()
        {
            double res = 0;

            // TODO

            return res;
        }

        #endregion
    }
}

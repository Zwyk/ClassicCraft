using System;
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

        #region Constructors

        public Warrior(Player p)
            : base(p)
        {
        }

        public Warrior(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Warrior(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, Classes.Warrior, r, level, items, talents, buffs)
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

            Talents = new Dictionary<string, int>();
            // Arms
            Talents.Add("IHS", arms.Length > 0 ? (int)Char.GetNumericValue(arms[0]) : 0);
            Talents.Add("DW", arms.Length > 8 ? (int)Char.GetNumericValue(arms[8]) : 0);
            Talents.Add("2HS", arms.Length > 9 ? (int)Char.GetNumericValue(arms[9]) : 0);
            Talents.Add("Impale", arms.Length > 10 ? (int)Char.GetNumericValue(arms[10]) : 0);
            // Fury
            Talents.Add("Cruelty", fury.Length > 1 ? (int)Char.GetNumericValue(fury[1]) : 0);
            Talents.Add("UW", fury.Length > 3 ? (int)Char.GetNumericValue(fury[3]) : 0);
            Talents.Add("IBS", fury.Length > 7 ? (int)Char.GetNumericValue(fury[7]) : 0);
            Talents.Add("DWS", fury.Length > 8 ? (int)Char.GetNumericValue(fury[8]) : 0);
            Talents.Add("IE", fury.Length > 9 ? (int)Char.GetNumericValue(fury[9]) : 0);
            Talents.Add("IS", fury.Length > 11 ? (int)Char.GetNumericValue(fury[11]) : 0);
            Talents.Add("Flurry", fury.Length > 15 ? (int)Char.GetNumericValue(fury[15]) : 0);
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            ww = new Whirlwind(this);
            bt = new Bloodthirst(this);
            hs = new HeroicStrike(this);
            exec = new Execute(this);
            br = new Bloodrage(this);
            bs = new BattleShout(this);
            ham = new Hamstring(this);
            slam = new Slam(this);

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

            if (GetTalentPoints("IS") > 0)
            {
                rota = 1;
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
                        cd.Cast();
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
                    else if (!Simulation.tank && ww.CanUse() && Resource >= ww.Cost + bt.Cost && bt.RemainingCD() >= GCD)
                    {
                        ww.Cast();
                    }
                    else if (!Simulation.tank && ham.CanUse() && Resource >= bt.Cost + ww.Cost + hs.Cost && ww.RemainingCD() >= GCD && bt.RemainingCD() >= GCD && (!Effects.ContainsKey(Flurry.NAME) || ((Flurry)Effects[Flurry.NAME]).CurrentStacks < 3))
                    {
                        ham.Cast();
                    }

                    if (!MH.TwoHanded && Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
                    {
                        hs.Cast();
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

            CheckAAs();
        }

        #endregion
    }
}

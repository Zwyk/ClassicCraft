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
            base.Rota();

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
                    else if (ww.CanUse() && Resource >= ww.Cost + bt.Cost && bt.RemainingCD() >= GCD)
                    {
                        ww.Cast();
                    }
                    else if (ham.CanUse() && Resource >= bt.Cost + ww.Cost + hs.Cost && ww.RemainingCD() >= GCD && bt.RemainingCD() >= GCD && (!Effects.Any(e => e is Flurry) || ((Flurry)Effects.Where(f => f is Flurry).First()).CurrentStacks < 3))
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

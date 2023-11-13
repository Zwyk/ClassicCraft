using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Warlock : Player
    {
        private ShadowBolt sb;
        private Corruption co;
        private CurseOfAgony ca;
        private LifeTap lt;
        private DrainLife dl;
        private SearingPain sp;
        private ShadowCleave sc;
        private DemonicGrace dg;

        #region Constructors

        public Warlock(Player p)
            : base(p)
        {
        }

        public Warlock(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Warlock(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null, bool tanking = false, bool facing = false, List<string> cooldowns = null, List<string> runes = null)
            : base(s, Classes.Warlock, r, level, items, talents, buffs, tanking, facing, cooldowns, runes)
        {
        }

        #endregion

        #region Talents

        public override void SetupTalents(string ptal)
        {
            if (ptal == null || ptal == "")
            {
                // DS-Ruin
                ptal = "25002-2050300152201-52500051020001";

                // SM-RUin
                ptal = "5500203412201005--52500051020001";
            }

            string[] talents = ptal.Split('-');
            string affli = talents.Length > 0 ? talents[0] : "";
            string demo = talents.Length > 1 ? talents[1] : "";
            string destru = talents.Length > 2 ? talents[2] : "";

            Talents = new Dictionary<string, int>();
            // Affli
            Talents.Add("Sup", affli.Length > 0 ? (int)Char.GetNumericValue(affli[0]) : 0);
            Talents.Add("IC", affli.Length > 1 ? (int)Char.GetNumericValue(affli[1]) : 0);
            Talents.Add("ILT", affli.Length > 4 ? (int)Char.GetNumericValue(affli[4]) : 0);
            Talents.Add("IDL", affli.Length > 5 ? (int)Char.GetNumericValue(affli[5]) : 0);
            Talents.Add("ICA", affli.Length > 6 ? (int)Char.GetNumericValue(affli[6]) : 0);
            Talents.Add("AC", affli.Length > 8 ? (int)Char.GetNumericValue(affli[8]) : 0);
            Talents.Add("NF", affli.Length > 10 ? (int)Char.GetNumericValue(affli[10]) : 0);
            Talents.Add("Siph", affli.Length > 12 ? (int)Char.GetNumericValue(affli[12]) : 0);
            Talents.Add("SM", affli.Length > 15 ? (int)Char.GetNumericValue(affli[15]) : 0);
            // Demo
            Talents.Add("IHS", demo.Length > 0 ? (int)Char.GetNumericValue(demo[0]) : 0);
            Talents.Add("DE", demo.Length > 2 ? (int)Char.GetNumericValue(demo[2]) : 0);
            Talents.Add("DS", demo.Length > 12 ? (int)Char.GetNumericValue(demo[12]) : 0);
            Talents.Add("IF", demo.Length > 13 ? (int)Char.GetNumericValue(demo[13]) : 0);
            Talents.Add("MD", demo.Length > 14 ? (int)Char.GetNumericValue(demo[14]) : 0);
            Talents.Add("SL", demo.Length > 15 ? (int)Char.GetNumericValue(demo[15]) : 0);
            // Destru
            Talents.Add("ISB", destru.Length > 0 ? (int)Char.GetNumericValue(destru[0]) : 0);
            Talents.Add("Cata", destru.Length > 1 ? (int)Char.GetNumericValue(destru[1]) : 0);
            Talents.Add("Bane", destru.Length > 2 ? (int)Char.GetNumericValue(destru[2]) : 0);
            Talents.Add("Deva", destru.Length > 6 ? (int)Char.GetNumericValue(destru[6]) : 0);
            Talents.Add("Shadowburn", destru.Length > 7 ? (int)Char.GetNumericValue(destru[7]) : 0);
            Talents.Add("ISP", destru.Length > 10 ? (int)Char.GetNumericValue(destru[10]) : 0);
            Talents.Add("Ruin", destru.Length > 13 ? (int)Char.GetNumericValue(destru[13]) : 0);
            Talents.Add("Emberstorm", destru.Length > 14 ? (int)Char.GetNumericValue(destru[14]) : 0);
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            Mana = MaxMana;

            if(Runes.Contains("Demonic Tactics"))
            {
                SpellCritChance += 0.1;
            }

            if (Tanking && Runes.Contains("Metamorphosis"))
            {
                Form = Forms.Metamorphosis;
                dl = Runes.Contains("Master Channeler") ? new DrainLife(this) : null;
                sp = new SearingPain(this);
                sc = new ShadowCleave(this);
                if (Runes.Contains("Demonic Grace")) dg = new DemonicGrace(this);
            }
            else
            {
                sb = new ShadowBolt(this);
            }

            co = Cooldowns.Contains("Corruption") ? new Corruption(this) : null;
            ca = Cooldowns.Contains("Curse of Agony") ? new CurseOfAgony(this) : null;
            lt = new LifeTap(this);
        }

        public override void Rota()
        {
            if (Tanking)
            {
                if(dg != null && dg.CanUse())
                {
                    dg.Cast();
                }
                if(Sim.NbTargets > 1 && sb.CanUse())
                {
                    sc.Cast();
                }
                if (ca != null && ca.CanUse() && Sim.TimeLeft >= CurseOfAgonyDoT.DURATION / 2 && !Sim.Boss.Effects.ContainsKey(CurseOfAgonyDoT.NAME))
                {
                    ca.Cast();
                }
                if (co != null && co.CanUse() && Sim.TimeLeft >= co.CastTime + CorruptionDoT.DURATION(Level) / 2 && (!Sim.Boss.Effects.ContainsKey(CorruptionDoT.NAME) || Sim.Boss.Effects[CorruptionDoT.NAME].RemainingTime() < co.CastTime))
                {
                    co.Cast();
                }
                if (dl != null && dl.CanUse() && Sim.TimeLeft >= DrainLifeDoT.DURATION / 2 && !Sim.Boss.Effects.ContainsKey(DrainLifeDoT.NAME) || Sim.Boss.Effects[DrainLifeDoT.NAME].RemainingTime() < ca.CastTime)
                {
                    dl.Cast();
                }
                if (sp.CanUse())
                {
                    sp.Cast();
                }
                if (MaxMana - Mana >= lt.ManaGain() && lt.CanUse())
                {
                    lt.Cast();
                }

                CheckAAs();
            }
            else
            {
                if (rota == 0) // SB + LT (+ Corr)
                {
                    if (casting == null)
                    {
                        if (co != null && co.CanUse() && (!Sim.Boss.Effects.ContainsKey(CorruptionDoT.NAME) || Sim.Boss.Effects[CorruptionDoT.NAME].RemainingTime() < co.CastTime))
                        {
                            co.Cast();
                        }
                        if (ca != null && ca.CanUse() && (!Sim.Boss.Effects.ContainsKey(CurseOfAgonyDoT.NAME) || Sim.Boss.Effects[CurseOfAgonyDoT.NAME].RemainingTime() < ca.CastTime))
                        {
                            ca.Cast();
                        }
                        if (sb.CanUse()
                            && (co == null || (Sim.Boss.Effects.ContainsKey(CorruptionDoT.NAME) && Sim.Boss.Effects[CorruptionDoT.NAME].RemainingTime() > sb.CastTimeWithGCD + co.CastTime))
                            && (ca == null || (Sim.Boss.Effects.ContainsKey(CurseOfAgonyDoT.NAME) && Sim.Boss.Effects[CurseOfAgonyDoT.NAME].RemainingTime() > sb.CastTimeWithGCD + ca.CastTime))
                            )
                        {
                            sb.Cast();
                        }
                        if (MaxMana - Mana >= lt.ManaGain() && lt.CanUse())
                        {
                            lt.Cast();
                        }
                    }
                }
            }
        }

        #endregion
    }
}

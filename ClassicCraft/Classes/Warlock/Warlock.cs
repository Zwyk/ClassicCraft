﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Warlock : Player
    {
        private ShadowBolt sb = null;
        private Corruption co = null;
        private CurseOfAgony ca = null;
        private LifeTap lt = null;
        private DrainLife dl = null;
        private SearingPain sp = null;
        private ShadowCleave sc = null;
        private DemonicGrace dg = null;
        private Shadowburn sburn = null;

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

            Talents = new Dictionary<string, int>
            {
                // Affli
                { "Suppr", affli.Length > 0 ? (int)Char.GetNumericValue(affli[0]) : 0 },
                { "IC", affli.Length > 1 ? (int)Char.GetNumericValue(affli[1]) : 0 },
                { "ILT", affli.Length > 4 ? (int)Char.GetNumericValue(affli[4]) : 0 },
                { "IDL", affli.Length > 5 ? (int)Char.GetNumericValue(affli[5]) : 0 },
                { "ICA", affli.Length > 6 ? (int)Char.GetNumericValue(affli[6]) : 0 },
                { "AC", affli.Length > 8 ? (int)Char.GetNumericValue(affli[8]) : 0 },
                { "NF", affli.Length > 10 ? (int)Char.GetNumericValue(affli[10]) : 0 },
                { "Siph", affli.Length > 12 ? (int)Char.GetNumericValue(affli[12]) : 0 },
                { "SM", affli.Length > 15 ? (int)Char.GetNumericValue(affli[15]) : 0 },
                // Demo
                { "IHS", demo.Length > 0 ? (int)Char.GetNumericValue(demo[0]) : 0 },
                { "DE", demo.Length > 2 ? (int)Char.GetNumericValue(demo[2]) : 0 },
                { "DS", demo.Length > 12 ? (int)Char.GetNumericValue(demo[12]) : 0 },
                { "IF", demo.Length > 13 ? (int)Char.GetNumericValue(demo[13]) : 0 },
                { "MD", demo.Length > 14 ? (int)Char.GetNumericValue(demo[14]) : 0 },
                { "SL", demo.Length > 15 ? (int)Char.GetNumericValue(demo[15]) : 0 },
                // Destru
                { "ISB", destru.Length > 0 ? (int)Char.GetNumericValue(destru[0]) : 0 },
                { "Cata", destru.Length > 1 ? (int)Char.GetNumericValue(destru[1]) : 0 },
                { "Bane", destru.Length > 2 ? (int)Char.GetNumericValue(destru[2]) : 0 },
                { "Deva", destru.Length > 6 ? (int)Char.GetNumericValue(destru[6]) : 0 },
                { "Shadowburn", destru.Length > 7 ? (int)Char.GetNumericValue(destru[7]) : 0 },
                { "ISP", destru.Length > 10 ? (int)Char.GetNumericValue(destru[10]) : 0 },
                { "Ruin", destru.Length > 13 ? (int)Char.GetNumericValue(destru[13]) : 0 },
                { "Emberstorm", destru.Length > 14 ? (int)Char.GetNumericValue(destru[14]) : 0 }
            };
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            Mana = MaxMana;

            if (Tanking && Runes.Contains("Metamorphosis"))
            {
                Form = Forms.Metamorphosis;
                dl = (Runes.Contains("Master Channeler") && Cooldowns.Contains("Drain Life")) ? new DrainLife(this) : null;
                sp = new SearingPain(this);
                sc = new ShadowCleave(this);
                dg = Runes.Contains("Demonic Grace") ? new DemonicGrace(this) : null;
            }
            else
            {
                sb = new ShadowBolt(this);
            }

            co = Cooldowns.Contains("Corruption") ? new Corruption(this) : null;
            ca = Cooldowns.Contains("Curse of Agony") ? new CurseOfAgony(this) : null;
            lt = new LifeTap(this);
            sburn = (GetTalentPoints("Shadowburn") > 0 && Cooldowns.Contains("Shadowburn")) ? new Shadowburn(this) : null;
        }

        public override void Rota()
        {
            if (Tanking)
            {
                if(casting == null)
                {
                    if (dg != null && dg.CanUse())
                    {
                        dg.Cast(Target);
                    }
                    foreach(Boss b in Sim.Boss)
                    {
                        if (ca != null && ca.CanUse() && Sim.TimeLeft >= CurseOfAgonyDoT.DURATION / 2 && !b.Effects.ContainsKey(CurseOfAgonyDoT.NAME))
                        {
                            ca.Cast(b);
                        }
                        if (co != null && co.CanUse() && Sim.TimeLeft >= co.CastTime + CorruptionDoT.DURATION(Level) / 2 && (!b.Effects.ContainsKey(CorruptionDoT.NAME) || b.Effects[CorruptionDoT.NAME].RemainingTime() < co.CastTime))
                        {
                            co.Cast(b);
                        }
                        if (dl != null && dl.CanUse() && Sim.TimeLeft >= DrainLifeDoT.DURATION / 2 && !b.Effects.ContainsKey(DrainLifeDoT.NAME))
                        {
                            dl.Cast(b);
                        }
                    }
                    if (Sim.NbTargets > 1 && sc.CanUse())
                    {
                        sc.Cast(Target);
                    }
                    if (sburn != null && sburn.CanUse())
                    {
                        sburn.Cast(Target);
                    }
                    if (sp.CanUse())
                    {
                        sp.Cast(Target);
                    }
                    if (MaxMana - Mana >= lt.ManaGain() && lt.CanUse())
                    {
                        lt.Cast(Target);
                    }
                }

                CheckAAs();
            }
            else
            {
                if (rota == 0) // SB + LT (+ Corr)
                {
                    if (casting == null)
                    {
                        if (co != null && co.CanUse() && (!Target.Effects.ContainsKey(CorruptionDoT.NAME) || Target.Effects[CorruptionDoT.NAME].RemainingTime() < co.CastTime))
                        {
                            co.Cast(Target);
                        }
                        if (ca != null && ca.CanUse() && (!Target.Effects.ContainsKey(CurseOfAgonyDoT.NAME) || Target.Effects[CurseOfAgonyDoT.NAME].RemainingTime() < ca.CastTime))
                        {
                            ca.Cast(Target);
                        }
                        if (sb.CanUse()
                            && (co == null || (Target.Effects.ContainsKey(CorruptionDoT.NAME) && Target.Effects[CorruptionDoT.NAME].RemainingTime() > sb.CastTimeWithGCD + co.CastTime))
                            && (ca == null || (Target.Effects.ContainsKey(CurseOfAgonyDoT.NAME) && Target.Effects[CurseOfAgonyDoT.NAME].RemainingTime() > sb.CastTimeWithGCD + ca.CastTime))
                            )
                        {
                            sb.Cast(Target);
                        }
                        if (MaxMana - Mana >= lt.ManaGain() && lt.CanUse())
                        {
                            lt.Cast(Target);
                        }
                    }
                }
            }
        }

        #endregion
    }
}

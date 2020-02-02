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
        private LifeTap lt;

        #region Constructors

        public Warlock(Player p)
            : base(p)
        {
        }

        public Warlock(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Warlock(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, Classes.Warlock, r, level, items, talents, buffs)
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
            Talents.Add("ICA", affli.Length > 6 ? (int)Char.GetNumericValue(affli[6]) : 0);
            Talents.Add("AC", affli.Length > 8 ? (int)Char.GetNumericValue(affli[8]) : 0);
            Talents.Add("NF", affli.Length > 10 ? (int)Char.GetNumericValue(affli[10]) : 0);
            Talents.Add("Siph", affli.Length > 12 ? (int)Char.GetNumericValue(affli[12]) : 0);
            Talents.Add("SM", affli.Length > 15 ? (int)Char.GetNumericValue(affli[15]) : 0);
            // Demo
            Talents.Add("IHS", demo.Length > 0 ? (int)Char.GetNumericValue(demo[0]) : 0);
            Talents.Add("DE", demo.Length > 2 ? (int)Char.GetNumericValue(demo[2]) : 0);
            Talents.Add("DS", demo.Length > 12 ? (int)Char.GetNumericValue(demo[12]) : 0);
            // Destru
            Talents.Add("ISB", destru.Length > 0 ? (int)Char.GetNumericValue(destru[0]) : 0);
            Talents.Add("Cata", destru.Length > 1 ? (int)Char.GetNumericValue(destru[1]) : 0);
            Talents.Add("Bane", destru.Length > 2 ? (int)Char.GetNumericValue(destru[2]) : 0);
            Talents.Add("Deva", destru.Length > 6 ? (int)Char.GetNumericValue(destru[6]) : 0);
            Talents.Add("Shadowburn", destru.Length > 7 ? (int)Char.GetNumericValue(destru[7]) : 0);
            Talents.Add("Ruin", destru.Length > 13 ? (int)Char.GetNumericValue(destru[13]) : 0);
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            Mana = MaxMana;

            sb = new ShadowBolt(this);
            co = new Corruption(this);
            lt = new LifeTap(this);
        }

        public override void Rota()
        {
            if (rota == 0) // SB (+ Corr) (+ Agony)
            {
                if(casting == null)
                {
                    if(co.CanUse() && (!Sim.Boss.Effects.Any(e => e is CorruptionDoT) || Sim.Boss.Effects.First(e => e is CorruptionDoT).RemainingTime() < co.CastTime))
                    {
                        co.Cast();
                    }
                    if(sb.CanUse() && Sim.Boss.Effects.Any(e => e is CorruptionDoT) && Sim.Boss.Effects.First(e => e is CorruptionDoT).RemainingTime() > sb.CastTimeWithGCD + co.CastTime)
                    {
                        sb.Cast();
                    }
                    if(MaxMana - Mana >= lt.ManaGain() && lt.CanUse())
                    {
                        lt.Cast();
                    }
                }
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Priest : Player
    {
        public InnerFocus inner;
        private MindBlast mb;
        private MindFlay mf;
        private DevouringPlague dp;
        private SWP swp;

        public Priest(Player p)
            : base(p)
        {
        }

        public Priest(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Priest(Simulation s = null, Races r = Races.Undead, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null, bool tanking = false, bool facing = false)
            : base(s, Classes.Priest, r, level, items, talents, buffs, tanking, facing)
        {
        }

        #region Talents

        public override void SetupTalents(string ptal)
        {
            if (ptal == null || ptal == "")
            {
                ptal = "05500013--5032504103501251";
            }

            string[] talents = ptal.Split('-');
            string disc = talents.Length > 0 ? talents[0] : "";
            string holy = talents.Length > 1 ? talents[1] : "";
            string shadow = talents.Length > 2 ? talents[2] : "";

            Talents = new Dictionary<string, int>();
            // Disc
            Talents.Add("WS", disc.Length > 1 ? (int)Char.GetNumericValue(disc[1]) : 0);
            Talents.Add("SR", disc.Length > 2 ? (int)Char.GetNumericValue(disc[2]) : 0);
            Talents.Add("IF", disc.Length > 6 ? (int)Char.GetNumericValue(disc[6]) : 0);
            Talents.Add("Med", disc.Length > 7 ? (int)Char.GetNumericValue(disc[7]) : 0);
            // Shadow
            Talents.Add("SA", shadow.Length > 2 ? (int)Char.GetNumericValue(shadow[2]) : 0);
            Talents.Add("ISWP", shadow.Length > 3 ? (int)Char.GetNumericValue(shadow[3]) : 0);
            Talents.Add("SF", shadow.Length > 4 ? (int)Char.GetNumericValue(shadow[4]) : 0);
            Talents.Add("IMB", shadow.Length > 6 ? (int)Char.GetNumericValue(shadow[6]) : 0);
            Talents.Add("MF", shadow.Length > 7 ? (int)Char.GetNumericValue(shadow[7]) : 0);
            Talents.Add("Dark", shadow.Length > 14 ? (int)Char.GetNumericValue(shadow[14]) : 0);
            Talents.Add("Form", shadow.Length > 15 ? (int)Char.GetNumericValue(shadow[15]) : 0);
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            Mana = MaxMana;

            inner = new InnerFocus(this);
            mb = new MindBlast(this);
            mf = new MindFlay(this);
            swp = new SWP(this);
            if (Race == Races.Undead)
            {
                dp = new DevouringPlague(this);
            }

            ranged = new AutoAttack(this, Ranged, false, AutoAttack.AAType.Wand);
        }

        public static bool USE_WAND = true;
        public static bool USE_POTS = true;

        public bool wanding = false;

        public override void Rota()
        {
            if (rota == 0) // MB > MF + SWP + DP + IF
            {
                if (casting == null)
                {
                    if(USE_POTS)
                    {
                        if (pot.CanUse() && MaxMana - Mana > ManaPotion.MAX)
                        {
                            pot.Cast();
                        }
                        if (rune.CanUse() && MaxMana - Mana > ManaRune.MANA_MAX)
                        {
                            rune.Cast();
                        }
                    }

                    if(USE_WAND)
                    {
                        if (wanding)
                        {
                            if (ranged.Available())
                            {
                                ranged.Cast();
                            }
                            if (Mana / mf.Cost * mf.CastTime >= Sim.TimeLeft || Mana >= 0.98 * MaxMana)
                            {
                                wanding = false;
                            }
                        }
                        else if (Mana < swp.Cost && Mana / mf.Cost * mf.CastTime < Sim.TimeLeft)
                        {
                            ranged.WandFirstCast();
                            wanding = true;
                        }
                    }

                    if (Sim.TimeLeft > SWPDoT.DURATION / 2 && swp.CanUse() && !Sim.Boss.Effects.ContainsKey(SWPDoT.NAME))
                    {
                        swp.Cast();
                        wanding = false;
                    }
                    if (Race == Races.Undead && Sim.TimeLeft > DevouringPlagueDoT.DURATION / 2 && dp.CanUse())
                    {
                        if (inner.CanUse())
                        {
                            inner.Cast();
                        }
                        dp.Cast();
                        wanding = false;
                    }
                    if (!wanding)
                    {
                        if ((USE_POTS || Mana / (mf.Cost * 2 + mb.Cost) * (mf.CastTime * 2 + mb.CastTime) >= Sim.TimeLeft) && mb.CanUse())
                        {
                            if (Race != Races.Undead && inner.CanUse())
                            {
                                inner.Cast();
                            }
                            mb.Cast();
                        }
                        if (mf.CanUse())
                        {
                            mf.Cast();
                        }
                    }
                }
            }
        }

        #endregion
    }
}

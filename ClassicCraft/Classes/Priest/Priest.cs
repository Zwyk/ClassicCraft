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

        public Priest(Simulation s, Races r, int level, Dictionary<Slot, Item> items, Dictionary<string, int> talents, List<Enchantment> buffs, bool tanking, bool facing, List<string> cooldowns, List<string> runes, string prepull)
            : base(s, Classes.Priest, r, level, items, talents, buffs, tanking, facing, cooldowns, runes, null, prepull)
        {
        }

        #region Talents

        public static Dictionary<string, int> TalentsFromString(string ptal)
        {
            if (ptal == null || ptal == "")
            {
                ptal = "05500013--5032504103501251";
            }

            string[] talents = ptal.Split('-');
            string disc = talents.Length > 0 ? talents[0] : "";
            string holy = talents.Length > 1 ? talents[1] : "";
            string shadow = talents.Length > 2 ? talents[2] : "";

            var Talents = new Dictionary<string, int>
            {
                // Disc
                { "WS", disc.Length > 1 ? (int)Char.GetNumericValue(disc[1]) : 0 },
                { "SR", disc.Length > 2 ? (int)Char.GetNumericValue(disc[2]) : 0 },
                { "IF", disc.Length > 6 ? (int)Char.GetNumericValue(disc[6]) : 0 },
                { "Med", disc.Length > 7 ? (int)Char.GetNumericValue(disc[7]) : 0 },
                { "MS", disc.Length > 11 ? (int)Char.GetNumericValue(disc[11]) : 0 },
                // Shadow
                { "SA", shadow.Length > 2 ? (int)Char.GetNumericValue(shadow[2]) : 0 },
                { "ISWP", shadow.Length > 3 ? (int)Char.GetNumericValue(shadow[3]) : 0 },
                { "SF", shadow.Length > 4 ? (int)Char.GetNumericValue(shadow[4]) : 0 },
                { "IMB", shadow.Length > 6 ? (int)Char.GetNumericValue(shadow[6]) : 0 },
                { "MF", shadow.Length > 7 ? (int)Char.GetNumericValue(shadow[7]) : 0 },
                { "Dark", shadow.Length > 14 ? (int)Char.GetNumericValue(shadow[14]) : 0 },
                { "Form", shadow.Length > 15 ? (int)Char.GetNumericValue(shadow[15]) : 0 }
            };

            return Talents;
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
        public static bool USE_RUNES = false;

        public bool wanding = false;

        public override void Rota()
        {
            if (rota == 0) // MB > MF + SWP + DP + IF
            {
                if (casting == null)
                {
                    if(USE_POTS)
                    {
                        if (pot.CanUse() && MaxMana - Mana > ManaPotion.MAX(Level))
                        {
                            pot.Cast(Target);
                        }
                    }
                    if (USE_RUNES)
                    {
                        if (rune.CanUse() && MaxMana - Mana > ManaRune.MANA_MAX)
                        {
                            rune.Cast(Target);
                        }
                    }

                    if (USE_WAND)
                    {
                        if (wanding)
                        {
                            if (ranged.Available())
                            {
                                ranged.Cast(Target);
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

                    if (Sim.TimeLeft > SWPDoT.DURATION / 2 && swp.CanUse() && !Target.Effects.ContainsKey(SWPDoT.NAME))
                    {
                        swp.Cast(Target);
                        wanding = false;
                    }
                    if (Race == Races.Undead && Sim.TimeLeft > DevouringPlagueDoT.DURATION / 2 && dp.CanUse())
                    {
                        if (inner.CanUse())
                        {
                            inner.Cast(Target);
                        }
                        dp.Cast(Target);
                        wanding = false;
                    }
                    if (!wanding)
                    {
                        if ((USE_POTS || Mana / (mf.Cost * 2 + mb.Cost) * (mf.CastTime * 2 + mb.CastTime) >= Sim.TimeLeft) && mb.CanUse())
                        {
                            if (Race != Races.Undead && inner.CanUse())
                            {
                                inner.Cast(Target);
                            }
                            mb.Cast(Target);
                        }
                        if (mf.CanUse())
                        {
                            mf.Cast(Target);
                        }
                    }
                }
            }
        }

        #endregion
    }
}

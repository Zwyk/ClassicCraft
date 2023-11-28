using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Druid : Player
    {
        private Shred shred;
        private FerociousBite fb;
        private Shift shift;
        private Innervate innerv;

        private Maul maul;
        private Swipe swipe;

        private RuneOfMeta rom = null;

        #region Constructors

        public Druid(Player p)
            : base(p)
        {
        }

        public Druid(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Druid(Simulation s, Races r, int level, Dictionary<Slot, Item> items, Dictionary<string, int> talents, List<Enchantment> buffs, bool tanking, bool facing, List<string> cooldowns, List<string> runes)
            : base(s, Classes.Druid, r, level, items, talents, buffs, tanking, facing, cooldowns, runes, null)
        {
        }

        #endregion

        #region Talents

        public static Dictionary<string, int> TalentsFromString(string ptal)
        {
            if (ptal == null || ptal == "")
            {
                // DPS Feral 014005301-5500021323202151-05
                ptal = "014005301-5500021323202151-05";
            }

            string[] talents = ptal.Split('-');
            string balance = talents.Length > 0 ? talents[0] : "";
            string feral = talents.Length > 1 ? talents[1] : "";
            string resto = talents.Length > 2 ? talents[2] : "";

            var Talents = new Dictionary<string, int>
            {
                // Balance
                { "NW", balance.Length > 5 ? (int)Char.GetNumericValue(balance[5]) : 0 },
                { "NS", balance.Length > 6 ? (int)Char.GetNumericValue(balance[6]) : 0 },
                { "OC", balance.Length > 8 ? (int)Char.GetNumericValue(balance[8]) : 0 },
                // Feral
                { "Fero", feral.Length > 0 ? (int)Char.GetNumericValue(feral[0]) : 0 },
                { "FA", feral.Length > 1 ? (int)Char.GetNumericValue(feral[1]) : 0 },
                { "FI", feral.Length > 2 ? (int)Char.GetNumericValue(feral[2]) : 0 },
                { "SC", feral.Length > 7 ? (int)Char.GetNumericValue(feral[7]) : 0 },
                { "IS", feral.Length > 8 ? (int)Char.GetNumericValue(feral[8]) : 0 },
                { "PS", feral.Length > 9 ? (int)Char.GetNumericValue(feral[9]) : 0 },
                { "BF", feral.Length > 10 ? (int)Char.GetNumericValue(feral[10]) : 0 },
                { "PF", feral.Length > 11 ? (int)Char.GetNumericValue(feral[11]) : 0 },
                { "SF", feral.Length > 12 ? (int)Char.GetNumericValue(feral[12]) : 0 },
                { "HW", feral.Length > 14 ? (int)Char.GetNumericValue(feral[14]) : 0 },
                // Resto
                { "Furor", resto.Length > 1 ? (int)Char.GetNumericValue(resto[1]) : 0 }
            };

            return Talents;
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            if(Tanking && Sim.TankHitRage > 0 && Sim.TankHitEvery > 0)
            {
                maul = new Maul(this);
                swipe = new Swipe(this);
            }
            else
            {
                shred = new Shred(this);
                fb = new FerociousBite(this);
                shift = new Shift(this);
                innerv = new Innervate(this);
            }

            if(Equipment[Slot.Trinket1].Name.ToLower().Equals("rune of metamorphosis") || Equipment[Slot.Trinket2].Name.ToLower().Equals("rune of metamorphosis"))
            {
                rom = new RuneOfMeta(this);
            }

            HasteMod = CalcHaste();
            Mana = MaxMana;

            if (Equipment[Slot.MH].Name.ToLower().Equals("manual crowd pummeler"))
            {
                new MCP(this).Cast(Target);
            }

            if(Tanking && Sim.TankHitRage > 0 && Sim.TankHitEvery > 0)
            {
                Form = Forms.Bear;
            }
            else
            {
                Form = Forms.Cat;
                Resource = MaxResource;
            }
        }

        public static bool USE_POTS = true;

        public override void Rota()
        {
            if (Tanking && Sim.TankHitRage > 0 && Sim.TankHitEvery > 0) // MAUL + SWIPE
            {
                if(maul.CanUse())
                {
                    maul.Cast(Target);
                }
                if(swipe.CanUse() && Resource > maul.Cost + swipe.Cost)
                {
                    swipe.Cast(Target);
                }
            }
            else if (rota == 0) //SHRED + FB + SHIFT + INNERV
            {
                if (Form == Forms.Cat)
                {
                    if (shred.CanUse() && (
                        Combo < 5
                        || Effects.ContainsKey(ClearCasting.NAME)
                        || (Combo > 4 && Resource > fb.Cost + shred.Cost - (20 * (GCDUntil - Sim.CurrentTime) / GCD))
                        ))
                    {
                        shred.Cast(Target);
                    }
                    else if (Combo > 4 && fb.CanUse())
                    {
                        fb.Cast(Target);
                    }
                    else if (Resource < shred.Cost - 20 && shift.CanUse() && (innerv.Available() || Effects.ContainsKey(InnervateBuff.NAME)
                                                                    || (rom != null && rom.Available()) || Effects.ContainsKey(RuneOfMeta.NAME)
                                                                    || ((int)((double)Mana / shift.Cost)) * 4 + 5 >= Sim.FightLength - Sim.CurrentTime
                                                                    || !(SpiritTicking() && Mana + SpiritMPT() < MaxMana)))
                    {
                        Unshift();
                    }
                }

                if (Form == Forms.Humanoid)
                {
                    if (USE_POTS)
                    {
                        if (pot.CanUse() && MaxMana - Mana > ManaPotion.MAX)
                        {
                            pot.Cast(Target);
                        }
                        if (rune.CanUse() && MaxMana - Mana > ManaRune.MANA_MAX)
                        {
                            rune.Cast(Target);
                        }
                    }

                    if (Mana < shift.Cost * 2 && !(Effects.ContainsKey(InnervateBuff.NAME) || Effects.ContainsKey(RuneOfMeta.NAME)))
                    {
                        if (innerv.CanUse())
                        {
                            innerv.Cast(Target);
                        }
                        else if (rom != null && rom.CanUse())
                        {
                            rom.Cast(Target);
                        }
                    }

                    if (shift.CanUse())
                    {
                        shift.Cast(Target);
                    }
                }
            }

            CheckAAs();
        }

        public void Unshift()
        {
            Form = Forms.Humanoid;
            ResetMHSwing();

            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : Unshifted", Sim.CurrentTime));
            }
        }

        #endregion
    }
}

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

        #region Constructors

        public Druid(Player p)
            : base(p)
        {
        }

        public Druid(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Druid(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, Classes.Druid, r, level, items, talents, buffs)
        {
        }

        #endregion

        #region Talents

        public override void SetupTalents(string ptal)
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

            Talents = new Dictionary<string, int>();
            // Balance
            Talents.Add("NW", balance.Length > 5 ? (int)Char.GetNumericValue(balance[5]) : 0);
            Talents.Add("NS", balance.Length > 6 ? (int)Char.GetNumericValue(balance[6]) : 0);
            Talents.Add("OC", balance.Length > 8 ? (int)Char.GetNumericValue(balance[8]) : 0);
            // Feral
            Talents.Add("Fero", feral.Length > 0 ? (int)Char.GetNumericValue(feral[0]) : 0);
            Talents.Add("FA", feral.Length > 1 ? (int)Char.GetNumericValue(feral[1]) : 0);
            Talents.Add("SC", feral.Length > 7 ? (int)Char.GetNumericValue(feral[7]) : 0);
            Talents.Add("IS", feral.Length > 8 ? (int)Char.GetNumericValue(feral[8]) : 0);
            Talents.Add("PS", feral.Length > 9 ? (int)Char.GetNumericValue(feral[9]) : 0);
            Talents.Add("BF", feral.Length > 10 ? (int)Char.GetNumericValue(feral[10]) : 0);
            Talents.Add("SF", feral.Length > 12 ? (int)Char.GetNumericValue(feral[12]) : 0);
            Talents.Add("HW", feral.Length > 14 ? (int)Char.GetNumericValue(feral[14]) : 0);
            // Resto
            Talents.Add("Furor", resto.Length > 1 ? (int)Char.GetNumericValue(resto[1]) : 0);
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            shred = new Shred(this);
            fb = new FerociousBite(this);
            shift = new Shift(this);
            innerv = new Innervate(this);

            HasteMod = CalcHaste();
            Resource = MaxResource;
            Mana = MaxMana;

            if (Equipment[Slot.MH].Name == "Manual Crowd Pummeler")
            {
                new MCP(this).Cast();
            }
            Form = Forms.Cat;
        }

        public override void Rota()
        {
            if (rota == 0) //SHRED + FB + SHIFT + INNERV
            {
                if (Form == Forms.Cat)
                {
                    if (shred.CanUse() && (
                        Combo < 5
                        || Effects.Any(e => e is ClearCasting)
                        || (Combo > 4 && Resource > fb.Cost + shred.Cost - (20 * (GCDUntil - Sim.CurrentTime) / GCD))
                        ))
                    {
                        shred.Cast();
                    }
                    else if (Combo > 4 && fb.CanUse())
                    {
                        fb.Cast();
                    }
                    else if (Resource < shred.Cost - 20 && shift.CanUse() && (innerv.Available() || Effects.Any(e => e is InnervateBuff) || ((int)((double)Mana / shift.Cost)) * 4 + 5 >= Sim.FightLength - Sim.CurrentTime || !(ManaTicking() && Mana + MPT() < MaxMana)))
                    {
                        shift.Cast();
                    }
                    else if (Mana < shift.Cost && innerv.CanUse())
                    {
                        Form = Forms.Human;
                        ResetMHSwing();
                        innerv.Cast();
                    }
                }
                else if (Form == Forms.Human && shift.CanUse())
                {
                    Form = Forms.Cat;
                    ResetMHSwing();
                    shift.Cast();
                }
            }

            CheckAAs();
        }

        #endregion
    }
}

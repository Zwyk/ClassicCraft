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
            base.Rota();

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

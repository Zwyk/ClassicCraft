using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rogue : Player
    {
        private Ambush am;
        private Backstab bs;
        private Eviscerate ev;
        private SinisterStrike ss;
        private SliceAndDice sad;
        private AdrenalineRush ar;
        private BladeFlurry bf;

        #region Constructors

        public Rogue(Player p)
            : base(p)
        {
        }

        public Rogue(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Rogue(Simulation s = null, Races r = Races.Orc, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
            : base(s, Classes.Rogue, r, level, items, talents, buffs)
        {
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();

            am = new Ambush(this);
            bs = new Backstab(this);
            ev = new Eviscerate(this);
            ss = new SinisterStrike(this);
            sad = new SliceAndDice(this);
            ar = new AdrenalineRush(this);
            bf = new BladeFlurry(this);

            Stealthed = true;

            HasteMod = CalcHaste();

            Resource = MaxResource;

            if (MH.Type == Weapon.WeaponType.Dagger)
            {
                rota = 1;
            }
        }

        public override void Rota()
        {
            base.Rota();

            double sadleft = 0;
            if (Effects.Any(e => e is SliceAndDiceBuff))
            {
                sadleft = Effects.Where(e => e is SliceAndDiceBuff).First().RemainingTime();
            }

            if (sadleft > 0)
            {
                if (bf.CanUse())
                {
                    bf.Cast();
                }
                if (ar.CanUse())
                {
                    ar.Cast();
                }
            }

            if (rota == 0) // SS + EV
            {
                if (Sim.FightLength - Sim.CurrentTime > SliceAndDiceBuff.DurationCalc(this) && Combo > 0 && sadleft == 0 && sad.CanUse())
                {
                    sad.Cast();
                }
                else if (Combo > 4)
                {
                    if (Sim.FightLength - Sim.CurrentTime > SliceAndDiceBuff.DurationCalc(this) && sadleft < 10)
                    {
                        if (Resource >= 80 && sad.CanUse())
                        {
                            sad.Cast();
                        }
                    }
                    else if (ev.CanUse())
                    {
                        ev.Cast();
                    }
                }
                else if (ss.CanUse())
                {
                    ss.Cast();
                }
            }
            else if (rota == 1) // BS + EV
            {
                if (Sim.FightLength - Sim.CurrentTime > SliceAndDiceBuff.DurationCalc(this) && Combo > 1 && sadleft == 0 && sad.CanUse())
                {
                    sad.Cast();
                }
                else if (Combo > 4)
                {
                    if (Sim.FightLength - Sim.CurrentTime > SliceAndDiceBuff.DurationCalc(this) && sadleft < 10)
                    {
                        if (Resource >= 80 && sad.CanUse())
                        {
                            sad.Cast();
                        }
                    }
                    else if (ev.CanUse())
                    {
                        ev.Cast();
                    }
                }
                else if (bs.CanUse())
                {
                    bs.Cast();
                }
            }

            CheckAAs();
        }

        #endregion
    }
}

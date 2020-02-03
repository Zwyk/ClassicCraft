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

        #region Talents

        public override void SetupTalents(string ptal)
        {
            if (ptal == null || ptal == "")
            {
                if (MH.Type == Weapon.WeaponType.Dagger)
                {
                    // Combat Daggers
                    ptal = "005303103-3203052020550100201-05";
                }
                else if (MH.Type == Weapon.WeaponType.Fist)
                {
                    // Combat Fists
                    ptal = "005323105-3210052020050105231";
                }
                else
                {
                    // Combat Sword
                    ptal = "005323105-3210052020050150231";
                }
            }

            string[] talents = ptal.Split('-');
            string assass = talents.Length > 0 ? talents[0] : "";
            string combat = talents.Length > 1 ? talents[1] : "";
            string subti = talents.Length > 2 ? talents[2] : "";

            Talents = new Dictionary<string, int>();
            // Assassination
            Talents.Add("IE", assass.Length > 0 ? (int)Char.GetNumericValue(assass[0]) : 0);
            Talents.Add("Malice", assass.Length > 2 ? (int)Char.GetNumericValue(assass[2]) : 0);
            Talents.Add("Ruth", assass.Length > 3 ? (int)Char.GetNumericValue(assass[3]) : 0);
            Talents.Add("Murder", (assass.Length > 4 && (Program.jsonSim.Boss.Type == "Humanoid" || Program.jsonSim.Boss.Type == "Giant" || Program.jsonSim.Boss.Type == "Beast" || Program.jsonSim.Boss.Type == "Dragonkin"))
                 ? (int)Char.GetNumericValue(assass[4]) : 0);
            Talents.Add("ISD", assass.Length > 5 ? (int)Char.GetNumericValue(assass[5]) : 0);
            Talents.Add("RS", assass.Length > 6 ? (int)Char.GetNumericValue(assass[6]) : 0);
            Talents.Add("Letha", assass.Length > 8 ? (int)Char.GetNumericValue(assass[8]) : 0);
            // Combat
            Talents.Add("IG", combat.Length > 0 ? (int)Char.GetNumericValue(combat[0]) : 0);
            Talents.Add("ISS", combat.Length > 1 ? (int)Char.GetNumericValue(combat[1]) : 0);
            Talents.Add("IB", combat.Length > 3 ? (int)Char.GetNumericValue(combat[3]) : 0);
            Talents.Add("Prec", combat.Length > 5 ? (int)Char.GetNumericValue(combat[5]) : 0);
            Talents.Add("DS", combat.Length > 10 ? (int)Char.GetNumericValue(combat[10]) : 0);
            Talents.Add("DWS", combat.Length > 11 ? (int)Char.GetNumericValue(combat[11]) : 0);
            Talents.Add("BF", combat.Length > 13 ? (int)Char.GetNumericValue(combat[13]) : 0);
            Talents.Add("SS", combat.Length > 14 ? (int)Char.GetNumericValue(combat[14]) : 0);
            Talents.Add("FS", combat.Length > 15 ? (int)Char.GetNumericValue(combat[15]) : 0);
            Talents.Add("WE", combat.Length > 16 ? (int)Char.GetNumericValue(combat[16]) : 0);
            Talents.Add("Agg", combat.Length > 17 ? (int)Char.GetNumericValue(combat[17]) : 0);
            Talents.Add("AR", combat.Length > 18 ? (int)Char.GetNumericValue(combat[18]) : 0);
            // Subtlety
            Talents.Add("Oppo", subti.Length > 1 ? (int)Char.GetNumericValue(subti[1]) : 0);
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
            double sadleft = 0;
            if (Effects.ContainsKey(SliceAndDiceBuff.NAME))
            {
                sadleft = Effects[SliceAndDiceBuff.NAME].RemainingTime();
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

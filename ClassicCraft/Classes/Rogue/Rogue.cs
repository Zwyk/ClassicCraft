using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Rogue : Player
    {
        private Ambush am = null;
        private Backstab bs = null;
        private Eviscerate ev = null;
        private SinisterStrike ss = null;
        private SliceAndDice snd = null;
        private Rupture rup = null;
        private Shiv shiv = null;
        private Envenom env = null;

        #region Constructors

        public Rogue(Player p)
            : base(p)
        {
        }

        public Rogue(Simulation s, Player p)
            : base(s, p)
        {
        }

        public Rogue(Simulation s, Races r, int level, Dictionary<Slot, Item> items, Dictionary<string, int> talents, List<Enchantment> buffs, bool tanking, bool facing, List<string> cooldowns, List<string> runes, string prepull)
            : base(s, Classes.Rogue, r, level, items, talents, buffs, tanking, facing, cooldowns, runes, null, prepull)
        {
        }

        #endregion

        #region Talents

        public static Dictionary<string, int> TalentsFromString(string ptal, Weapon.WeaponType mhtype = Weapon.WeaponType.Sword)
        {
            if (ptal == null || ptal == "")
            {
                if (mhtype == Weapon.WeaponType.Dagger)
                {
                    // Combat Daggers
                    ptal = "005303103-3203052020550100201-05";
                }
                else if (mhtype == Weapon.WeaponType.Fist)
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

            var Talents = new Dictionary<string, int>();

            switch(Program.version)
            {
                case Version.Vanilla:
                    // Assassination
                    Talents.Add("IE", assass.Length > 0 ? (int)Char.GetNumericValue(assass[0]) : 0);
                    Talents.Add("Malice", assass.Length > 2 ? (int)Char.GetNumericValue(assass[2]) : 0);
                    Talents.Add("Ruth", assass.Length > 3 ? (int)Char.GetNumericValue(assass[3]) : 0);
                    Talents.Add("Murder", assass.Length > 4 ? (int)Char.GetNumericValue(assass[4]) : 0);
                    Talents.Add("ISD", assass.Length > 5 ? (int)Char.GetNumericValue(assass[5]) : 0);
                    Talents.Add("RS", assass.Length > 6 ? (int)Char.GetNumericValue(assass[6]) : 0);
                    Talents.Add("Letha", assass.Length > 8 ? (int)Char.GetNumericValue(assass[8]) : 0);
                    // Combat
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
                    break;
                case Version.TBC:
                    // Assassination
                    Talents.Add("IE", assass.Length > 0 ? (int)Char.GetNumericValue(assass[0]) : 0);
                    Talents.Add("Malice", assass.Length > 2 ? (int)Char.GetNumericValue(assass[2]) : 0);
                    Talents.Add("Ruth", assass.Length > 3 ? (int)Char.GetNumericValue(assass[3]) : 0);
                    Talents.Add("Murder", (assass.Length > 4 && (Program.jsonSim.Boss.Type == "Humanoid" || Program.jsonSim.Boss.Type == "Giant" || Program.jsonSim.Boss.Type == "Beast" || Program.jsonSim.Boss.Type == "Dragonkin"))
                         ? (int)Char.GetNumericValue(assass[4]) : 0);
                    Talents.Add("PW", assass.Length > 5 ? (int)Char.GetNumericValue(assass[5]) : 0);
                    Talents.Add("RS", assass.Length > 6 ? (int)Char.GetNumericValue(assass[6]) : 0);
                    Talents.Add("Letha", assass.Length > 8 ? (int)Char.GetNumericValue(assass[8]) : 0);
                    Talents.Add("VP", assass.Length > 9 ? (int)Char.GetNumericValue(assass[9]) : 0);
                    Talents.Add("IP", assass.Length > 10 ? (int)Char.GetNumericValue(assass[10]) : 0);
                    // Combat
                    Talents.Add("ISS", combat.Length > 1 ? (int)Char.GetNumericValue(combat[1]) : 0);
                    Talents.Add("ISD", combat.Length > 3 ? (int)Char.GetNumericValue(combat[3]) : 0);
                    Talents.Add("Prec", combat.Length > 5 ? (int)Char.GetNumericValue(combat[5]) : 0);
                    Talents.Add("DS", combat.Length > 10 ? (int)Char.GetNumericValue(combat[10]) : 0);
                    Talents.Add("DWS", combat.Length > 11 ? (int)Char.GetNumericValue(combat[11]) : 0);
                    Talents.Add("Mace", combat.Length > 12 ? (int)Char.GetNumericValue(combat[12]) : 0);
                    Talents.Add("BF", combat.Length > 13 ? (int)Char.GetNumericValue(combat[13]) : 0);
                    Talents.Add("Sword", combat.Length > 14 ? (int)Char.GetNumericValue(combat[14]) : 0);
                    Talents.Add("Fist", combat.Length > 15 ? (int)Char.GetNumericValue(combat[15]) : 0);
                    Talents.Add("WE", combat.Length > 17 ? (int)Char.GetNumericValue(combat[17]) : 0);
                    Talents.Add("Agg", combat.Length > 18 ? (int)Char.GetNumericValue(combat[18]) : 0);
                    Talents.Add("Vitality", combat.Length > 19 ? (int)Char.GetNumericValue(combat[19]) : 0);
                    Talents.Add("AR", combat.Length > 20 ? (int)Char.GetNumericValue(combat[20]) : 0);
                    Talents.Add("CP", combat.Length > 22 ? (int)Char.GetNumericValue(combat[22]) : 0);
                    Talents.Add("SA", combat.Length > 23 ? (int)Char.GetNumericValue(combat[23]) : 0);
                    // Subtlety
                    Talents.Add("Oppo", subti.Length > 1 ? (int)Char.GetNumericValue(subti[1]) : 0);
                    break;
            }

            return Talents;
        }

        #endregion

        #region Rota

        public override void PrepFight()
        {
            base.PrepFight();
            ev = new Eviscerate(this);
            snd = new SliceAndDice(this);
            env = new Envenom(this);
            shiv = new Shiv(this);
            rup = new Rupture(this);

            Stealthed = true;

            HasteMod = CalcHaste();

            Resource = MaxResource;

            if (MH.Type == Weapon.WeaponType.Dagger)
            {
                am = new Ambush(this);
                bs = new Backstab(this);

                rota = 1;
            }
            else
            {
                ss = new SinisterStrike(this);

                rota = 0;
            }

            if (GetTalentPoints("BF") > 0) cds.Add(new BladeFlurry(this), BladeFlurryBuff.LENGTH);
            if (GetTalentPoints("AR") > 0) cds.Add(new AdrenalineRush(this), AdrenalineRushBuff.LENGTH);
            if (Cooldowns != null)
            {
                foreach (string s in Cooldowns)
                {
                    switch (s)
                    {
                        case "Racial":
                            if (Race == Races.Orc)
                            {
                                cds.Add(new BloodFury(this), BloodFuryBuff.LENGTH);
                            }
                            else if (Race == Races.Troll)
                            {
                                cds.Add(new Berserking(this), BerserkingBuff.LENGTH);
                            }
                            break;
                    }
                }
            }
        }

        public override void Rota()
        {
            double sndLeft = 0;
            if (Effects.ContainsKey(SliceAndDiceBuff.NAME))
            {
                sndLeft = Effects[SliceAndDiceBuff.NAME].RemainingTime();
            }

            if (sndLeft > 0)
            {
                if (cds != null)
                {
                    foreach (Skill cd in cds.Keys)
                    {
                        if (cd.CanUse() &&
                            (Sim.FightLength - Sim.CurrentTime <= cds[cd]
                            || Sim.FightLength - Sim.CurrentTime >= cd.BaseCD + cds[cd]))
                        {
                            cd.Cast(Target);
                        }
                    }
                }
            }

            if (rota == 0) // SS + SND + RUPT>EV
            {
                if (Combo > 0 && sndLeft == 0 && snd.CanUse())
                {
                    snd.Cast(Target);
                }
                else if (Combo > 4 && sndLeft < 4 && snd.CanUse() && Sim.FightLength - Sim.CurrentTime > SliceAndDiceBuff.DurationCalc(this))
                {
                    snd.Cast(Target);
                }
                else if (Combo > 4 && rup.CanUse() && Sim.FightLength - Sim.CurrentTime > RuptureDoT.DurationCalc(this) && !Target.Effects.ContainsKey("Rupture"))
                {
                    rup.Cast(Target);
                }
                else if (Combo > 2 && ev.CanUse() && Target.Effects.ContainsKey("Rupture"))
                {
                    ev.Cast(Target);
                }
                else if (ss.CanUse())
                {
                    ss.Cast(Target);
                }
            }
            else if (rota == 1) // BS + EV
            {
                if (Sim.FightLength - Sim.CurrentTime > SliceAndDiceBuff.DurationCalc(this) && Combo > 1 && sndLeft == 0 && snd.CanUse())
                {
                    snd.Cast(Target);
                }
                else if (Combo > 4)
                {
                    if (Sim.FightLength - Sim.CurrentTime > SliceAndDiceBuff.DurationCalc(this) && sndLeft < 10)
                    {
                        if (Resource >= 80 && snd.CanUse())
                        {
                            snd.Cast(Target);
                        }
                    }
                    else if (ev.CanUse())
                    {
                        ev.Cast(Target);
                    }
                }
                else if (bs.CanUse())
                {
                    bs.Cast(Target);
                }
            }

            CheckAAs();
        }

        #endregion
    }
}

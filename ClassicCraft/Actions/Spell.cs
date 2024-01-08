using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Spell : Action
    {
        // Spell SpellType
        public enum SpellType
        {
            Melee,
            Ranged,
            Magical,
        }
        // Spell Melee Interaction
        public enum SMI
        {
            None,
            Reset,
            UseOnNextMHSwing
        }

        public enum RatioType
        {
            None,
            SP,
            AP,
            WeaponMH,
            WeaponOH,
            WeaponDual,
            Shield,
        }

        public enum EnergyType
        {
            None,
            ComboAward,
            ComboSpend,
        }

        //public double TravelSpeed { get; set; }
        public double CastFinish { get; set; }
        public double CastTimeWithGCD { get { return Math.Max(SpellDataInfo.CastTime, SpellDataInfo.GCD ? Player.GCD_Hasted() : 0); } }

        public class SpellData
        {
            public SpellType Type { get; set; }
            public int Cost { get; set; }
            public bool GCD { get; set; }
            public double CastTime { get; set; }
            public SMI MeleeInteraction { get; set; }
            public int MaxTargets { get; set; }
            public double ThreatRatio { get; set; }
            public double BaseThreat { get; set; }
            public EnergyType Energy { get; set; }

            public SpellData(SpellType type, int cost, bool gcd = true, double castTime = 0, SMI meleeInteraction = SMI.None, int maxTargets = 1, double threatRatio = 1, double baseThreat = 0, EnergyType energy = EnergyType.None)
            {
                Type = type;
                Cost = cost;
                GCD = gcd;
                CastTime = castTime;
                MeleeInteraction = meleeInteraction;
                MaxTargets = maxTargets;
                ThreatRatio = threatRatio;
                BaseThreat = baseThreat;
                Energy = energy;
            }
        }

        public SpellData SpellDataInfo { get; set; }

        public SpellType Type { get { return SpellDataInfo.Type; } set { SpellDataInfo.Type = value; } }
        public int Cost { get { return SpellDataInfo.Cost; } set { SpellDataInfo.Cost = value; } }
        public bool AffectedByGCD { get { return SpellDataInfo.GCD; } set { SpellDataInfo.GCD = value; } }
        public double CastTime { get { return SpellDataInfo.CastTime; } set { SpellDataInfo.CastTime = value; } }
        public SMI MeleeInteraction { get { return SpellDataInfo.MeleeInteraction; } set { SpellDataInfo.MeleeInteraction = value; } }
        public int MaxTargets { get { return SpellDataInfo.MaxTargets; } set { SpellDataInfo.MaxTargets = value; } }
        public double ThreatRatio { get { return SpellDataInfo.ThreatRatio; } set { SpellDataInfo.ThreatRatio = value; } }
        public double BaseThreat { get { return SpellDataInfo.BaseThreat; } set { SpellDataInfo.BaseThreat = value; } }
        public EnergyType Energy { get { return SpellDataInfo.Energy; } set { SpellDataInfo.Energy = value; } }

        public class EndDmg
        {
            public double MinDmg { get; set; }
            public double MaxDmg { get; set; }
            public double PowerRatio { get; set; }
            public RatioType Type { get; set; }

            public EndDmg(double minDmg, double maxDmg, double powerRatio, RatioType type)
            {
                MinDmg = minDmg;
                MaxDmg = maxDmg;
                PowerRatio = powerRatio;
                Type = type;
            }
        }
        public EndDmg EndDmgInfo { get; set; }

        public class EndEffect
        {
            public string Name { get; set; }
            public double BaseThreat { get; set; }

            public EndEffect(string name, double baseThreat = 0)
            {
                Name = name;
                BaseThreat = baseThreat;
            }
        }

        public EndEffect EndEffectInfo { get; set; }

        public class ChannelDmg
        {
            public int TickDmg { get; set; }
            public double NextTick { get; set; }

            public double TickDelay { get; set; }
            public int BaseDmg { get; set; }
            public double PowerRatio { get; set; }

            public ChannelDmg(int baseDmg, double tickDelay, double powerRatio)
            {
                TickDelay = tickDelay;
                BaseDmg = baseDmg;
                PowerRatio = powerRatio;
            }
        }
        public ChannelDmg ChannelDmgInfo { get; set; }

        public Spell(Player p, double baseCD, School school, SpellData spellDataInfo, EndDmg endDmgInfo = null, EndEffect endEffectInfo = null, ChannelDmg channelDmgInfo = null)
            : base(p, baseCD, school)
        {
            SpellDataInfo = spellDataInfo;
            EndDmgInfo = endDmgInfo;
            EndEffectInfo = endEffectInfo;
            ChannelDmgInfo = channelDmgInfo;
        }

        public override bool CanUse()
        {
            return (Player.CurrentMainResource == Player.Resources.Mana ? Player.Mana >= CustomCost() : Player.Resource >= CustomCost()) && Available() && (!AffectedByGCD || Player.HasGCD());
        }

        public virtual int CustomCost()
        {
            return Cost;
        }

        public void OnCommonSpellCast()
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            if (Player.CurrentMainResource == Player.Resources.Mana)
            {
                OnManaResourceUse();
            }
        }

        public virtual void OnManaResourceUse()
        {
            Player.Mana -= CustomCost();
        }

        public virtual void OnNonManaResourceUse(ResultType res)
        {
            Player.Resources r = Player.CurrentMainResource;

            if (r == Player.Resources.Rage)
            {
                if (res == ResultType.Parry || res == ResultType.Dodge)
                {
                    Player.Resource -= (int)(CustomCost() * 0.2);
                }
                else
                {
                    Player.Resource -= CustomCost();
                }
            }
            else if (r == Player.Resources.Energy)
            {
                int cost = CustomCost();

                if (Player.Class == Player.Classes.Rogue)
                {
                    if (Player.Effects.ContainsKey("CdG"))
                    {
                        cost = 0;
                        Player.Effects["CdG"].EndEffect();
                    }
                }

                if (res == ResultType.Parry || res == ResultType.Dodge)
                {
                    // TODO à vérifier
                    Player.Resource -= cost / 2;
                }
                else
                {
                    Player.Resource -= cost;
                }

                if (Player.Class == Player.Classes.Rogue)
                {
                    if (Player.GetTalentPoints("RS") > 0 && Randomer.NextDouble() < 0.2 * Player.Combo)
                    {
                        Player.Resource += 25;
                    }
                }
            }
        }

        public void AddComboPoints(ResultType res)
        {
            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                Player.Combo++;
            }

            Player.BonusComboPoints(this, res);
        }

        public void ConsumeComboPoints()
        {
            Player.Combo = 0;

            Player.OnFinisher();
        }

        public override void Cast(Entity t)
        {
            Cast(t, false, null);
        }

        public void Cast(Entity t, bool forceInstant, bool? forceGcd)
        {
            Target = t;
            StartCast(forceInstant, forceGcd);
        }

        public virtual void StartCast(bool forceInstant = false, bool? forceGcd = null)
        {
            if ((AffectedByGCD && !forceGcd.HasValue) || (forceGcd.HasValue && forceGcd.Value))
            {
                Player.StartGCD();
            }
            if (CastTime > 0 && !forceInstant)
            {
                Player.casting = this;
                CastFinish = Player.Sim.CurrentTime + CastTime;

                if (ChannelDmgInfo != null)
                {
                    OnCommonSpellCast();

                    ChannelDmgInfo.NextTick = Player.Sim.CurrentTime + ChannelDmgInfo.TickDelay;

                    ResultType res = Target == Player ? ResultType.Hit : Player.SpellBinaryResistanceCheck(Target, School);
                    // TODO physical channel?

                    if (res == ResultType.Hit)
                    {
                        res = Player.SpellAttackEnemy(Target, false, Player.BonusHit(ToString(), Target, School));
                    }

                    if (res == ResultType.Hit)
                    {
                        Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                        ChannelDmgInfo.TickDmg = GetTickDamage();
                    }
                    else
                    {
                        Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Resist, 0, 0), Player.Sim.CurrentTime));
                    }
                }

                LogCast();
            }
            else if (MeleeInteraction == SMI.UseOnNextMHSwing)
            {
                Player.applyAtNextAA = this;
            }
            else
            {
                DoAction();
            }

            if (MeleeInteraction == SMI.Reset)
            {
                Player.ResetSwings();
            }
        }

        public virtual double GetEndDmgBase(bool mh = true)
        {
            double flatDmg = EndDmgInfo.MinDmg == EndDmgInfo.MaxDmg ? EndDmgInfo.MaxDmg : Randomer.Next(EndDmgInfo.MinDmg, EndDmgInfo.MaxDmg + 1);
            double power;
            switch (EndDmgInfo.Type)
            {
                case RatioType.None: power = 0; break;
                case RatioType.SP: power = Player.SchoolSP(School); break;
                case RatioType.Shield: power = Player.BlockValue; break;
                default: power = ((EndDmgInfo.Type == RatioType.WeaponMH || EndDmgInfo.Type == RatioType.WeaponDual) ? Simulation.Normalization(mh ? Player.MH : Player.OH) : 1)
                                    * (Player.AP + (MeleeInteraction == SMI.UseOnNextMHSwing ? Player.nextAABonus : 0)); break;
            }

            return flatDmg + power * EndDmgInfo.PowerRatio;
        }

        public virtual void CustomActionBefore() { }
        public virtual void CustomActionAfter() { }
        
        public override void DoAction()
        {
            Player.casting = null;

            if (MeleeInteraction == SMI.Reset)
            {
                Player.ResetSwings();
            }

            if (ChannelDmgInfo == null)
            {
                OnCommonSpellCast();
            }

            CustomActionBefore();

            List<ResultType> resList = new List<ResultType>();
            List<int> dmgList = new List<int>();

            Entity t = Target;
            for (int i = 0; i < (Target == Player ? 1 : Math.Min(MaxTargets, Player.Sim.NbTargets)); i++)
            {
                if(Target != Player)
                {
                    Target = Player.Sim.Boss[i];
                }

                ResultType res = ResultType.Success;

                if (EndDmgInfo != null)
                {
                    double mitigation;
                    if (School == School.Physical)
                    {
                        mitigation = Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen));
                    }
                    else
                    {
                        mitigation = Simulation.MagicMitigation(Target.ResistChances[School]);
                    }

                    if(Type == SpellType.Melee)
                    {
                        res = Player.YellowAttackEnemy(Target, ToString());
                    }
                    else if(Type == SpellType.Magical)
                    {
                        if (mitigation == 0)
                        {
                            res = ResultType.Resist;
                        }
                        else
                        {
                            res = Player.SpellAttackEnemy(Target, true, Player.BonusHit(ToString(), Target, School), Player.BonusCrit(ToString(), Target, School));
                        }
                    }
                    else
                    {
                        res = ResultType.Hit; // TODO res = Player.RangedYellowAttackEnemy(..)
                    }

                    double baseDmg = GetEndDmgBase();

                    int damage = (int)Math.Round(baseDmg
                        * Player.Sim.DamageMod(res, School)
                        * mitigation
                        * Player.DamageMod
                        * Player.TotalModifiers(NAME, Target, School, res));

                    OnNonManaResourceUse(res);
                    if(Player.CurrentMainResource == Player.Resources.Energy)
                    {
                        if(Energy == EnergyType.ComboAward)
                        {
                            AddComboPoints(res);
                        }
                        else if(Energy == EnergyType.ComboSpend)
                        {
                            ConsumeComboPoints();
                        }
                    }

                    dmgList.Add(damage);

                    RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod * ThreatRatio)));

                    if(Type != SpellType.Magical && Program.version != Version.TBC) Player.CheckOnHits(true, MeleeInteraction == SMI.UseOnNextMHSwing, res, false, null, this);

                    if(EndDmgInfo.Type == RatioType.WeaponDual)
                    {
                        baseDmg = GetEndDmgBase(false);

                        damage = (int)Math.Round(baseDmg
                            * Player.Sim.DamageMod(res, School)
                            * mitigation
                            * Player.DamageMod
                            * Player.TotalModifiers(NAME, Target, School, res));

                        dmgList.Add(damage);

                        RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod * ThreatRatio)));

                        if (Type != SpellType.Magical && Program.version != Version.TBC) Player.CheckOnHits(true, MeleeInteraction == SMI.UseOnNextMHSwing, res, false, null, this);
                    }
                }

                if(EndEffectInfo != null)
                {
                    if(Target == Player)
                    {
                        res = ResultType.Hit;
                    }
                    else if (EndDmgInfo == null)
                    {
                        if (Type == SpellType.Melee)
                        {
                            res = Player.YellowAttackEnemy(Target, ToString());
                        }
                        else if(Type == SpellType.Magical)
                        {
                            res = Player.SpellBinaryResistanceCheck(Target, School);
                            if(res == ResultType.Hit)
                            {
                                res = Player.SpellAttackEnemy(Target, false, Player.BonusHit(ToString(), Target, School));
                            }
                        }
                        else
                        {
                            res = ResultType.Hit;   // TODO Ranged
                        }
                    }

                    if (res == ResultType.Hit)
                    {
                        Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                        Effect.Apply(Player, Target, EndEffectInfo.Name);
                    }
                    else
                    {
                        RegisterDamage(new ActionResult(ResultType.Resist, 0, 0));
                    }
                }

                resList.Add(res);
            }

            CustomActionAfter();

            Player.CheckOnSpell(this, resList, dmgList);

            Target = t;
        }

        public void CheckTick()
        {
            if (ChannelDmgInfo.NextTick <= Player.Sim.CurrentTime)
            {
                ApplyTick(ChannelDmgInfo.TickDmg);
                ChannelDmgInfo.NextTick += ChannelDmgInfo.TickDelay;
            }
        }

        public int GetTickDamage()
        {
            double mitigation = 1;
            return (int)Math.Round((ChannelDmgInfo.BaseDmg + Player.SchoolSP(School) * ChannelDmgInfo.PowerRatio) / ChannelDmgInfo.TickDelay
                * Player.Sim.DamageMod(ResultType.Hit, School)
                * mitigation
                * Player.DamageMod
                * Player.TotalModifiers(NAME, Target, School, ResultType.Hit));
        }

        public virtual void ApplyTick(int damage)
        {
            //Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, damage), Player.Sim.CurrentTime));
            Player.Sim.RegisterEffect(new RegisteredEffect(new CustomEffect(Player, Target, ToString(), false, 1), damage, Player.Sim.CurrentTime, (int)(damage * Player.ThreatMod)));

            if (Program.logFight)
            {
                string log = string.Format("{0:N2} : {1} ticks for {2} damage", Player.Sim.CurrentTime, ToString(), damage);

                if (Player.Sim.NbTargets > 1)
                {
                    for (int i = 0; i < Player.Sim.Boss.Count; i++)
                    {
                        if (Player.Sim.Boss[i] == Target)
                        {
                            log += string.Format(" on Target {0}", i + 1);
                        }
                    }
                }

                Program.Log(log);
            }
        }

        public void LogCast()
        {
            if (Program.logFight)
            {
                string log = string.Format("{0:N2} : [{1}] started cast", Player.Sim.CurrentTime, ToString());
                if (Player.Sim.NbTargets > 1)
                {
                    for (int i = 0; i < Player.Sim.Boss.Count; i++)
                    {
                        if (Player.Sim.Boss[i] == Target)
                        {
                            log += string.Format(" on Target {0}", i + 1);
                        }
                    }
                }
                if (Player.CurrentMainResource != Player.Resources.Mana)
                {
                    log += string.Format(" ({0} {1}/{2})", Player.CurrentMainResource, Player.Resource, Player.MaxResource);
                }
                if (Player.Form == Player.Forms.Cat || Player.Class == Player.Classes.Rogue)
                {
                    log += " [combo " + Player.Combo + "]";
                }
                if (Player.MaxMana > 0)
                {
                    log += " - Mana " + Player.Mana + "/" + Player.MaxMana;
                }
                Program.Log(log);
            }
        }
    }
}

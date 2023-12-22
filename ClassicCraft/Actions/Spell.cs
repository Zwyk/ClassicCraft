using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Spell : Skill
    {
        public double CastTime { get; set; }
        public double TravelSpeed { get; set; }
        public double CastFinish;
        public double CastTimeWithGCD { get { return Math.Max(CastTime, AffectedByGCD ? Player.GCD_Hasted() : 0); } }

        public int MaxTargets { get; set; }
        public double ThreatRatio { get; set; }

        public class EndDmg
        {
            public int MinDmg { get; set; }
            public int MaxDmg { get; set; }
            public double PowerRatio { get; set; }
            public bool UseWeaponSpeed { get; set; }
            public bool IsBonusAA { get; set; }

            public EndDmg(int minDmg, int maxDmg, double powerRatio, bool useWeaponSpeed = false, bool isBonusAA = false)
            {
                MinDmg = minDmg;
                MaxDmg = maxDmg;
                PowerRatio = powerRatio;
                UseWeaponSpeed = useWeaponSpeed;
                IsBonusAA = isBonusAA;
            }
        }
        public EndDmg EndDmgInfo { get; set; }

        public class EndEffect
        {
            public string Name { get; set; }

            public EndEffect(string name)
            {
                Name = name;
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

        public Spell(Player p, double baseCD, int resourceCost, bool useMana, bool gcd, School school, double castTime, int maxTargets, double threatRatio, EndDmg endDmgInfo, EndEffect endEffectInfo, ChannelDmg channelDmgInfo)
            : base(p, baseCD, resourceCost, gcd, useMana, school)
        {
            CastTime = castTime;
            MaxTargets = maxTargets;
            ThreatRatio = threatRatio;
            EndDmgInfo = endDmgInfo;
            EndEffectInfo = endEffectInfo;
            ChannelDmgInfo = channelDmgInfo;
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
            if((AffectedByGCD && !forceGcd.HasValue) || (forceGcd.HasValue && forceGcd.Value))
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

                    ResultType res = Simulation.MagicMitigationBinary(Target.MagicResist[School]);

                    if (res == ResultType.Hit)
                    {
                        res = Player.SpellAttackEnemy(Target, false, 0.02 * Player.GetTalentPoints("SF"));
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
            else
            {
                DoAction();
            }
        }

        public void OnCommonSpellCast(int? customCost = null)
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            if(UseMana)
            {
                Player.Mana -= customCost ?? Cost;
            }
            else
            {
                Player.Resource -= customCost ?? Cost;
            }
        }

        public virtual double GetEndDmg()
        {
            double flatDmg = EndDmgInfo.MinDmg == EndDmgInfo.MaxDmg ? EndDmgInfo.MaxDmg : Randomer.Next(EndDmgInfo.MinDmg, EndDmgInfo.MaxDmg);
            double ratioDmg = School == School.Physical ?
                                (EndDmgInfo.UseWeaponSpeed ? Player.MH.Speed : 1) * (Player.AP + (EndDmgInfo.IsBonusAA ? Player.nextAABonus : 0)) * EndDmgInfo.PowerRatio
                                : (Player.SchoolSP(School) * EndDmgInfo.PowerRatio);

            return flatDmg + ratioDmg;
        }
        
        public override void DoAction()
        {
            Player.casting = null;

            Player.ResetMHSwing();

            if(ChannelDmgInfo == null)
            {
                OnCommonSpellCast();
            }

            ResultType res;

            Entity t = Target;
            for (int i = 0; i < (Target == Player ? 1 : Math.Min(MaxTargets, Player.Sim.NbTargets)); i++)
            {
                if(Target != Player)
                {
                    Target = Player.Sim.Boss[i];
                }

                if(EndDmgInfo != null)
                {
                    double mitigation;
                    if (School == School.Physical)
                    {
                        mitigation = Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen));
                        res = Player.YellowAttackEnemy(Target, ToString());
                    }
                    else
                    {
                        mitigation = Simulation.MagicMitigation(Target.ResistChances[School]);

                        if (mitigation == 0)
                        {
                            res = ResultType.Resist;
                        }
                        else
                        {
                            res = Player.SpellAttackEnemy(Target, true, Player.BonusHit(ToString(), Target, School), Player.BonusCrit(ToString(), Target, School));
                        }
                    }

                    double baseDmg = GetEndDmg();

                    int damage = (int)Math.Round(baseDmg
                        * Player.Sim.DamageMod(res, School)
                        * mitigation
                        * Player.DamageMod
                        * Player.TotalModifiers(NAME, Target, School, res));

                    RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod * ThreatRatio)));

                    Player.CheckOnSpell(this, res);
                    if(School == School.Physical && Program.version != Version.TBC) Player.CheckOnHits(true, EndDmgInfo.IsBonusAA, res);
                }
                else
                {
                    res = Simulation.MagicMitigationBinary(Target.MagicResist[School]);
                }

                if(EndEffectInfo != null)
                {
                    if (res == ResultType.Hit)
                    {
                        res = Player.SpellAttackEnemy(Target, false, Player.BonusHit(ToString(), Target, School));
                    }

                    if (res == ResultType.Hit)
                    {
                        Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                        if (Target.Effects.ContainsKey(EndEffectInfo.Name))
                        {
                            Target.Effects[EndEffectInfo.Name].Refresh();
                        }
                        else
                        {
                            Effect.NewEffectFromString(EndEffectInfo.Name, Player, Target).StartEffect();
                        }
                    }
                    else
                    {
                        RegisterDamage(new ActionResult(ResultType.Resist, 0, 0));
                    }
                }
            }
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
                if (!ResourceName().Equals("mana"))
                {
                    log += string.Format(" ({0} {1}/{2})", ResourceName(), Player.Resource, Player.MaxResource);
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

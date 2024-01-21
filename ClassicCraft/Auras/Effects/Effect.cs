using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;

namespace ClassicCraft
{
    public abstract class Effect : Aura
    {
        public static Effect NewEffectFromString(string s, Player p, Entity t)
        {
            // COMMON
            if (s == BerserkingBuff.NAME) return new BerserkingBuff(p);
            else if (s == BloodFuryBuff.NAME) return new BloodFuryBuff(p);
            // DRUID
            else if (s == ClearCasting.NAME) return new ClearCasting(p);
            else if (s == InnervateBuff.NAME) return new InnervateBuff(p);
            else if (s == Mangle.NAME) return new Mangle(p, t);
            else if (s == RipDoT.NAME) return new RipDoT(p, t);
            else if (s == SavageRoarBuff.NAME) return new SavageRoarBuff(p);
            // MAGE
            else if (s == PresenceOfMindEffect.NAME) return new PresenceOfMindEffect(p);
            // PRIEST
            else if (s == DevouringPlagueDoT.NAME) return new DevouringPlagueDoT(p, t);
            else if (s == InnerFocusBuff.NAME) return new InnerFocusBuff(p);
            else if (s == SWPDoT.NAME) return new SWPDoT(p, t);
            // ROGUE
            else if (s == AdrenalineRushBuff.NAME) return new AdrenalineRushBuff(p);
            else if (s == BladeFlurryBuff.NAME) return new BladeFlurryBuff(p);
            else if (s == DeadlyPoisonDoT.NAME) return new DeadlyPoisonDoT(p, t);
            else if (s == RuptureDoT.NAME) return new RuptureDoT(p, t);
            else if (s == SliceAndDiceBuff.NAME) return new SliceAndDiceBuff(p);
            // WARLOCK
            else if (s == CorruptionDoT.NAME) return new CorruptionDoT(p, t);
            else if (s == CurseOfAgonyDoT.NAME) return new CurseOfAgonyDoT(p, t);
            else if (s == DemonicGraceBuff.NAME) return new DemonicGraceBuff(p);
            else if (s == DrainLifeDoT.NAME) return new DrainLifeDoT(p, t);
            else if (s == Incinerate.NAME) return Incinerate.NewEffect(p, t);
            else if (s == ShadowTrance.NAME) return new ShadowTrance(p);
            else if (s == ShadowVulnerability.NAME) return new DrainLifeDoT(p, t);
            // WARRIOR
            else if (s == BattleShoutBuff.NAME) return new BattleShoutBuff(p);
            else if (s == BloodrageBuff.NAME) return new BloodrageBuff(p);
            else if (s == DeathWishBuff.NAME) return new DeathWishBuff(p);
            else if (s == DeepWounds.NAME) return new DeepWounds(p, t);
            else if (s == Flurry.NAME) return new Flurry(p);
            else if (s == RampageBuff.NAME) return new RampageBuff(p);
            else if (s == RecklessnessBuff.NAME) return new RecklessnessBuff(p);
            else if (s == RendDoT.NAME) return new RendDoT(p, t);
            else if (s == SweepingStrikesBuff.NAME) return new SweepingStrikesBuff(p);
            else throw new NotImplementedException(s);
        }

        public Entity Target { get; set; }
        public bool Friendly { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public double Duration { get; set; }
        public int BaseStacks { get; set; }
        public int MaxStacks { get; set; }
        public int CurrentStacks { get; set; }
        public List<double> AppliedTimes { get; set; }

        public bool IsPermanent { get; set; }

        public Effect(Player p, Entity target, bool friendly, double baseLength, int baseStacks = 1, int maxStacks = 0)
            : base(p)
        {
            IsPermanent = baseLength < 0;
            Target = target;
            Friendly = friendly;
            Start = IsPermanent ? -1 : Player.Sim.CurrentTime;
            Duration = IsPermanent ? -1 : baseLength;
            End = IsPermanent ? -1 : Start + CustomDuration();
            BaseStacks = baseStacks;
            MaxStacks = maxStacks;
            CurrentStacks = BaseStacks;
            AppliedTimes = new List<double>
            {
                Start
            };
        }

        public double RemainingTime()
        {
            return IsPermanent ? -1 : End - Player.Sim.CurrentTime;
        }

        public virtual void CheckEffect()
        {
            if (!IsPermanent && End < Player.Sim.CurrentTime)
            {
                EndEffect();
            }
        }

        public virtual void WhenApplied()
        {
        }

        public static void Apply(Player source, Entity target, string effect)
        {
            if (target.Effects.ContainsKey(effect))
            {
                target.Effects[effect].Refresh();
            }
            else
            {
                NewEffectFromString(effect, source, target).StartEffect();
            }
        }

        public virtual double CustomDuration()
        {
            return Duration;
        }

        public virtual void Refresh()
        {
            WhenApplied();

            End = Player.Sim.CurrentTime + CustomDuration();
            AppliedTimes.Add(Player.Sim.CurrentTime);
            CurrentStacks = BaseStacks;

            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} refreshed", Player.Sim.CurrentTime, ToString()));
            }
        }

        public virtual void StartEffect()
        {
            WhenApplied();

            Target.Effects.Add(ToString(), this);
            CurrentStacks = BaseStacks;

            if (Program.logFight)
            {
                string log = string.Format("{0:N2} : {1} started", Player.Sim.CurrentTime, ToString());

                if (Target != Player && Player.Sim.NbTargets > 1)
                {
                    for (int i = 0; i < Player.Sim.Boss.Count; i++)
                    {
                        if (Player.Sim.Boss[i] == Target)
                        {
                            log += string.Format(" on Target {0}", i + 1);
                            break;
                        }
                    }
                }

                Program.Log(log);
            }
        }

        public virtual void StackAdd(int nb = 1)
        {
            int oldStacks = CurrentStacks;
            CurrentStacks = Math.Min(MaxStacks, CurrentStacks + nb);

            if (oldStacks != CurrentStacks && Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} stacks added, at {2}", Player.Sim.CurrentTime, ToString(), CurrentStacks));
            }
        }

        public virtual void StackRemove(int nb = 1)
        {
            int oldStacks = CurrentStacks;
            CurrentStacks -= Math.Max(IsPermanent ? 1 : 0, nb);
            if(CurrentStacks < 1)
            {
                EndEffect();
            }

            if (oldStacks != CurrentStacks && CurrentStacks > 0 && Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} stacks lost, at {2}", Player.Sim.CurrentTime, ToString(), CurrentStacks));
            }
        }

        public virtual void EndEffect()
        {
            if(Target.Effects.ContainsKey(ToString()))
            {
                End = Player.Sim.CurrentTime;
                Target.Effects.Remove(ToString());

                if (Program.logFight)
                {
                    Program.Log(string.Format("{0:N2} : {1} ended", Player.Sim.CurrentTime, ToString()));
                }
            }
        }
    }
}

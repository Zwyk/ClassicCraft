using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Effect : Aura
    {
        public Entity Target { get; set; }
        public bool Friendly { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public double BaseLength { get; set; }
        public int BaseStacks { get; set; }
        public int CurrentStacks { get; set; }
        public List<double> AppliedTimes { get; set; }

        public bool IsPermanent { get; set; }

        public Effect(Player p, Entity target, bool friendly, double baseLength, int baseStacks = 1)
            : base(p)
        {
            IsPermanent = baseLength < 0;
            Target = target;
            Friendly = friendly;
            Start = IsPermanent ? -1 : Player.Sim.CurrentTime;
            BaseLength = IsPermanent ? -1 : baseLength;
            End = IsPermanent ? -1 : Start + BaseLength;
            BaseStacks = baseStacks;
            CurrentStacks = BaseStacks;
            AppliedTimes = new List<double>();
            AppliedTimes.Add(Start);
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

        public virtual void Refresh()
        {
            CurrentStacks = BaseStacks;
            End = Player.Sim.CurrentTime + BaseLength;
            AppliedTimes.Add(Player.Sim.CurrentTime);

            if(Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} refreshed", Player.Sim.CurrentTime, ToString()));
            }
        }

        public virtual void StartEffect()
        {
            Target.Effects.Add(ToString(), this);

            if(Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} started", Player.Sim.CurrentTime, ToString()));
            }
        }

        public virtual void StackAdd(int nb = 1)
        {
            CurrentStacks += nb;
        }

        public virtual void StackRemove(int nb = 1)
        {
            CurrentStacks -= nb;
            if(CurrentStacks < 1)
            {
                EndEffect();
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

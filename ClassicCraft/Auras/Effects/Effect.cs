using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Effect : Aura
    {
        public bool Friendly { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public double BaseLength { get; set; }
        public int BaseStacks { get; set; }
        public int CurrentStacks { get; set; }
        public List<double> AppliedTimes { get; set; }
        public bool Ended { get; set; }

        public Effect(Player p, Entity target, bool friendly, double baseLength, int baseStacks = 1)
            : base(p, target)
        {
            Target = target;
            Friendly = friendly;
            Start = Player.Sim.CurrentTime;
            BaseLength = baseLength;
            End = Start + BaseLength;
            BaseStacks = baseStacks;
            CurrentStacks = BaseStacks;
            AppliedTimes = new List<double>();
            AppliedTimes.Add(Start);
            Ended = false;
        }

        public double RemainingTime()
        {
            return End - Player.Sim.CurrentTime;
        }

        public virtual void CheckEffect()
        {
            if (End < Player.Sim.CurrentTime)
            {
                EndBuff();
            }
        }

        public virtual void Refresh()
        {
            CurrentStacks = BaseStacks;
            End = Player.Sim.CurrentTime + BaseLength;
            AppliedTimes.Add(Player.Sim.CurrentTime);

            if(Program.logFight)
            {
                Console.WriteLine("{0:N2} : {1} refreshed", Player.Sim.CurrentTime, ToString());
            }
        }

        public virtual void StartBuff()
        {
            Target.Effects.Add(this);

            if(Program.logFight)
            {
                Console.WriteLine("{0:N2} : {1} started", Player.Sim.CurrentTime, ToString());
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
                EndBuff();
            }
        }

        public virtual void EndBuff()
        {
            End = Player.Sim.CurrentTime;
            Ended = true;

            if(Program.logFight)
            {
                Console.WriteLine("{0:N2} : {1} ended", Player.Sim.CurrentTime, ToString());
            }
        }

        public override string ToString()
        {
            return "Undefined Effect";
        }
    }
}

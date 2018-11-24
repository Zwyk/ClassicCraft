using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class EffectOnTime : Effect
    {
        public static int TICK_DELAY = 3;

        public double NextTick { get; set; }
        public int TickDamage { get; set; }

        public EffectOnTime(Entity target, bool friendly, double baseLength, int baseStacks = 1)
            : base(target, friendly, baseLength, baseStacks)
        {
            NextTick = Program.currentTime + TICK_DELAY;
        }

        public override void StartBuff()
        {
            base.StartBuff();

            TickDamage = GetTickDamage();
        }

        public override void CheckEffect()
        {
            if (!Ended && NextTick <= Program.currentTime)
            {
                ApplyDamage(TickDamage);
                NextTick += TICK_DELAY;
            }

            base.CheckEffect();
        }

        public override void Refresh()
        {
            base.Refresh();

            TickDamage = GetTickDamage();
            NextTick = Program.currentTime + TICK_DELAY;
        }
        public abstract int GetTickDamage();

        public void ApplyDamage(int damage)
        {
            Program.RegisterEffect(new RegisteredEffect(this, damage, Program.currentTime));

            if (Program.nbSim < 10)
            {
                Console.WriteLine("{0:N2} : {1} for {2} damage", Program.currentTime, ToString(), damage, Program.Player.Ressource);
            }
        }

        public override string ToString()
        {
            return "Undefined Effect on Time";
        }
    }
}

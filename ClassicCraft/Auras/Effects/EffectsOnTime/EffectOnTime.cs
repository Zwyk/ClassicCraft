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

        public EffectOnTime(Player p, Entity target, bool friendly, double baseLength, int baseStacks = 1)
            : base(p, target, friendly, baseLength, baseStacks)
        {
            NextTick = Player.Sim.CurrentTime + TICK_DELAY;
        }

        public override void StartBuff()
        {
            base.StartBuff();

            TickDamage = GetTickDamage();
        }

        public override void CheckEffect()
        {
            if (!Ended && NextTick <= Player.Sim.CurrentTime)
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
            NextTick = Player.Sim.CurrentTime + TICK_DELAY;
        }
        public abstract int GetTickDamage();

        public void ApplyDamage(int damage)
        {
            Player.Sim.RegisterEffect(new RegisteredEffect(this, damage, Player.Sim.CurrentTime));

            if (Program.nbSim < 10)
            {
                Console.WriteLine("{0:N2} : {1} for {2} damage", Player.Sim.CurrentTime, ToString(), damage, Player.Ressource);
            }
        }

        public override string ToString()
        {
            return "Undefined Effect on Time";
        }
    }
}

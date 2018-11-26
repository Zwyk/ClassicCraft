using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class BloodrageBuff : EffectOnTime
    {
        public BloodrageBuff(Player p, bool friendly = true, double baseLength = 10, int baseStacks = 1, int tickDelay = 1)
            : base(p, p, friendly, baseLength, baseStacks, tickDelay)
        {
        }

        public override int GetTickDamage()
        {
            return 0;
        }

        public override void ApplyTick(int damage)
        {
            Player.Ressource += 1;

            if(Program.logFight)
            {
                Console.WriteLine("{0:N2} : {1} for {2} rage ({3})", Player.Sim.CurrentTime, ToString(), damage, Player.Ressource);
            }
        }

        public override string ToString()
        {
            return "BrBuff";
        }
    }
}

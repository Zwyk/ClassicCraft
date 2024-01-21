using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeepWounds : EffectOnTime
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Deep Wounds";

        public static double DURATION = 12;

        public static int TICK_DELAY = 3;

        public static double RATIO = 0;

        public double PooledDmg = 0;

        public DeepWounds(Player p, Entity target)
            : base(p, target, false, DURATION, 1, RATIO, TICK_DELAY, 1, School.Physical)
        {
        }

        public double DmgCalc()
        {
            int minDmg = (int)Math.Round(Player.MH.DamageMin + Player.MH.Speed * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Player.MH.Speed * Player.AP / 14);

            return (minDmg + maxDmg) / 2.0 * 0.2 * Player.GetTalentPoints("DW");
        }

        public override double BaseDmg()
        {
            return PooledDmg + DmgCalc();
        }

        public static void CheckProc(Player p, ResultType type)
        {
            if (type == ResultType.Crit && p.GetTalentPoints("DW") > 0)
            {
                if (p.Target.Effects.ContainsKey(NAME))
                {
                    p.Target.Effects[NAME].Refresh();
                }
                else
                {
                    new DeepWounds(p, p.Target).StartEffect();
                }
            }
        }

        public override int GetTickDamage()
        {
            int tick = base.GetTickDamage();

            PooledDmg = tick * CustomDuration() / TickDelay;

            //Program.Log("Pooled = " + PooledDmg + " - Tick = " + tick);

            return tick;
        }

        public override void ApplyTick(int damage)
        {
            base.ApplyTick(damage);

            PooledDmg *= TicksLeft > 0 ? (TicksLeft - 1.0) / TicksLeft : 0;

            //Program.Log("Pooled = " + PooledDmg + " - Damage = " + damage);
        }
    }
}

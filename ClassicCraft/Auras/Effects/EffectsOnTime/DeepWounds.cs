using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeepWounds : EffectOnTime
    {
        public double Ratio { get; set; }

        public DeepWounds(Simulation s, int points, Entity target, bool friendly = false, double baseLength = 12, int baseStacks = 1)
            : base(s, target, friendly, baseLength, baseStacks)
        {
            Ratio = 0.2 * points;
        }

        public static void CheckProc(Simulation sim, ResultType type, int points)
        {
            if (type == ResultType.Crit)
            {
                if (sim.Boss.Effects.Any(e => e is DeepWounds))
                {
                    sim.Boss.Effects.Where(e => e is DeepWounds).First().Refresh();
                }
                else
                {
                    DeepWounds dw = new DeepWounds(sim, points, sim.Boss);
                    dw.StartBuff();
                }
            }
        }

        public override int GetTickDamage()
        {
            int minDmg = (int)Math.Round(Sim.Player.MH.DamageMin + Sim.Player.MH.Speed * Sim.Player.AP / 14);
            int maxDmg = (int)Math.Round(Sim.Player.MH.DamageMax + Sim.Player.MH.Speed * Sim.Player.AP / 14);

            int damage = (int)Math.Round((minDmg + maxDmg) / 2
                * Entity.ArmorMitigation(Sim.Boss.Armor)
                * (Sim.Player.DualWielding() ? 1 : 1.03)
                * Ratio);

            return (int)Math.Round(damage / BaseLength * TICK_DELAY);
        }

        public override string ToString()
        {
            return "DW";
        }
    }
}

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

        public DeepWounds(Player p, int points, Entity target, bool friendly = false, double baseLength = 12, int baseStacks = 1)
            : base(p, target, friendly, baseLength, baseStacks)
        {
            Ratio = 0.2 * points;
        }

        public static void CheckProc(Player p, ResultType type, int points)
        {
            if (type == ResultType.Crit)
            {
                if (p.Sim.Boss.Effects.Any(e => e is DeepWounds))
                {
                    p.Sim.Boss.Effects.Where(e => e is DeepWounds).First().Refresh();
                }
                else
                {
                    DeepWounds dw = new DeepWounds(p, points, p.Sim.Boss);
                    dw.StartBuff();
                }
            }
        }

        public override int GetTickDamage()
        {
            int minDmg = (int)Math.Round(Player.MH.DamageMin + Player.MH.Speed * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Player.MH.Speed * Player.AP / 14);

            int damage = (int)Math.Round((minDmg + maxDmg) / 2
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (Player.DualWielding() ? 1 : 1.03)
                * Ratio);

            return (int)Math.Round(damage / BaseLength * TickDelay);
        }

        public override string ToString()
        {
            return "DW";
        }
    }
}

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

        public DeepWounds(int points, Entity target, bool friendly = false, double baseLength = 12, int baseStacks = 1)
            : base(target, friendly, baseLength, baseStacks)
        {
            Ratio = 0.2 * points;
        }

        public static void CheckProc(ResultType type, int points)
        {
            if (type == ResultType.Crit)
            {
                if (Program.Boss.Effects.Any(e => e is DeepWounds))
                {
                    Program.Boss.Effects.Where(e => e is DeepWounds).First().Refresh();
                }
                else
                {
                    DeepWounds dw = new DeepWounds(points, Program.Boss);
                    dw.StartBuff();
                }
            }
        }

        public override int GetTickDamage()
        {
            int minDmg = (int)Math.Round(Program.Player.MH.DamageMin + Program.Player.MH.Speed * Program.Player.AP / 14);
            int maxDmg = (int)Math.Round(Program.Player.MH.DamageMax + Program.Player.MH.Speed * Program.Player.AP / 14);

            int damage = (int)Math.Round((minDmg + maxDmg) / 2
                * Entity.ArmorMitigation(Program.Boss.Armor)
                * (Program.Player.DualWielding() ? 1 : 1.03)
                * Ratio);

            return (int)Math.Round(damage / BaseLength * TICK_DELAY);
        }

        public override string ToString()
        {
            return "DW";
        }
    }
}

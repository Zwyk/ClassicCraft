using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DeepWounds : EffectOnTime
    {
        public override string ToString() { return NAME; } public static new string NAME = "Deep Wounds";

        public static double DURATION = 12;

        public double Ratio { get; set; }

        public DeepWounds(Player p, int points, Entity target)
            : base(p, target, false, DURATION, 1)
        {
            Ratio = 0.2 * points;
        }

        public static void CheckProc(Player p, ResultType type, int points)
        {
            if (type == ResultType.Crit)
            {
                if (p.Sim.Boss.Effects.ContainsKey(NAME))
                {
                    p.Sim.Boss.Effects[NAME].Refresh();
                }
                else
                {
                    new DeepWounds(p, points, p.Sim.Boss).StartEffect();
                }
            }
        }

        public override int GetTickDamage()
        {
            int minDmg = (int)Math.Round(Player.MH.DamageMin + Player.MH.Speed * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Player.MH.Speed * Player.AP / 14);

            int damage = (int)Math.Round((minDmg + maxDmg) / 2
                * (Player.DualWielding ? 1 : 1.03)
                * Ratio);

            return (int)Math.Round(damage / BaseLength * TickDelay);
        }
        
        public override double GetExternalModifiers()
        {
            return base.GetExternalModifiers() * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen));
        }
    }
}

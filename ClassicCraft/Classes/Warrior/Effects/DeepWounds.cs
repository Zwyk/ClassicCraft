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
                * Ratio
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                * (Player.Sim.Boss.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                );

            return (int)Math.Round(damage / BaseLength * TickDelay);
        }
        
        public override double GetExternalModifiers()
        {
            return base.GetExternalModifiers()
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen)
                * (Target.Effects.ContainsKey("Mangle") ? 1.3 : 1)
                );
        }
    }
}

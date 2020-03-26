using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
	public class Vengeance : Effect
	{
        public static new string NAME = "Vengeance";
        public override string ToString()
        {
            return NAME;
        }

        public static int LENGTH = 8;
        public static double DMG_BONUS_MULTIPLIER = 1.0;

        public Vengeance(Player p)
            : base(p, p, true, LENGTH, 1)
        {
            DMG_BONUS_MULTIPLIER = 1.0 + 0.03 * p.GetTalentPoints("Veng");
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.DamageMod *= DMG_BONUS_MULTIPLIER;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.DamageMod /= DMG_BONUS_MULTIPLIER;
        }

        public static void CheckProc(Player p, ResultType res)
        {
            if (res == ResultType.Crit)
            {
                if (p.Effects.ContainsKey(NAME))
                {
                    p.Effects[NAME].Refresh();
                }
                else
                {
                    new Vengeance(p).StartEffect();
                }
            }

            /* Trying to track vengeance uptime in the sim...
            if (p.Effects.ContainsKey(NAME))
            {
                p.Sim.RegisterEffect(new RegisteredEffect(new Vengeance(p), 0, p.Sim.CurrentTime));
            }
            */
        }
    }
}

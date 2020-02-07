using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class PresenceOfMind : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Presence of Mind";

        public static int BASE_COST = 0;
        public static int CD = 180;
        public static double CAST_TIME = 0;

        public PresenceOfMind(Player p)
            : base(p, CD, BASE_COST, true, false)
        {
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();

            if (Player.Sim.Boss.Effects.ContainsKey(PresenceOfMindEffect.NAME))
            {
                Player.Sim.Boss.Effects[PresenceOfMindEffect.NAME].Refresh();
            }
            else
            {
                new PresenceOfMindEffect(Player).StartEffect();
            }
        }
    }
}

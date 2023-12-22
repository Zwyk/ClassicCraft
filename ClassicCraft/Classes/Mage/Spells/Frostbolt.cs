using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Frostbolt : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Frostbolt";

        public static int BASE_COST = 260;
        public static int CD = 0;
        public static double CAST_TIME = 3;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public static int MIN_DMG = 440;
        public static int MAX_DMG = 475;

        public Frostbolt(Player p)
            : base(p, CD, (int)(BASE_COST * 1 - (0.05 * p.GetTalentPoints("FC"))), true, true, School.Frost, CAST_TIME - 0.1 * p.GetTalentPoints("IFB"), 1, 1, new EndDmg(MIN_DMG, MAX_DMG, RATIO), null, null)
        {
        }

        public override void Cast(Entity t)
        {
            bool pom = Player.Effects.ContainsKey(PresenceOfMindEffect.NAME);
            if(pom) Player.Effects[PresenceOfMindEffect.NAME].EndEffect();
            Cast(t, pom, false);
        }
    }
}

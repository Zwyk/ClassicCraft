using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BattleShoutBuff : Effect
    {
        public static int LENGTH = 120;

        public double Bonus { get; set; }

        public BattleShoutBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Bonus = (Program.version == Version.TBC ? 305 : 232) * (1 + (0.05 * Player.GetTalentPoints("IBS")));

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + Bonus * (Program.version == Version.TBC ? 1 + 0.02 * Player.GetTalentPoints("IBStance") : 1));
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) - Bonus * (Program.version == Version.TBC ? 1 + 0.02 * Player.GetTalentPoints("IBStance") : 1));
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Battle Shout's Buff";
    }
}

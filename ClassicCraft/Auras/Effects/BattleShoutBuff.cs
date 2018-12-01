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

        public override void StartBuff()
        {
            base.StartBuff();

            Bonus = 232 * 1 + (0.05 * Player.GetTalentPoints("IBS"));

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + Bonus);
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) - Bonus);
        }

        public override string ToString()
        {
            return "Battle Shout's Buff";
        }
    }
}

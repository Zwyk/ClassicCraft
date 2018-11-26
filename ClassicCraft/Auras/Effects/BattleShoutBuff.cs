﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BattleShoutBuff : Effect
    {
        public BattleShoutBuff(Player p, bool friendly = true, double baseLength = 120, int baseStacks = 1)
            : base(p, p, friendly, baseLength, baseStacks)
        {
        }

        public override void StartBuff()
        {
            base.StartBuff();

            Player.Attributes.SetValue(Attribute.AP, Player.Attributes.GetValue(Attribute.AP) + 232 * 1 + (0.05 * Player.GetTalentPoints("IBS")));
        }

        public override void EndBuff()
        {
            base.EndBuff();

            Player.Attributes.SetValue(Attribute.AP, Player.Attributes.GetValue(Attribute.AP) - 232 * 1 + (0.05 * Player.GetTalentPoints("IBS")));
        }

        public override string ToString()
        {
            return "BSBuff";
        }
    }
}

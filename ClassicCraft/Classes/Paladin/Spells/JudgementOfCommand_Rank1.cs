using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JudgementOfCommand_Rank1 : JudgementOfCommand_Base
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "=== JUDGEMENT === JoC (Rank 1)";

        public JudgementOfCommand_Rank1(Player p) : base(p)
        {
            minDmg_Normal = 68;
            maxDmg_Normal = 73;
            minDmg_Stunned = 137;
            maxDmg_Stunned = 146;
        }
    }
}

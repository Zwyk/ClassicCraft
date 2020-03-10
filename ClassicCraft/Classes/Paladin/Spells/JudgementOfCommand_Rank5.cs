using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JudgementOfCommand_Rank5 : JudgementOfCommand_Base
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "=== JUDGEMENT === JoC (Rank 5)";

        public JudgementOfCommand_Rank5(Player p) : base(p)
        {
            minDmg_Normal = 169;
            maxDmg_Normal = 186;
            minDmg_Stunned = 339;
            maxDmg_Stunned = 373;
        }
    }
}

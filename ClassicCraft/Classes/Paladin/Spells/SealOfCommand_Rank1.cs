using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SealOfCommand_Rank1 : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "+++ SEAL +++ SoC (Rank 1)";

        public static int CD = 0;

        public SealOfCommand_Rank1(Player p)
            : base(p, CD, (int)(65 - (0.03 * p.GetTalentPoints("Bene"))), true, true, School.Light, 0)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank1.NAME))
            {
                Player.Effects[SealOfCommandBuff_Rank1.NAME].Refresh();
            }
            else
            {
                //  Remove other seals
                if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank5.NAME))
                {
                    Player.Effects[SealOfCommandBuff_Rank5.NAME].StackRemove();
                }
                if (Player.Effects.ContainsKey(SealOfTheCrusaderBuff.NAME))
                {
                    Player.Effects[SealOfTheCrusaderBuff.NAME].StackRemove();
                }
                if (Player.Effects.ContainsKey(SealOfRighteousnessBuff.NAME))
                {
                    Player.Effects[SealOfRighteousnessBuff.NAME].StackRemove();
                }

                new SealOfCommandBuff_Rank1(Player).StartEffect();
            }
        }
    }
}

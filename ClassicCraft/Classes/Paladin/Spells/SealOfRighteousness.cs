using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SealOfRighteousness : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "+++ SEAL +++ SoR";

        public static int CD = 0;

        public SealOfRighteousness(Player p)
            : base(p, CD, (int)(170 - (0.03 * p.GetTalentPoints("Bene"))), true, true, School.Light, 0)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            if (Player.Effects.ContainsKey(SealOfRighteousnessBuff.NAME))
            {
                Player.Effects[SealOfRighteousnessBuff.NAME].Refresh();
            }
            else
            {
                //  Remove other seals
                if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank1.NAME))
                {
                    Player.Effects[SealOfCommandBuff_Rank1.NAME].StackRemove();
                }
                if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank5.NAME))
                {
                    Player.Effects[SealOfCommandBuff_Rank5.NAME].StackRemove();
                }
                if (Player.Effects.ContainsKey(SealOfTheCrusaderBuff.NAME))
                {
                    Player.Effects[SealOfTheCrusaderBuff.NAME].StackRemove();
                }

                new SealOfRighteousnessBuff(Player).StartEffect();
            }
        }
    }
}

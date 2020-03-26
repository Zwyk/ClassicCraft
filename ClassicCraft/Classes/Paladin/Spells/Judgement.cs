using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public enum JudgementType : ushort
    {
        None = 0,

        Crusader,

        Light,

        Wisdom,

        Justice,

        Righteousness,

        Command
    }

    public class Judgement : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Judgement";

        public static int CD = 10;

        public Judgement(Player p)
            : base(p, CD - p.GetTalentPoints("ImpJ"), (int)(p.MaxMana * 0.06 - (0.03 * p.GetTalentPoints("Bene"))), true, false, School.Light, 0)
        {
        }

        public override void DoAction()
        {
            //  Figure out which Seal is currently enabled
            Judgement theJudgement = null;
            if (Player.Effects.ContainsKey(SealOfTheCrusaderBuff.NAME))
            {
                //  Remove the seal
                if (Player.Effects.ContainsKey(SealOfTheCrusaderBuff.NAME))
                {
                    Player.Effects[SealOfTheCrusaderBuff.NAME].StackRemove();
                }

                //  And execute Judgement
                theJudgement = new JudgementOfTheCrusader(Player);
            }
            else if (Player.Effects.ContainsKey("+++ SEAL +++ Light"))
            {
            }
            else if (Player.Effects.ContainsKey("+++ SEAL +++ Wisdom"))
            {
            }
            else if (Player.Effects.ContainsKey("+++ SEAL +++ Justice"))
            {
            }
            else if (Player.Effects.ContainsKey(SealOfRighteousnessBuff.NAME))
            {
                //  Remove the seal
                if (Player.Effects.ContainsKey(SealOfRighteousnessBuff.NAME))
                {
                    Player.Effects[SealOfRighteousnessBuff.NAME].StackRemove();
                }

                //  And execute Judgement
                theJudgement = new JudgementOfRighteousness(Player);
            }
            else if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank5.NAME))
            {
                //  Remove the seal
                if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank5.NAME))
                {
                    Player.Effects[SealOfCommandBuff_Rank5.NAME].StackRemove();
                }

                //  And execute Judgement
                theJudgement = new JudgementOfCommand_Rank5(Player);
            }
            else if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank1.NAME))
            {
                //  Remove the seal
                if (Player.Effects.ContainsKey(SealOfCommandBuff_Rank1.NAME))
                {
                    Player.Effects[SealOfCommandBuff_Rank1.NAME].StackRemove();
                }

                //  And execute Judgement
                theJudgement = new JudgementOfCommand_Rank1(Player);
            }

            if (theJudgement != null)
            {
                theJudgement.DoAction();

                //  Does trigger GCD after cast
                CommonManaSpell();
            }
        }
    }
}

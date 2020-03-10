using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class JudgementOfTheCrusader : Judgement
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "=== JUDGEMENT === JotC";

        public JudgementOfTheCrusader(Player p)
            : base(p)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            /*  I do;'t think this can be resisted...
            ResultType res = Player.SpellAttackEnemy(Player.Sim.Boss, false, 0, 0);

            //  Can't crit, can't glance
            if (res == ResultType.Hit)
            */
            {
                //  Refresh or apply the judgement debuff
                if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME))
                {
                    Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME].Refresh();
                }
                else {
                    //  Remove any existing judgements
                    /*  FIXME - TBD
                    if (Player.Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME))
                    {
                        Player.Sim.Boss.Effects[JudgementOfTheCrusaderDebuff.NAME].StackRemove();
                    }
                    */


                    //  And apply crusader
                    new JudgementOfTheCrusaderDebuff(Player).StartEffect();
                }
            }
        }
    }
}

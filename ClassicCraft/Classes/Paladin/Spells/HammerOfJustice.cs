using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class HammerOfJustice : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Hammer of Justice";

        public static int CD = 0;

        public HammerOfJustice(Player p)
            : base(p, CD, 100, true, true, School.Light, 0)
        {
        }

        public override void DoAction()
        {
            //  Don't call base.DoAction, since that resets the swing timer
            Player.casting = null;

            LogAction();

            if (Player.Effects.ContainsKey(HammerOfJusticeDebuff.NAME))
            {
                Player.Effects[HammerOfJusticeDebuff.NAME].Refresh();
            }
            else
            {
                new HammerOfJusticeDebuff(Player).StartEffect();
            }
        }
    }
}

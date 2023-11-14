using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class RuneOfMeta : ActiveItem
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Rune of Metamorphosis";

        public static int CD = 300;

        public RuneOfMeta(Player p)
            : base(p, CD)
        {
        }

        public override void Cast(Entity t)
        {
            CDAction();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(NAME))
            {
                Player.Effects[NAME].Refresh();
            }
            else
            {
                new CustomEffect(Player, Player, NAME, true, 20).StartEffect();
            }

            LogAction();
        }
    }
}

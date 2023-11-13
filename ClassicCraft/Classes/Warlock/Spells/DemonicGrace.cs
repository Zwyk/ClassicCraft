using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DemonicGrace : Skill
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Demonic Grace";

        public static int CD = 20;

        public DemonicGrace(Player p)
               : base(p, CD, 0, false, true)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(DemonicGraceBuff.NAME))
            {
                Player.Effects[DemonicGraceBuff.NAME].Refresh();
            }
            else
            {
                new DemonicGraceBuff(Player).StartEffect();
            }
        }
    }
}

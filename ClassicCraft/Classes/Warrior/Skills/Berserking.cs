using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Berserking : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Berserking";

        public static int BASE_COST = 5;
        public static int CD = 180;

        public Berserking(Player p)
            : base(p, CD, BASE_COST, false)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(BerserkingBuff.NAME))
            {
                Player.Effects[BerserkingBuff.NAME].Refresh();
            }
            else
            {
                new BerserkingBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class ManaRune : ActiveItem
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Demonic/Dark Rune";

        public static int CD = 120;

        public static int LIFE_MIN = 600;
        public static int LIFE_MAX = 1000;
        public static int MANA_MIN = 900;
        public static int MANA_MAX = 1500;

        public ManaRune(Player p)
            : base(p, CD)
        {
        }

        public override void DoAction()
        {
            //Player.Life -= Randomer.Next(LIFE_MIN, LIFE_MAX + 1);
            Player.Mana += Randomer.Next(MANA_MIN, MANA_MAX + 1);

            LogAction();
        }
    }
}

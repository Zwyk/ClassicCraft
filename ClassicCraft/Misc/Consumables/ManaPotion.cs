using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class ManaPotion : ActiveItem
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Mana Potion";

        public static int CD = 120;

        public static int MIN = 1350;
        public static int MAX = 2250;

        public ManaPotion(Player p)
            : base(p, CD)
        {
        }

        public override void DoAction()
        {
            Player.Mana += Randomer.Next(MIN, MAX + 1);

            LogAction();
        }
    }
}

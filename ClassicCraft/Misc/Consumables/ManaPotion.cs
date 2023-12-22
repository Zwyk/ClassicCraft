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

        public static int MIN(int level)
        {
            if (level >= 60) return 1350;
            else return 455;
        }
        public static int MAX(int level)
        {
            if (level >= 60) return 2250;
            else return 585;
        }

        public ManaPotion(Player p)
            : base(p, CD)
        {
        }

        public override void DoAction()
        {
            Player.Mana += Randomer.Next(MIN(Player.Level), MAX(Player.Level) + 1);

            LogAction();
        }
    }
}

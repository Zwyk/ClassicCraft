using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shift : Spell
    {
        public static int COST = 0;
        public static int CD = 0;

        static Random random = new Random();

        public Shift(Player p)
            : base(p, CD, COST) { }


        public override void CommonSpell()
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            Player.Mana -= ResourceCost;
        }

        public override bool CanUse()
        {
            return Player.Mana >= ResourceCost && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            CommonAction();

            Player.Resource = 0
                + ((Randomer.NextDouble() < Player.GetTalentPoints("Furor") * 0.2) ? 40 : 0)
                + (Player.Equipment[Player.Slot.Head].Name == "Wolfshead Helm" ? 20 : 0);

            LogAction();
        }

        public override string ToString()
        {
            return "Shift";
        }
    }
}

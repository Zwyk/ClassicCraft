using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shift : Spell
    {
        public static int CD = 0;

        static Random random = new Random();

        public Shift(Player p)
            : base(p, CD, (int)(p.BaseMana * 0.55), true, true) {  }
        

        public override bool CanUse()
        {
            return Player.Mana >= Cost && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override void Cast()
        {
            CommonManaSpell();
            DoAction();
        }

        public override void DoAction()
        {
            Player.Resource = 0
                + ((Randomer.NextDouble() < Player.GetTalentPoints("Furor") * 0.2) ? 40 : 0)
                + (Player.Equipment[Player.Slot.Head].Name == "Wolfshead Helm" ? 20 : 0);

            LogAction();
            Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0), Player.Sim.CurrentTime));
        }

        public override string ToString()
        {
            return "Shift";
        }
    }
}

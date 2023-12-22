using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shift : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Shift";

        public static int CD = 0;

        public Shift(Player p)
            : base(p, CD, (int)(p.BaseMana * 0.55 * (1 - p.GetTalentPoints("NS") * 0.1)), true, true, School.Magical, 0, 1, 1, null, null, null) {  }
        

        public override bool CanUse()
        {
            return Player.Mana >= Cost && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override void Cast(Entity t)
        {
            OnCommonSpellCast(Player.Effects.ContainsKey(RuneOfMeta.NAME) ? 0 : Cost);
            DoAction();
            Player.StartGCD();
        }

        public override void DoAction()
        {
            base.DoAction();

            Player.Form = Player.Forms.Cat;
            Player.ResetMHSwing();

            Player.Resource = 0
                + ((Randomer.NextDouble() < Player.GetTalentPoints("Furor") * 0.2) ? 40 : 0)
                + (Player.Equipment[Player.Slot.Head].Name == "Wolfshead Helm" ? 20 : 0);

            LogAction();
            Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class CurseOfAgony : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Curse of Agony";

        public static int BASE_COST = 215;
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public CurseOfAgony(Player p)
            : base(p, CD, BASE_COST, true, true, School.Shadow, CAST_TIME)
        {
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();

            ResultType res = Simulation.MagicMitigationBinary(Player.Sim.Boss.MagicResist[School]);

            if (res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Player.Sim.Boss, false, 0.02 * Player.GetTalentPoints("Suppr"));
            }

            if (res == ResultType.Hit)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                if (Player.Sim.Boss.Effects.ContainsKey(CurseOfAgonyDoT.NAME))
                {
                    Player.Sim.Boss.Effects[CurseOfAgonyDoT.NAME].Refresh();
                }
                else
                {
                    new CurseOfAgonyDoT(Player, Player.Sim.Boss).StartEffect();
                }
            }
            else
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Resist, 0, 0), Player.Sim.CurrentTime));
            }
        }
    }
}

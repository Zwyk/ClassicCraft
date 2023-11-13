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

        public int? _BASE_COST = null;
        public int BASE_COST
        {
            get
            {
                if (!_BASE_COST.HasValue)
                {
                    if (Player.Level >= 58) _BASE_COST = 215;
                    else if (Player.Level >= 48) _BASE_COST = 170;
                    else if (Player.Level >= 38) _BASE_COST = 130;
                    else if (Player.Level >= 28) _BASE_COST = 90;
                    else if (Player.Level >= 18) _BASE_COST = 50;
                    else if (Player.Level >= 8) _BASE_COST = 25;
                    else _BASE_COST = 0;
                }
                return _BASE_COST.Value;
            }
        }
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public CurseOfAgony(Player p)
            : base(p, CD, 0, true, true, School.Shadow, CAST_TIME)
        {
            Cost = BASE_COST;
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();

            LogAction();

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

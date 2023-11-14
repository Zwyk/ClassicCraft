using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Corruption : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Corruption";

        public int BASE_COST(int level)
        {
            if (level >= 60) return 340;
            else if (level >= 54) return 290;
            else if (level >= 44) return 225;
            else if (level >= 34) return 160;
            else if (level >= 24) return 100;
            else if (level >= 14) return 55;
            else if (level >= 4) return 35;
            else return 0;
        }
        public static int CD = 0;
        public static double CAST_TIME = 2;

        public Corruption(Player p)
            : base(p, CD, 0, true, true, School.Shadow, CAST_TIME - 0.4 * p.GetTalentPoints("IC"))
        {
            Cost = BASE_COST(p.Level);
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();

            LogAction();

            ResultType res = Simulation.MagicMitigationBinary(Target.MagicResist[School]);

            if(res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Target, false, 0.02 * Player.GetTalentPoints("Suppr"));
            }
            
            if(res == ResultType.Hit)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                if (Target.Effects.ContainsKey(CorruptionDoT.NAME))
                {
                    Target.Effects[CorruptionDoT.NAME].Refresh();
                }
                else
                {
                    new CorruptionDoT(Player, Target).StartEffect();
                }
            }
            else
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Resist, 0, 0), Player.Sim.CurrentTime));
            }
        }
    }
}

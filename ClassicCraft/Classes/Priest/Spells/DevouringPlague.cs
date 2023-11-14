using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DevouringPlague : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Devouring Plague";

        public static int BASE_COST = 985;
        public static int CD = 180;
        public static double CAST_TIME = 0;

        public DevouringPlague(Player p)
            : base(p, CD, BASE_COST, true, true, School.Shadow, CAST_TIME)
        {
        }

        public override void DoAction()
        {
            base.DoAction();

            bool inner = Player.Effects.ContainsKey(InnerFocusBuff.NAME);

            CommonManaSpell(inner ? 0 : Cost);

            if(inner)
            {
                Player.Effects[InnerFocusBuff.NAME].EndEffect();
            }

            ResultType res = Simulation.MagicMitigationBinary(Target.MagicResist[School]);

            if (res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Target, false, 0.02 * Player.GetTalentPoints("SF"));
            }

            if (res == ResultType.Hit)
            {
                if (Target.Effects.ContainsKey(DevouringPlagueDoT.NAME))
                {
                    Target.Effects[DevouringPlagueDoT.NAME].Refresh();
                }
                else
                {
                    new DevouringPlagueDoT(Player, Target).StartEffect();
                }
            }
        }
    }
}

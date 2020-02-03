using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodrage : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Bloodrage";

        public static int BASE_COST = 0;
        public static int CD = 60;

        public Bloodrage(Player p)
            : base(p, CD, BASE_COST, false)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            Player.Resource += 10;

            if (Player.Effects.ContainsKey(BloodrageBuff.NAME))
            {
                Player.Effects[BloodrageBuff.NAME].Refresh();
            }
            else
            {
                new BloodrageBuff(Player).StartEffect();
            }

            LogAction();
        }

        public override void LogAction()
        {
            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} cast for 10 rage (rage {2})", Player.Sim.CurrentTime, ToString(), Player.Resource));
            }
        }
    }
}

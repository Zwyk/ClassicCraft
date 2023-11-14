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

        public static int BASE_RAGE = 10;

        public int Rage { get; set; }

        public Bloodrage(Player p)
            : base(p, CD, BASE_COST, false)
        {
            Rage = 10 + (p.GetTalentPoints("IBR") > 0 ? (p.GetTalentPoints("IBR") == 1 ? 2 : 5) : 0);
        }

        public override void Cast(Entity t)
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            Player.Resource += Rage;

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

        public override void LogAction(int? threat = null)
        {
            if (threat.HasValue)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Success, 0, threat.Value), Player.Sim.CurrentTime));
            }

            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} cast for 10 rage (rage {2})", Player.Sim.CurrentTime, ToString(), Player.Resource));
            }
        }
    }
}

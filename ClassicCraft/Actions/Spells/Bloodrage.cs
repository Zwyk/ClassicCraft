using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodrage : Spell
    {
        public Bloodrage(Player p, double baseCD = 60, int ressourceCost = 0, bool gcd = false)
            : base(p, baseCD, ressourceCost, gcd)
        {
        }

        public override void Cast()
        {
            CommonSpell();
            DoAction();
        }

        public override void DoAction()
        {
            Player.Ressource += 10;

            if (Player.Effects.Any(e => e is BloodrageBuff))
            {
                Effect current = Player.Effects.Where(e => e is BloodrageBuff).First();
                current.Refresh();
            }
            else
            {
                BloodrageBuff r = new BloodrageBuff(Player);
                r.StartBuff();
            }

            LogAction();
        }

        public override void LogAction()
        {
            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} cast for 10 rage (rage {2})", Player.Sim.CurrentTime, ToString(), Player.Ressource));
            }
        }

        public override string ToString()
        {
            return "Bloodrage";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Flurry : Effect
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Flurry";

        public static int DURATION = 15;
        public static int START_STACKS = 3;

        public double Haste { get; set; }

        public Flurry(Player p)
            : base(p, p, true, DURATION, START_STACKS)
        {
            int points = p.GetTalentPoints(NAME);
            Haste = 1 + (points > 0 ? (Program.version == Version.Vanilla ? 0.1 : 0.05) + (points - 1) * 0.05 : 0);
        }

        public static void CheckProc(Player p, ResultType type, bool noDestack = false)
        {
            int points = p.GetTalentPoints(NAME);
            if (points > 0)
            {
                if (p.Effects.ContainsKey(Flurry.NAME))
                {
                    Effect current = p.Effects[Flurry.NAME];
                    if (type == ResultType.Crit)
                    {
                        current.Refresh();
                    }
                    else if (!noDestack)
                    {
                        current.StackRemove();
                    }
                }
                else if (type == ResultType.Crit)
                {
                    Flurry flu = new Flurry(p);
                    flu.StartEffect();
                }
            }
        }

        public override void StartEffect()
        {
            Target.Effects.Add(NAME, this);
            double haste = Player.HasteMod;
            Player.HasteMod *= Haste;

            if (Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} started", Player.Sim.CurrentTime, ToString()));
            }
        }

        public override void EndEffect()
        {
            if (Target.Effects.ContainsKey(ToString()))
            {
                End = Player.Sim.CurrentTime;
                Target.Effects.Remove(ToString());

                Player.HasteMod /= Haste;

                if (Program.logFight)
                {
                    Program.Log(string.Format("{0:N2} : {1} ended", Player.Sim.CurrentTime, ToString()));
                }
            }
        }
    }
}

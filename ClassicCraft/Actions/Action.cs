using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public enum ResultType
    {
        Miss,
        Dodge,
        Parry,
        Glancing,
        Block,
        Crit,
        Hit,
        Resist
    }

    public class ActionResult
    {
        public ResultType Type { get; set; }
        public int Damage { get; set; }
        public ActionResult(ResultType type, int damage)
        {
            Type = type;
            Damage = damage;
        }
    }

    public abstract class Action : SimulationObject
    {
        public double BaseCD { get; set; }

        public double LockedUntil { get; set; }

        public Action(Simulation s, double baseCD)
            : base(s)
        {
            BaseCD = baseCD;
            LockedUntil = 0;
        }

        public abstract void Cast();

        public abstract void DoAction();

        public void CommonAction()
        {
            LockedUntil = Sim.CurrentTime + BaseCD;
            Sim.Player.StartGCD();
        }

        public bool Available()
        {
            return LockedUntil <= Sim.CurrentTime;
        }

        public double RemainingCD()
        {
            return LockedUntil - Sim.CurrentTime;
        }

        public void RegisterDamage(ActionResult res)
        {
            Sim.RegisterAction(new RegisteredAction(this, res, Sim.CurrentTime));

            if (Program.nbSim < 10)
            {
                Console.WriteLine("{0:N2} : {1} {2} for {3} damage (rage {4})", Sim.CurrentTime, ToString(), res.Type, res.Damage, Sim.Player.Ressource);
            }
        }

        public override string ToString()
        {
            return "Undefined Action";
        }
    }
}

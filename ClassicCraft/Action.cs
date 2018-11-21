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

    public abstract class Action
    {
        public double BaseCD { get; set; }

        public double LockedUntil { get; set; }

        public Action(double baseCD)
        {
            BaseCD = baseCD;
            LockedUntil = 0;
        }

        public abstract void Cast();

        public abstract void DoAction();

        public void CommonAction()
        {
            LockedUntil = Program.currentTime + BaseCD;
            Player.Instance.StartGCD();
        }

        public bool Available()
        {
            return LockedUntil <= Program.currentTime;
        }

        public double RemainingCD()
        {
            return LockedUntil - Program.currentTime;
        }

        public void RegisterDamage(ActionResult res)
        {
            Program.actions.Add(new RegisteredAction(this, res, Program.currentTime));

            if (Program.nbSim < 10)
            {
                Console.WriteLine("{0:N2} : {1} {2}for {3} damage (rage {4})", Program.currentTime, ToString(), res.Type == ResultType.Crit ? "crits " : "", res.Damage, Player.Instance.Ressource);
            }
        }

        public override string ToString()
        {
            return "Undefined Action";
        }
    }
}

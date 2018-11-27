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

    public abstract class Action : PlayerObject
    {
        public double BaseCD { get; set; }

        public double LockedUntil { get; set; }

        public Action(Player p, double baseCD)
            : base(p)
        {
            BaseCD = baseCD;
            LockedUntil = 0;
        }

        public abstract void Cast();

        public abstract void DoAction();

        public void CommonAction()
        {
            CDAction();
            Player.StartGCD();
        }

        public void CDAction()
        {
            LockedUntil = Player.Sim.CurrentTime + BaseCD;
        }

        public bool Available()
        {
            return LockedUntil <= Player.Sim.CurrentTime;
        }

        public double RemainingCD()
        {
            return LockedUntil - Player.Sim.CurrentTime;
        }

        public virtual void RegisterDamage(ActionResult res)
        {
            Player.Sim.RegisterAction(new RegisteredAction(this, res, Player.Sim.CurrentTime));

            if(Program.logFight)
            {
                Program.Log(String.Format("{0:N2} : {1} {2} for {3} damage (rage {4})", Player.Sim.CurrentTime, ToString(), res.Type, res.Damage, Player.Ressource));
            }
        }

        public virtual void LogAction()
        {
            if(Program.logFight)
            {
                Program.Log(String.Format("{0:N2} : {1} cast (rage {2})", Player.Sim.CurrentTime, ToString(), Player.Ressource));
            }
        }

        public override string ToString()
        {
            return "Undefined Action";
        }
    }
}

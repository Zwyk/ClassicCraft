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
        Glance,
        Block,
        Crit,
        Hit,
        Resist,
        Success,
        Failure,
    }

    public enum School
    {
        Physical,
        Magical,
        Shadow,
        Holy,
        Fire,
        Nature,
        Frost,
        Arcane,
    }

    public class ActionResult
    {
        public ResultType Type { get; set; }
        public int Damage { get; set; }
        public int Threat { get; set; }

        public ActionResult(ResultType type, int damage, int threat)
        {
            Type = type;
            Damage = damage;
            Threat = threat;
        }
    }

    public abstract class Action : PlayerObject
    {
        public static string NAME = "Undefined Action";

        public double BaseCD { get; set; }

        public double LockedUntil { get; set; }

        public School School { get; set; }

        public Entity Target { get; set; }

        public Action(Player p, double baseCD, School school = School.Physical)
            : base(p)
        {
            BaseCD = baseCD;
            LockedUntil = 0;
            School = school;
            Target = p?.Target;
        }

        public virtual void Cast(Entity t)
        {
            Target = t;
        }

        public virtual void DoAction() { }

        public abstract bool CanUse();

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
                string log = string.Format("{0:N2} : [{1}] {2}", Player.Sim.CurrentTime, ToString(), res.Type);
                if(res.Type != ResultType.Miss && res.Type != ResultType.Parry && res.Type != ResultType.Resist && res.Type != ResultType.Dodge)
                {
                    log += string.Format(" for {0} damage", res.Damage);
                }
                if(Player.Sim.NbTargets > 1)
                {
                    for(int i = 0;  i < Player.Sim.Boss.Count; i++)
                    {
                        if (Player.Sim.Boss[i] == Target)
                        {
                            log += string.Format(" on Target {0}", i+1);
                        }
                    }
                }
                if (!Player.CurrentMainResource.Equals("mana"))
                {
                    log += string.Format(" ({0} {1}/{2})", Player.CurrentMainResource, Player.Resource, Player.MaxResource);
                }
                if(Player.Form == Player.Forms.Cat || Player.Class == Player.Classes.Rogue)
                {
                    log += " [combo " + Player.Combo + "]";
                }
                if (Player.Mana > 0 && Player.Form != Player.Forms.Bear)
                {
                    log += " - Mana " + Player.Mana + "/" + Player.MaxMana;
                }
                Program.Log(log);
            }
        }

        public virtual void LogAction(int? threat = null)
        {
            if(threat.HasValue)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Success, 0, threat.Value), Player.Sim.CurrentTime));
            }

            if(Program.logFight)
            {
                string log = string.Format("{0:N2} : [{1}] cast", Player.Sim.CurrentTime, ToString());
                if (Player.Sim.NbTargets > 1)
                {
                    for (int i = 0; i < Player.Sim.Boss.Count; i++)
                    {
                        if (Player.Sim.Boss[i] == Target)
                        {
                            log += string.Format(" on Target {0}", i + 1);
                        }
                    }
                }
                if (!Player.CurrentMainResource.Equals("mana"))
                {
                    log += string.Format(" ({0} {1}/{2})", Player.CurrentMainResource, Player.Resource, Player.MaxResource);
                }
                if (Player.Form == Player.Forms.Cat || Player.Class == Player.Classes.Rogue)
                {
                    log += "[combo " + Player.Combo + "]";
                }
                if(Player.Mana > 0)
                {
                    log += " - Mana " + Player.Mana + "/" + Player.MaxMana;
                }
                Program.Log(log);
            }
        }

        public override string ToString()
        {
            return NAME;
        }
    }
}

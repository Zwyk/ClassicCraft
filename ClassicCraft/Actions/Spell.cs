using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Spell : Skill
    {
        public double CastTime { get; set; }
        public double TravelSpeed { get; set; }
        public double CastFinish;
        public double CastTimeWithGCD { get { return Math.Max(CastTime, AffectedByGCD ? Player.GCD_Hasted() : 0); } }

        public Spell(Player p, double baseCD, int resourceCost, bool useMana = false, bool gcd = true, School school = School.Magical, double castTime = 0, double travelSpeed = 0)
            : base(p, baseCD, resourceCost, gcd, useMana, school)
        {
            CastTime = castTime;
            TravelSpeed = travelSpeed;
        }

        public override void Cast(Entity t)
        {
            Target = t;
            StartCast();
        }

        public virtual void StartCast(bool forceInstant = false)
        {
            if(AffectedByGCD)
            {
                Player.StartGCD();
            }
            if (CastTime > 0 && !forceInstant)
            {
                Player.casting = this;
                CastFinish = Player.Sim.CurrentTime + CastTime;
                LogCast();
            }
            else
            {
                DoAction();
            }
        }

        public void CommonManaSpell(int? customCost = null)
        {
            CDAction();

            Player.Mana -= customCost ?? Cost;
        }
        
        public override void DoAction()
        {
            Player.casting = null;

            Player.ResetMHSwing();
        }

        public void LogCast()
        {
            if (Program.logFight)
            {
                string log = string.Format("{0:N2} : [{1}] started cast", Player.Sim.CurrentTime, ToString());
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
                if (!ResourceName().Equals("mana"))
                {
                    log += string.Format(" ({0} {1}/{2})", ResourceName(), Player.Resource, Player.MaxResource);
                }
                if (Player.Form == Player.Forms.Cat || Player.Class == Player.Classes.Rogue)
                {
                    log += " [combo " + Player.Combo + "]";
                }
                if (Player.MaxMana > 0)
                {
                    log += " - Mana " + Player.Mana + "/" + Player.MaxMana;
                }
                Program.Log(log);
            }
        }
    }
}

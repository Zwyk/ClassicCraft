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

        public Spell(Player p, double baseCD, int resourceCost, bool useMana = false, bool gcd = true, double castTime = 0, double travelSpeed = 0)
            : base(p, baseCD, resourceCost, gcd, useMana)
        {
            CastTime = castTime;
            TravelSpeed = travelSpeed;
        }

        public override void Cast()
        {
            Player.StartGCD();
            if(CastTime > 0)
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

        public void CommonManaSpell()
        {
            CDAction();

            Player.Mana -= Cost;
        }

        public override void DoAction()
        {
            if(CastTime > 0)
            {
                Player.casting = null;
            }

            Player.ResetMHSwing();
        }

        public void LogCast()
        {
            if (Program.logFight)
            {
                string log = string.Format("{0:N2} : {1} started cast ({2} {3}/{4})", Player.Sim.CurrentTime, ToString(), ResourceName(), Player.Resource, Player.MaxResource);
                if (Player.Form == Player.Forms.Cat || Player.Class == Player.Classes.Rogue)
                {
                    log += "[combo " + Player.Combo + "]";
                }
                if (Player.Mana > 0)
                {
                    log += " - Mana " + Player.Mana + "/" + Player.MaxMana;
                }
                Program.Log(log);
            }
        }

        public override string ToString()
        {
            return "Undefined Spell";
        }
    }
}

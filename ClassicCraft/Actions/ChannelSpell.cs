using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class ChannelSpell : Spell
    {
        public double TickDelay { get; set; }
        public double NextTick { get; set; }
        public int TickDamage { get; set; }

        public ChannelSpell(Player p, double baseCD, int resourceCost, bool useMana = false, bool gcd = true, School school = School.Magical, double castTime = 0, double travelSpeed = 0, double tickDelay = 1)
            : base(p, baseCD, resourceCost, useMana, gcd, school, castTime, travelSpeed)
        {
            TickDelay = tickDelay;
        }

        public override void StartCast(bool forceInstant = false)
        {
            CommonManaSpell();

            NextTick = Player.Sim.CurrentTime + TickDelay;

            ResultType res = Simulation.MagicMitigationBinary(Target.MagicResist[School]);

            if (res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Target, false, 0.02 * Player.GetTalentPoints("SF"));
            }

            if (res == ResultType.Hit)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                TickDamage = GetTickDamage();
                base.StartCast(forceInstant);
            }
            else
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Resist, 0, 0), Player.Sim.CurrentTime));
            }
        }

        public void CheckTick()
        {
            if (NextTick <= Player.Sim.CurrentTime)
            {
                ApplyTick(TickDamage);
                NextTick += TickDelay;
            }
        }

        public abstract int GetTickDamage();

        public virtual void ApplyTick(int damage)
        {
            //Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, damage), Player.Sim.CurrentTime));
            Player.Sim.RegisterEffect(new RegisteredEffect(new CustomEffect(Player, Target, ToString(), false, 1), damage, Player.Sim.CurrentTime, (int)(damage * Player.ThreatMod)));

            if (Program.logFight)
            {
                string log = string.Format("{0:N2} : {1} ticks for {2} damage", Player.Sim.CurrentTime, ToString(), damage);

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

                Program.Log(log);
            }
        }
    }
}

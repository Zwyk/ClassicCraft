using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class EffectOnTime : Effect
    {
        public int TickDelay { get; set; }
        public double NextTick { get; set; }
        public int TickDamage { get; set; }
        public double Ratio { get; set; }
        
        public School School { get; set; }

        public double BonusDmg { get; set; }

        public EffectOnTime(Player p, Entity target, bool friendly, double baseLength, int baseStacks, double ratio, int tickDelay, int maxStacks, School school = School.Magical)
            : base(p, target, friendly, baseLength, baseStacks, maxStacks)
        {
            Ratio = ratio;
            TickDelay = tickDelay;
            School = school;
            BonusDmg = 0;
        }

        public override void StartEffect()
        {
            base.StartEffect();

            NextTick = Player.Sim.CurrentTime + TickDelay;
            TickDamage = GetTickDamage();
        }

        public override void CheckEffect()
        {
            if (NextTick <= Player.Sim.CurrentTime)
            {
                ApplyTick((int)Math.Round(TickDamage * Player.ExternalModifiers(ToString(), Target, School, ResultType.Hit)));
                NextTick += TickDelay;
            }

            base.CheckEffect();
        }

        public override void Refresh()
        {
            base.Refresh();

            TickDamage = GetTickDamage();
        }

        public virtual double BaseDmg()
        {
            return 0;
        }

        public int GetTickDamage()
        {
            double mitigation = 1;
            return (int)Math.Round((BaseDmg() * CurrentStacks + (School == School.Physical ? Player.AP : Player.SchoolSP(School)) * Ratio) / (Duration / TickDelay)
                * Player.Sim.DamageMod(ResultType.Hit, School)
                * mitigation
                * Player.DamageMod
                * Player.SelfModifiers(ToString(), Target, School, ResultType.Hit)
                );
        }

        public virtual void ApplyTick(int damage)
        {
            Player.Sim.RegisterEffect(new RegisteredEffect(this, damage, Player.Sim.CurrentTime, (int)(damage * Player.ThreatMod)));

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

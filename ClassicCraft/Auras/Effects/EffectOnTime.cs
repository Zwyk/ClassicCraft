﻿using System;
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

        public double ThreatRatio { get; set; }

        public int TicksLeft { get { return (int)Math.Round(RemainingTime() / TickDelay); } }  // meh

        public int TotalTicks { get { return (int)Math.Round(CustomDuration() / TickDelay); } }  // meh

        public EffectOnTime(Player p, Entity target, bool friendly, double baseLength, int baseStacks, double ratio, int tickDelay, int maxStacks, School school, double threatRatio = 1)
            : base(p, target, friendly, baseLength, baseStacks, maxStacks)
        {
            Ratio = ratio;
            TickDelay = tickDelay;
            School = school;
            BonusDmg = 0;
            ThreatRatio = threatRatio;
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

            if(Player.Runes.Contains("Invocation"))
            {
                // remaining ticks damage if remaining < 6sec
            }
        }

        public virtual double BaseDmg()
        {
            return 0;
        }

        public virtual int GetTickDamage()
        {
            ResultType res = ResultType.Hit;

            if(Player.Class == Player.Classes.Warlock && Player.Runes.Contains("Pandemic")
                && new List<String>() { CorruptionDoT.NAME, CurseOfAgonyDoT.NAME }.Contains(ToString())
                && Randomer.NextDouble() < Player.SpellCritChance)
            {
                res = ResultType.Crit;
            }

            return (int)Math.Round((BaseDmg() * CurrentStacks + (School == School.Physical ? Player.AP : Player.SchoolSP(School)) * Ratio) / (CustomDuration() / TickDelay)
                * Player.Sim.DamageMod(res, School)
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

            Player.CheckOnTick(this);
        }
    }
}

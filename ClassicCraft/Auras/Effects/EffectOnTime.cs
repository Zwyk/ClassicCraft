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

        public EffectOnTime(Player p, Entity target, bool friendly, double baseLength, int baseStacks = 1, int tickDelay = 3, int maxStacks = 1)
            : base(p, target, friendly, baseLength, baseStacks, maxStacks)
        {
            TickDelay = tickDelay;
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
                ApplyTick((int)Math.Round(TickDamage * GetExternalModifiers()));
                NextTick += TickDelay;
            }

            base.CheckEffect();
        }

        public override void Refresh()
        {
            base.Refresh();

            TickDamage = GetTickDamage();
        }

        public abstract int GetTickDamage();

        public virtual double GetExternalModifiers()
        {
            return 1;
        }

        public virtual void ApplyTick(int damage)
        {
            Player.Sim.RegisterEffect(new RegisteredEffect(this, damage, Player.Sim.CurrentTime));

            if(Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} ticks for {2} damage", Player.Sim.CurrentTime, ToString(), damage));
            }
        }
    }
}

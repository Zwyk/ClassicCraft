﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class BloodrageBuff : EffectOnTime
    {
        public BloodrageBuff(Player p, bool friendly = true, double baseLength = 10, int baseStacks = 1, int tickDelay = 1)
            : base(p, p, friendly, baseLength, baseStacks, tickDelay)
        {
        }

        public override int GetTickDamage()
        {
            return 0;
        }

        public override void ApplyTick(int damage)
        {
            Player.Resource += 1;

            if(Program.logFight)
            {
                Program.Log(string.Format("{0:N2} : {1} for {2} rage (rage {3}/{4})", Player.Sim.CurrentTime, ToString(), 1, Player.Resource, Player.MaxResource));
            }
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Bloodrage Buff";
    }
}

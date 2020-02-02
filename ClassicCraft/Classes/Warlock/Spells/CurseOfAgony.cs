﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class CurseOfAgony : Spell
    {
        public static int BASE_COST = 215;
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public CurseOfAgony(Player p)
            : base(p, CD, BASE_COST, true, true, School.Shadow, CAST_TIME)
        {
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();

            ResultType res = Simulation.MagicMitigationBinary(Player.Sim.Boss.MagicResist[School]);

            if (res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Player.Sim.Boss, false, 0.02 * Player.GetTalentPoints("Suppr"));
            }

            if (res == ResultType.Hit)
            {
                if (Player.Sim.Boss.Effects.Any(e => e is CurseOfAgonyDoT))
                {
                    Player.Sim.Boss.Effects.Where(e => e is CurseOfAgonyDoT).First().Refresh();
                }
                else
                {
                    new CurseOfAgonyDoT(Player, Player.Sim.Boss).StartEffect();
                }
            }
        }

        public override string ToString()
        {
            return "Curse of Agony";
        }
    }
}

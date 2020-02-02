﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Berserking : Skill
    {
        public static int BASE_COST = 5;
        public static int CD = 180;

        public Berserking(Player p)
            : base(p, CD, BASE_COST, false)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.Any(e => e is BerserkingBuff))
            {
                Effect current = Player.Effects.Where(e => e is BerserkingBuff).First();
                current.Refresh();
            }
            else
            {
                BerserkingBuff r = new BerserkingBuff(Player);
                r.StartEffect();
            }

            LogAction();
        }

        public override string ToString()
        {
            return "Berserking";
        }
    }
}

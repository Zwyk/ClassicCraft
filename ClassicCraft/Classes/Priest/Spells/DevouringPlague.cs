﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DevouringPlague : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Devouring Plague";

        public static int BASE_COST = 985;
        public static int CD = 180;
        public static double CAST_TIME = 0;

        public DevouringPlague(Player p)
            : base(p, CD, School.Shadow,
                  new SpellData(SpellType.Magical, BASE_COST, true, CAST_TIME, SMI.Reset),
                  null,
                  new EndEffect(DevouringPlagueDoT.NAME))
        {
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Corruption : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Corruption";

        public int? _BASE_COST = null;
        public int BASE_COST
        {
            get
            {
                if (!_BASE_COST.HasValue)
                {
                    if (Player.Level >= 60) _BASE_COST = 340;
                    else if (Player.Level >= 54) _BASE_COST = 290;
                    else if (Player.Level >= 44) _BASE_COST = 225;
                    else if (Player.Level >= 34) _BASE_COST = 160;
                    else if (Player.Level >= 24) _BASE_COST = 100;
                    else if (Player.Level >= 14) _BASE_COST = 55;
                    else if (Player.Level >= 4) _BASE_COST = 35;
                    else _BASE_COST = 0;
                }
                return _BASE_COST.Value;
            }
        }
        public static int CD = 0;
        public static double CAST_TIME = 2;

        public Corruption(Player p)
            : base(p, CD, 0, true, true, School.Shadow, CAST_TIME - 0.4 * p.GetTalentPoints("IC"))
        {
            Cost = BASE_COST;
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();

            ResultType res = Simulation.MagicMitigationBinary(Player.Sim.Boss.MagicResist[School]);

            if(res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Player.Sim.Boss, false, 0.02 * Player.GetTalentPoints("Suppr"));
            }
            
            if(res == ResultType.Hit)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                if (Player.Sim.Boss.Effects.ContainsKey(CorruptionDoT.NAME))
                {
                    Player.Sim.Boss.Effects[CorruptionDoT.NAME].Refresh();
                }
                else
                {
                    new CorruptionDoT(Player, Player.Sim.Boss).StartEffect();
                }
            }
            else
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Resist, 0, 0), Player.Sim.CurrentTime));
            }
        }
    }
}

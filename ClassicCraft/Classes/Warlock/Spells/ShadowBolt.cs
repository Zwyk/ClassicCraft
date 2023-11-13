﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowBolt : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Shadow Bolt";

        public static int CD = 0;

        public double RATIO = 0;

        public int? _BASE_COST = null;
        public int BASE_COST
        {
            get
            {
                if (!_BASE_COST.HasValue)
                {
                    if (Player.Level >= 60) _BASE_COST = 380;
                    //else if (Player.Level >= 60) _BASE_COST = 370; // Rank 9
                    else if (Player.Level >= 52) _BASE_COST = 315;
                    else if (Player.Level >= 44) _BASE_COST = 265;
                    else if (Player.Level >= 36) _BASE_COST = 210;
                    else if (Player.Level >= 28) _BASE_COST = 160;
                    else if (Player.Level >= 20) _BASE_COST = 110;
                    else if (Player.Level >= 12) _BASE_COST = 70;
                    else if (Player.Level >= 6) _BASE_COST = 40;
                    else _BASE_COST = 25;
                }
                return _BASE_COST.Value;
            }
        }

        public static double CAST_TIME(int level)
        {
            if (level >= 20) return 3;
            else if (level >= 12) return 2.8;
            else if (level >= 6) return 2.2;
            else return 1.7;
        }

        public int? _MIN_DMG = null;
        public int MIN_DMG
        {
            get
            {
                if (!_MIN_DMG.HasValue)
                {
                    if (Player.Level >= 60) _MIN_DMG = 482;
                    //else if (Player.Level >= 60) _MIN_DMG = 455; // Rank 9
                    else if (Player.Level >= 52) _MIN_DMG = 373;
                    else if (Player.Level >= 44) _MIN_DMG = 292;
                    else if (Player.Level >= 36) _MIN_DMG = 213;
                    else if (Player.Level >= 28) _MIN_DMG = 150;
                    else if (Player.Level >= 20) _MIN_DMG = 92;
                    else if (Player.Level >= 12) _MIN_DMG = 52;
                    else if (Player.Level >= 6) _MIN_DMG = 26;
                    else _MIN_DMG = 13;
                }
                return _MIN_DMG.Value;
            }
        }

        public int? _MAX_DMG;
        public int MAX_DMG
        {
            get
            {
                if (!_MAX_DMG.HasValue)
                {
                    if (Player.Level >= 60) _MAX_DMG = 538;
                    //else if (Player.Level >= 60) _MAX_DMG = 507; // Rank 9
                    else if (Player.Level >= 52) _MAX_DMG = 415;
                    else if (Player.Level >= 44) _MAX_DMG = 327;
                    else if (Player.Level >= 36) _MAX_DMG = 240;
                    else if (Player.Level >= 28) _MAX_DMG = 170;
                    else if (Player.Level >= 20) _MAX_DMG = 104;
                    else if (Player.Level >= 12) _MAX_DMG = 61;
                    else if (Player.Level >= 6) _MAX_DMG = 32;
                    else _MAX_DMG = 18;
                }
                return _MAX_DMG.Value;
            }
        }

        public static int VOLLEEY_MAX_TARGETS = 5;

        public double castTimeKeeper;

        public ShadowBolt(Player p)
            : base(p, CD, 0, true, true, School.Shadow, 0)
        {
            Cost = (int)(BASE_COST * 1 - (0.01 * p.GetTalentPoints("Cata")));
            double baseCast = CAST_TIME(p.Level);
            CastTime = baseCast - 0.1 * p.GetTalentPoints("ISB");
            RATIO = Math.Max(1.5, baseCast) / 3.5;

            castTimeKeeper = CastTime;
        }

        public override void Cast()
        {
            bool st = Player.Effects.ContainsKey(ShadowTrance.NAME);
            StartCast(st);
            if(st) Player.Effects[ShadowTrance.NAME].EndEffect();
        }

        public override void DoAction()
        {
            base.DoAction();

            CommonManaSpell();

            ResultType res;
            int minDmg = MIN_DMG;
            int maxDmg = MAX_DMG;

            int nbt = 1; // Shadow Volley

            for (int i = 1; i <= nbt; i++)
            {
                double mitigation = Simulation.MagicMitigation(Player.Sim.Boss.ResistChances[School]);
                if (mitigation == 0)
                {
                    res = ResultType.Resist;
                }
                else
                {
                    res = Player.SpellAttackEnemy(Player.Sim.Boss, true, 0, 0.02 * Player.GetTalentPoints("Deva"));
                }

                int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                    * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.5 * Player.GetTalentPoints("Ruin") : 0))
                    * (1 + 0.02 * Player.GetTalentPoints("SM"))
                    * (1 + 0.15 * Player.GetTalentPoints("DS"))
                    * (1 + 0.03 * Player.GetTalentPoints("SL"))
                    * (Player.Sim.Boss.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Player.Sim.Boss.Effects[ShadowVulnerability.NAME]).Modifier : 1)
                    * (Player.Sim.Boss.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                    * mitigation
                    * Player.DamageMod
                    );

                ShadowVulnerability.CheckProc(Player, this, res);
                RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
            }
        }
    }
}

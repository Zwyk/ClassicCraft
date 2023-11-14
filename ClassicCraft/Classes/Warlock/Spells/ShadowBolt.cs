using System;
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

        public int BASE_COST(int level)
        {
            if (level >= 60) return 380;
            //else if (level >= 60) return 370; // Rank 9
            else if (level >= 52) return 315;
            else if (level >= 44) return 265;
            else if (level >= 36) return 210;
            else if (level >= 28) return 160;
            else if (level >= 20) return 110;
            else if (level >= 12) return 70;
            else if (level >= 6) return 40;
            else return 25;
        }

        public static double CAST_TIME(int level)
        {
            if (level >= 20) return 3;
            else if (level >= 12) return 2.8;
            else if (level >= 6) return 2.2;
            else return 1.7;
        }

        public int MIN_DMG(int level)
        {
            if (level >= 60) return 482;
            //else if (level >= 60) return 455; // Rank 9
            else if (level >= 52) return 373;
            else if (level >= 44) return 292;
            else if (level >= 36) return 213;
            else if (level >= 28) return 150;
            else if (level >= 20) return 92;
            else if (level >= 12) return 52;
            else if (level >= 6) return 26;
            else return 13;
        }

        public int MAX_DMG(int level)
        {
            if (level >= 60) return 538;
            //else if (level >= 60) return 507; // Rank 9
            else if (level >= 52) return 415;
            else if (level >= 44) return 327;
            else if (level >= 36) return 240;
            else if (level >= 28) return 170;
            else if (level >= 20) return 104;
            else if (level >= 12) return 61;
            else if (level >= 6) return 32;
            else return 18;
        }

        public static int VOLLEEY_MAX_TARGETS = 5;

        public double castTimeKeeper;

        public ShadowBolt(Player p)
            : base(p, CD, 0, true, true, School.Shadow, 0)
        {
            Cost = (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata")));
            double baseCast = CAST_TIME(p.Level);
            CastTime = baseCast - 0.1 * p.GetTalentPoints("ISB");
            RATIO = Math.Max(1.5, baseCast) / 3.5;

            castTimeKeeper = CastTime;
        }

        public override void Cast(Entity t)
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
            int minDmg = MIN_DMG(Player.Level);
            int maxDmg = MAX_DMG(Player.Level);

            int nbt = 1; // Shadow Volley

            for (int i = 1; i <= nbt; i++)
            {
                double mitigation = Simulation.MagicMitigation(Player.Target.ResistChances[School]);
                if (mitigation == 0)
                {
                    res = ResultType.Resist;
                }
                else
                {
                    res = Player.SpellAttackEnemy(Player.Target, true, 0, 0.02 * Player.GetTalentPoints("Deva"));
                }

                int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                    * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.5 * Player.GetTalentPoints("Ruin") : 0))
                    * (1 + 0.02 * Player.GetTalentPoints("SM"))
                    * Math.Max(Player.Tanking ? 0 : (1 + 0.15 * Player.GetTalentPoints("DS")), 1 + 0.02 * Player.GetTalentPoints("MD") * (1 + 0.03 * Player.GetTalentPoints("SL")))
                    * (Player.Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Player.Target.Effects[ShadowVulnerability.NAME]).Modifier : 1)
                    * (Player.Target.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                    * mitigation
                    * Player.DamageMod
                    );

                ShadowVulnerability.CheckProc(Player, this, res);
                RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
            }
        }
    }
}

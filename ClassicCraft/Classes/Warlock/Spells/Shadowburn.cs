using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shadowburn : Spell
    {
        public override string ToString() { return NAME; } public static new string NAME = "Shadowburn";

        public static int CD = 15;
        public static double CAST_TIME = 0;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public int BASE_COST(int level)
        {
            if (level >= 56) return 365;
            else if (level >= 48) return 305;
            else if (level >= 40) return 245;
            else if (level >= 32) return 190;
            else if (level >= 24) return 130;
            else return 105;
        }

        public int MIN_DMG(int level)
        {
            if (level >= 56) return 462;
            else if (level >= 48) return 365;
            else if (level >= 40) return 274;
            else if (level >= 32) return 196;
            else if (level >= 24) return 123;
            else return 91;
        }

        public int MAX_DMG(int level)
        {
            if (level >= 56) return 514;
            else if (level >= 48) return 408;
            else if (level >= 40) return 307;
            else if (level >= 32) return 221;
            else if (level >= 24) return 140;
            else return 104;
        }

        public Shadowburn(Player p)
            : base(p, CD, 0, true, true, School.Shadow, CAST_TIME)
        {
            Cost = (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata")));
        }

        public override void DoAction()
        {
            base.DoAction();

            ResultType res;
            double mitigation = Simulation.MagicMitigation(Target.ResistChances[School]);
            if (mitigation == 0)
            {
                res = ResultType.Resist;
            }
            else
            {
                res = Player.SpellAttackEnemy(Target, true, 0, 0.02 * Player.GetTalentPoints("Deva"));
            }

            CommonManaSpell();

            int minDmg = MIN_DMG(Player.Level);
            int maxDmg = MAX_DMG(Player.Level);

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.5 * Player.GetTalentPoints("Ruin") : 0))
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * Math.Max(Player.Tanking ? 0 : (1 + 0.15 * Player.GetTalentPoints("DS")), 1 + 0.02 * Player.GetTalentPoints("MD") * (1 + 0.03 * Player.GetTalentPoints("SL")))
                * (Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Target.Effects[ShadowVulnerability.NAME]).Modifier : 1)
                * (Target.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                * mitigation
                * Player.DamageMod
                );

            ShadowVulnerability.CheckProc(Player, this, res);

            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
        }
    }
}

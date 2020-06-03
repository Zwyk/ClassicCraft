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

        public static int BASE_COST = 365;
        public static int CD = 0;
        public static double CAST_TIME = 0;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public static int MIN_DMG = 462;
        public static int MAX_DMG = 514;

        public Shadowburn(Player p)
            : base(p, CD, (int)(BASE_COST * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, true, School.Shadow, CAST_TIME)
        {
        }

        public override void DoAction()
        {
            base.DoAction();

            ResultType res;
            double mitigation = Simulation.MagicMitigation(Player.Sim.Boss.ResistChances[School]);
            if (mitigation == 0)
            {
                res = ResultType.Resist;
            }
            else
            {
                res = Player.SpellAttackEnemy(Player.Sim.Boss, true, 0, 0.02 * Player.GetTalentPoints("Deva"));
            }

            CommonManaSpell();

            int minDmg = MIN_DMG;
            int maxDmg = MAX_DMG;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.5 * Player.GetTalentPoints("Ruin") : 0))
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * (1 + 0.15 * Player.GetTalentPoints("DS"))
                * (Player.Sim.Boss.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Player.Sim.Boss.Effects[ShadowVulnerability.NAME]).Modifier : 1)
                * (Player.Sim.Boss.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                * mitigation
                * Player.DamageMod
                );

            ShadowVulnerability.CheckProc(Player, this, res);

            RegisterDamage(new ActionResult(res, damage));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SearingPain : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Searing Pain";

        public static int BASE_COST(int level)
        {
            if (level >= 60) return 168;
            else if (level >= 50) return 141;
            else if (level >= 42) return 118;
            else if (level >= 34) return 91;
            else if (level >= 26) return 68;
            else if (level >= 18) return 45;
            else return 0;
        }

        public static int CD = 0;
        public static double CAST_TIME = 1.5;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public int MIN_DMG(int level)
        {
            if (level >= 60) return 208;
            else if (level >= 50) return 168;
            else if (level >= 42) return 131;
            else if (level >= 34) return 93;
            else if (level >= 26) return 65;
            else if (level >= 18) return 38;
            else return 0;
        }

        public int MAX_DMG(int level)
        {
            if (level >= 60) return 244;
            else if (level >= 50) return 199;
            else if (level >= 42) return 155;
            else if (level >= 34) return 112;
            else if (level >= 26) return 77;
            else if (level >= 18) return 47;
            else return 0;
        }

        public double castTimeKeeper;

        public SearingPain(Player p)
            : base(p, CD, (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, true, School.Fire, CAST_TIME)
        {
            castTimeKeeper = CastTime;
        }

        public override void Cast(Entity t)
        {
            StartCast(Player.Form == Player.Forms.Metamorphosis);
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
                res = Player.SpellAttackEnemy(Target, true, 0, 0.01 * Player.GetTalentPoints("Deva") + 0.02 * Player.GetTalentPoints("ISP"));
            }

            CommonManaSpell();

            int minDmg = MIN_DMG(Player.Level);
            int maxDmg = MAX_DMG(Player.Level);

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.5 * Player.GetTalentPoints("Ruin") : 0))
                * Math.Max(1 + 0.15 * Player.GetTalentPoints("DS"), (1 + 0.02 * Player.GetTalentPoints("MD")) * (1 + 0.03 * Player.GetTalentPoints("SL")))
                * (1 + 0.02 * Player.GetTalentPoints("Emberstorm"))
                * (Target.Effects.ContainsKey("Improved Scorch") ? 1.15 : 1)
                * mitigation
                * Player.DamageMod
                );

            ShadowVulnerability.CheckProc(Player, this, res);
            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod * 2)));
        }
    }
}

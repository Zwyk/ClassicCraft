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
            int _BASE_COST;
            if (level >= 60) _BASE_COST = 168;
            else if (level >= 50) _BASE_COST = 141;
            else if (level >= 42) _BASE_COST = 118;
            else if (level >= 34) _BASE_COST = 91;
            else if (level >= 26) _BASE_COST = 68;
            else if (level >= 18) _BASE_COST = 45;
            else _BASE_COST = 0;
            return _BASE_COST;
        }

        public static int CD = 0;
        public static double CAST_TIME = 1.5;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public int MIN_DMG(int level)
        {
            int _MIN_DMG;
            if (level >= 60) _MIN_DMG = 208;
            else if (level >= 50) _MIN_DMG = 168;
            else if (level >= 42) _MIN_DMG = 131;
            else if (level >= 34) _MIN_DMG = 93;
            else if (level >= 26) _MIN_DMG = 65;
            else if (level >= 18) _MIN_DMG = 38;
            else _MIN_DMG = 0;
            return _MIN_DMG;
        }

        public int MAX_DMG(int level)
        {
            int _MAX_DMG;
            if (level >= 60) _MAX_DMG = 244;
            else if (level >= 50) _MAX_DMG = 199;
            else if (level >= 42) _MAX_DMG = 155;
            else if (level >= 34) _MAX_DMG = 112;
            else if (level >= 26) _MAX_DMG = 77;
            else if (level >= 18) _MAX_DMG = 47;
            else _MAX_DMG = 0;
            return _MAX_DMG;
        }

        public double castTimeKeeper;

        public SearingPain(Player p)
            : base(p, CD, 0, true, true, School.Fire, CAST_TIME)
        {
            Cost = (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata")));
            castTimeKeeper = CastTime;
        }

        public override void Cast()
        {
            StartCast(Player.Form == Player.Forms.Metamorphosis);
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
                res = Player.SpellAttackEnemy(Player.Sim.Boss, true, 0, 0.01 * Player.GetTalentPoints("Deva") + 0.02 * Player.GetTalentPoints("ISP"));
            }

            CommonManaSpell();

            int minDmg = MIN_DMG(Player.Level);
            int maxDmg = MAX_DMG(Player.Level);

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.5 * Player.GetTalentPoints("Ruin") : 0))
                * (1 + 0.15 * Player.GetTalentPoints("DS"))
                * (1 + 0.02 * Player.GetTalentPoints("MD"))
                * (1 + 0.03 * Player.GetTalentPoints("SL"))
                * (1 + 0.02 * Player.GetTalentPoints("Emberstorm"))
                * (Player.Sim.Boss.Effects.ContainsKey("Improved Scorch") ? 1.15 : 1)
                * mitigation
                * Player.DamageMod
                );

            ShadowVulnerability.CheckProc(Player, this, res);
            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod * 2)));
        }
    }
}

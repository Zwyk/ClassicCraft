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

        public static int CD = 0;
        public static double CAST_TIME = 0;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public int? _BASE_COST = null;
        public int BASE_COST
        {
            get
            {
                if (!_BASE_COST.HasValue)
                {
                    if (Player.Level >= 56) _BASE_COST = 365;
                    else if (Player.Level >= 48) _BASE_COST = 305;
                    else if (Player.Level >= 40) _BASE_COST = 245;
                    else if (Player.Level >= 32) _BASE_COST = 190;
                    else if (Player.Level >= 24) _BASE_COST = 130;
                    else _BASE_COST = 105;
                }
                return _BASE_COST.Value;
            }
        }

        public int? _MIN_DMG = null;
        public int MIN_DMG
        {
            get
            {
                if (!_MIN_DMG.HasValue)
                {
                    if (Player.Level >= 56) _MIN_DMG = 462;
                    else if (Player.Level >= 48) _MIN_DMG = 365;
                    else if (Player.Level >= 40) _MIN_DMG = 274;
                    else if (Player.Level >= 32) _MIN_DMG = 196;
                    else if (Player.Level >= 24) _MIN_DMG = 123;
                    else _MIN_DMG = 91;
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
                    if (Player.Level >= 56) _MAX_DMG = 514;
                    else if (Player.Level >= 48) _MAX_DMG = 408;
                    else if (Player.Level >= 40) _MAX_DMG = 307;
                    else if (Player.Level >= 32) _MAX_DMG = 221;
                    else if (Player.Level >= 24) _MAX_DMG = 140;
                    else _MAX_DMG = 104;
                }
                return _MAX_DMG.Value;
            }
        }

        public Shadowburn(Player p)
            : base(p, CD, 0, true, true, School.Shadow, CAST_TIME)
        {
            Cost = (int)(BASE_COST * 1 - (0.01 * p.GetTalentPoints("Cata")));
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

            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
        }
    }
}

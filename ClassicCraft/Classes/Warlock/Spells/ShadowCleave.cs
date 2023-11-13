using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowCleave : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shadow Cleave";

        public static int CD = 0;

        public double RATIO = 0;

        public int? _BASE_COST = null;
        public int BASE_COST
        {
            get
            {
                if (!_BASE_COST.HasValue)
                {
                    if (Player.Level >= 60) _BASE_COST = 190;
                    //else if (Player.Level >= 60) _BASE_COST = 185; // Rank 9
                    else if (Player.Level >= 52) _BASE_COST = 157;
                    else if (Player.Level >= 44) _BASE_COST = 132;
                    else if (Player.Level >= 36) _BASE_COST = 105;
                    else if (Player.Level >= 28) _BASE_COST = 80;
                    else if (Player.Level >= 20) _BASE_COST = 55;
                    else if (Player.Level >= 12) _BASE_COST = 35;
                    else if (Player.Level >= 6) _BASE_COST = 20;
                    else _BASE_COST = 12;
                }
                return _BASE_COST.Value;
            }
        }

        public double CAST_TIME = 0;

        public int? _MIN_DMG = null;
        public int MIN_DMG
        {
            get
            {
                if (!_MIN_DMG.HasValue)
                {
                    if (Player.Level >= 60) _MIN_DMG = 136;
                    //else if (Player.Level >= 60) _MIN_DMG = 128; // Rank 9
                    else if (Player.Level >= 52) _MIN_DMG = 105;
                    else if (Player.Level >= 44) _MIN_DMG = 82;
                    else if (Player.Level >= 36) _MIN_DMG = 60;
                    else if (Player.Level >= 28) _MIN_DMG = 42;
                    else if (Player.Level >= 20) _MIN_DMG = 26;
                    else if (Player.Level >= 12) _MIN_DMG = 14;
                    else if (Player.Level >= 6) _MIN_DMG = 7;
                    else _MIN_DMG = 3;
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
                    if (Player.Level >= 60) _MAX_DMG = 204;
                    //else if (Player.Level >= 60) _MAX_DMG = 193; // Rank 9
                    else if (Player.Level >= 52) _MAX_DMG = 158;
                    else if (Player.Level >= 44) _MAX_DMG = 124;
                    else if (Player.Level >= 36) _MAX_DMG = 91;
                    else if (Player.Level >= 28) _MAX_DMG = 64;
                    else if (Player.Level >= 20) _MAX_DMG = 39;
                    else if (Player.Level >= 12) _MAX_DMG = 23;
                    else if (Player.Level >= 6) _MAX_DMG = 12;
                    else _MAX_DMG = 7;
                }
                return _MAX_DMG.Value;
            }
        }

        public static int MAX_TARGETS = 3;

        public double castTimeKeeper;

        public ShadowCleave(Player p)
            : base(p, CD, 0, true, true, School.Shadow, 0)
        {
            Cost = (int)(BASE_COST * 1 - (0.01 * p.GetTalentPoints("Cata")));
            CastTime = CAST_TIME;
            RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

            castTimeKeeper = CastTime;
        }

        public override void Cast()
        {
            bool st = Player.Effects.ContainsKey(ShadowTrance.NAME);
            StartCast(st);
            if (st) Player.Effects[ShadowTrance.NAME].EndEffect();
        }

        public override void DoAction()
        {
            base.DoAction();

            CommonManaSpell();

            ResultType res;
            int minDmg = MIN_DMG;
            int maxDmg = MAX_DMG;

            for (int i = 1; i <= Math.Min(MAX_TARGETS, Player.Sim.NbTargets); i++)
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

                RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
            }
        }
    }
}

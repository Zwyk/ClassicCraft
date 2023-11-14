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

        public static int CD = 6;

        public double RATIO = 0;

        public int BASE_COST(int level)
        {
            if (level >= 60) return 190;
            //else if (level >= 60) return 185; // Rank 9
            else if (level >= 52) return 157;
            else if (level >= 44) return 132;
            else if (level >= 36) return 105;
            else if (level >= 28) return 80;
            else if (level >= 20) return 55;
            else if (level >= 12) return 35;
            else if (level >= 6) return 20;
            else return 12;
        }

        public double CAST_TIME = 0;

        public int MIN_DMG(int level)
        {
            if (level >= 60) return 136;
            //else if (level >= 60) return 128; // Rank 9
            else if (level >= 52) return 105;
            else if (level >= 44) return 82;
            else if (level >= 36) return 60;
            else if (level >= 28) return 42;
            else if (level >= 20) return 26;
            else if (level >= 12) return 14;
            else if (level >= 6) return 7;
            else return 3;
        }

        public int MAX_DMG(int level)
        {
            if (level >= 60) return 204;
            //else if (level >= 60) return 193; // Rank 9
            else if (level >= 52) return 158;
            else if (level >= 44) return 124;
            else if (level >= 36) return 91;
            else if (level >= 28) return 64;
            else if (level >= 20) return 39;
            else if (level >= 12) return 23;
            else if (level >= 6) return 12;
            else return 7;
        }

        public static int MAX_TARGETS = 3;

        public double castTimeKeeper;

        public ShadowCleave(Player p)
            : base(p, CD, 0, true, true, School.Shadow, 0)
        {
            Cost = (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata")));
            CastTime = CAST_TIME;
            RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

            castTimeKeeper = CastTime;
        }

        public override void Cast(Entity t)
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
            int minDmg = MIN_DMG(Player.Level);
            int maxDmg = MAX_DMG(Player.Level);

            Entity t = Target;
            for (int i = 0; i < Math.Min(MAX_TARGETS, Player.Sim.NbTargets); i++)
            {
                Target = Player.Sim.Boss[i];
                double mitigation = Simulation.MagicMitigation(Target.ResistChances[School]);
                if (mitigation == 0)
                {
                    res = ResultType.Resist;
                }
                else
                {
                    res = Player.SpellAttackEnemy(Target, true, 0, 0.02 * Player.GetTalentPoints("Deva"));
                }

                int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                    * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.5 * Player.GetTalentPoints("Ruin") : 0))
                    * (1 + 0.02 * Player.GetTalentPoints("SM"))
                    * Math.Max(Player.Tanking ? 0 : (1 + 0.15 * Player.GetTalentPoints("DS")), 1 + 0.02 * Player.GetTalentPoints("MD") * (1 + 0.03 * Player.GetTalentPoints("SL")))
                    * (Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Target.Effects[ShadowVulnerability.NAME]).Modifier : 1)
                    * (Target.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                    * mitigation
                    * Player.DamageMod
                    );

                RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
            }
            Target = t;
        }
    }
}

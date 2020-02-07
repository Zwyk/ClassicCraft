using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Frostbolt : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Frostbolt";

        public static int BASE_COST = 260;
        public static int CD = 0;
        public static double CAST_TIME = 3;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public static int MIN_DMG = 440;
        public static int MAX_DMG = 475;

        public Frostbolt(Player p)
            : base(p, CD, (int)(BASE_COST * 1 - (0.05 * p.GetTalentPoints("FC"))), true, true, School.Frost, CAST_TIME - 0.1 * p.GetTalentPoints("IFB"))
        {
        }

        public override void Cast()
        {
            StartCast(Player.Effects.ContainsKey(PresenceOfMindEffect.NAME));
            Player.Effects[PresenceOfMindEffect.NAME].EndEffect();
        }

        public override void DoAction()
        {
            base.DoAction();

            double winterChillBonusCrit = 0.1;

            ResultType res;
            double mitigation = Simulation.MagicMitigation(Player.Sim.Boss.ResistChances[School]);
            if (mitigation == 0)
            {
                res = ResultType.Resist;
            }
            else
            {
                res = Player.SpellAttackEnemy(Player.Sim.Boss, true, 0.02 * Player.GetTalentPoints("EP"), winterChillBonusCrit);
            }

            CommonManaSpell();

            int minDmg = MIN_DMG;
            int maxDmg = MAX_DMG;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                * (Player.Sim.DamageMod(res, School) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("IS") : 0))
                * (1 + 0.02 * Player.GetTalentPoints("PI"))
                * mitigation
                * Player.DamageMod
                );

            ShadowVulnerability.CheckProc(Player, this, res);
            RegisterDamage(new ActionResult(res, damage));
        }
    }
}

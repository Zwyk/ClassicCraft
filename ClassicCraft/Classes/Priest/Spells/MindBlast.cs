using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MindBlast : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Mind Blast";

        public static int BASE_COST = 350;
        public static int CD = 8;
        public static double CAST_TIME = 1.5;

        public static double RATIO = Math.Max(1.5, CAST_TIME) / 3.5;

        public static int MIN_DMG = 508;
        public static int MAX_DMG = 537;

        public MindBlast(Player p)
            : base(p, CD - 0.5 * p.GetTalentPoints("IMB"), BASE_COST, true, true, School.Shadow, CAST_TIME)
        {
        }

        public override void Cast(Entity t)
        {
            StartCast();
        }

        public override void DoAction()
        {
            base.DoAction();

            bool inner = Player.Effects.ContainsKey(InnerFocusBuff.NAME);

            ResultType res;
            double mitigation = Simulation.MagicMitigation(Target.ResistChances[School]);
            if (mitigation == 0)
            {
                res = ResultType.Resist;
            }
            else
            {
                res = Player.SpellAttackEnemy(Target, true, 0.02 * Player.GetTalentPoints("SF"), inner ? 0.25 : 0);
            }

            CommonManaSpell(inner ? 0 : Cost);

            if(inner)
            {
                Player.Effects[InnerFocusBuff.NAME].EndEffect();
            }

            int minDmg = MIN_DMG;
            int maxDmg = MAX_DMG;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Player.SP * RATIO))
                * Player.Sim.DamageMod(res, School)
                * (1 + 0.02 * Player.GetTalentPoints("Darkness"))
                * 1.15 // shadow weaving
                * 1.15 // shadow form
                * mitigation
                * Player.DamageMod
                );

            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DrainLife : ChannelSpell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Drain Life";

        public static double RUNE_COST_RATIO = 2;
        public static int CD = 0;
        public static int RUNE_CD = 0;
        public static double CAST_TIME = 5;
        public static double RUNE_CAST_TIME = 5;
        public static double TICK_DELAY = 1;
        public static int NB_TICKS = (int)(CAST_TIME / TICK_DELAY);

        public static double RATIO = 0.5;

        public static int BASE_COST(int level)
        {
            if (level >= 54) return 300;
            else if (level >= 46) return 240;
            else if (level >= 38) return 185;
            else if (level >= 30) return 135;
            else if (level >= 22) return 85;
            else if (level >= 14) return 55;
            else return 0;
        }

        public static int DMG(int level)
        {
            if (level >= 54) return 71 * 15;
            else if (level >= 46) return 55 * 15;
            else if (level >= 38) return 41 * 15;
            else if (level >= 30) return 29 * 15;
            else if (level >= 22) return 17 * 15;
            else if (level >= 14) return 10 * 15;
            else return 0;
        }

        public DrainLife(Player p)
            : base(p, p.Runes.Contains("Master Channeler") ? RUNE_CD : CD, (int)(BASE_COST(p.Level) * (p.Runes.Contains("Master Channeler") ? RUNE_COST_RATIO : 1)), true, true, School.Shadow, p.Form == Player.Forms.Metamorphosis ? RUNE_CAST_TIME : CAST_TIME)
        {
        }

        public override void Cast(Entity t)
        {
            if(Player.Form == Player.Forms.Metamorphosis)
            {
                Player.StartGCD();
                DoAction();
            }
            else
            {
                StartCast();
            }
        }

        public override void DoAction()
        {
            base.DoAction();
            CommonManaSpell();

            LogAction();

            ResultType res = Simulation.MagicMitigationBinary(Target.MagicResist[School]);

            if (res == ResultType.Hit)
            {
                res = Player.SpellAttackEnemy(Target, false, 0.02 * Player.GetTalentPoints("Suppr"));
            }

            if (res == ResultType.Hit)
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Hit, 0, 0), Player.Sim.CurrentTime));
                if (Target.Effects.ContainsKey(DrainLifeDoT.NAME))
                {
                    Target.Effects[DrainLifeDoT.NAME].Refresh();
                }
                else
                {
                    new DrainLifeDoT(Player, Target).StartEffect();
                }
            }
            else
            {
                Player.Sim.RegisterAction(new RegisteredAction(this, new ActionResult(ResultType.Resist, 0, 0), Player.Sim.CurrentTime));
            }
        }

        public override int GetTickDamage()
        {
            double mitigation = 1;
            return (int)Math.Round((DMG(Player.Level) + Player.SchoolSP(School) * RATIO) / NB_TICKS
                * Player.Sim.DamageMod(ResultType.Hit, School)
                * mitigation
                * Player.DamageMod
                * Player.TotalModifiers(NAME, Target, School, ResultType.Hit));
        }
    }
}

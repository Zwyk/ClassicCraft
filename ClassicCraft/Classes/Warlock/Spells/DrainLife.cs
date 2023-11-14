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

        public static double RUNE_COST_RATIO = 1.5;
        public static int CD = 0;
        public static int RUNE_CD = 0;
        public static double CAST_TIME = 5;
        public static double RUNE_CAST_TIME = 5;
        public static double TICK_DELAY = 1;
        public static int NB_TICKS = (int)(CAST_TIME / TICK_DELAY);

        public static double RATIO = 0.5;

        public int? _BASE_COST = null;
        public int BASE_COST
        {
            get
            {
                if (!_BASE_COST.HasValue)
                {
                    if (Player.Level >= 54) _BASE_COST = 300;
                    else if (Player.Level >= 46) _BASE_COST = 240;
                    else if (Player.Level >= 38) _BASE_COST = 185;
                    else if (Player.Level >= 30) _BASE_COST = 135;
                    else if (Player.Level >= 22) _BASE_COST = 85;
                    else if (Player.Level >= 14) _BASE_COST = 55;
                    else _BASE_COST = 0;
                }
                return _BASE_COST.Value;
            }
        }

        public int? _DMG;
        public int DMG
        {
            get
            {
                if (!_DMG.HasValue)
                {
                    if (Player.Level >= 54) _DMG = 71*5;
                    else if (Player.Level >= 46) _DMG = 55*5;
                    else if (Player.Level >= 38) _DMG = 41*5;
                    else if (Player.Level >= 30) _DMG = 29*5;
                    else if (Player.Level >= 22) _DMG = 17*5;
                    else if (Player.Level >= 14) _DMG = 10*5;
                    else _DMG = 0;
                }
                return _DMG.Value;
            }
        }

        public DrainLife(Player p)
            : base(p, p.Form == Player.Forms.Metamorphosis ? RUNE_CD : CD, 0, true, true, School.Shadow, p.Form == Player.Forms.Metamorphosis ? RUNE_CAST_TIME : CAST_TIME)
        {
            Cost = p.Form == Player.Forms.Metamorphosis ? (int)(BASE_COST * RUNE_COST_RATIO) : BASE_COST;
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
            return (int)Math.Round((DMG + Player.SP * RATIO) / NB_TICKS
                * (1 + 0.02 * Player.GetTalentPoints("IDL"))
                * (1 + 0.02 * Player.GetTalentPoints("SM"))
                * Math.Max(Player.Tanking ? 0 : (1 + 0.15 * Player.GetTalentPoints("DS")), 1 + 0.02 * Player.GetTalentPoints("MD") * (1 + 0.03 * Player.GetTalentPoints("SL")))
                * (Player.Target.Effects.ContainsKey(ShadowVulnerability.NAME) ? ((ShadowVulnerability)Player.Target.Effects[ShadowVulnerability.NAME]).Modifier : 1)
                * (Player.Target.Effects.ContainsKey("Shadow Weaving") ? 1.15 : 1)
                * Player.DamageMod
                );
        }
    }
}

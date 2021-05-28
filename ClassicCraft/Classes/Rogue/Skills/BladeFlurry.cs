using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BladeFlurry : Skill
    {
        public override string ToString() { return NAME; } public static new string NAME = "Blade Flurry";

        public static int BASE_COST = 25;
        public static int CD = 120;

        public BladeFlurry(Player p)
            : base(p, CD, BASE_COST)
        {
        }

        public override void Cast()
        {
            CommonRessourceSkill();
            DoAction();
        }

        public static void CheckProc(Player Player, int damage, ResultType res)
        {
            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                if (Player.Effects.ContainsKey(NAME))
                {
                    Proc(Player, damage);
                }
            }
        }

        public static void Proc(Player Player, int damage)
        {
            // TODO : Check if BF crit is in TBC (quick testing didn't show any crit)
            ResultType res = Program.version == Version.Vanilla && Randomer.NextDouble() < Player.SpellCritChance ? ResultType.Crit : ResultType.Hit;

            damage = (int)Math.Round(damage
                   * Player.Sim.DamageMod(res)
                   * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless") || b.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                   );

            if (!Player.CustomActions.ContainsKey(NAME))
            {
                Player.CustomActions.Add(NAME, new CustomAction(Player, NAME, School.Physical));
            }

            Player.CustomActions[NAME].RegisterDamage(new ActionResult(res, damage));
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(BladeFlurryBuff.NAME))
            {
                Player.Effects[BladeFlurryBuff.NAME].Refresh();
            }
            else
            {
                new BladeFlurryBuff(Player).StartEffect();
            }

            LogAction();
        }
    }
}

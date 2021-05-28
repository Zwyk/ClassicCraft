using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class BladeFlurryBuff : Effect
    {
        public static int LENGTH = 15;

        public BladeFlurryBuff(Player p)
            : base(p, p, true, LENGTH, 1)
        {
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.HasteMod *= 1.2;
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.HasteMod /= 1.2;
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

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Blade Flurry";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SweepingStrikesBuff : Effect
    {
        public static int LENGTH = 10;

        public SweepingStrikesBuff(Player p)
            : base(p, p, true, LENGTH, 10, 10)
        {
        }

        public static void CheckProc(Player Player, int damage, ResultType res)
        {
            if(res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                if (Player.Effects.ContainsKey(NAME))
                {
                    Proc(Player, damage);
                    Player.Effects[NAME].StackRemove();
                }
            }
        }

        public static void Proc(Player Player, int damage)
        {
            ResultType res = Randomer.NextDouble() < Player.SpellCritChance ? ResultType.Crit : ResultType.Hit;

            damage = (int)Math.Round(damage
                   * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                   * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless")) ? 1.03 : 1)
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
        public static new string NAME = "Sweeping Strikes";
    }
}

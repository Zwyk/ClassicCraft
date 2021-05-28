using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Shiv : Skill
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shiv";

        public static int BASE_COST = 20;
        public static int CD = 0;

        public Shiv(Player p)
            : base(p, CD, BASE_COST)
        {
            Cost += (int)Math.Round(p.OH.Speed / 10);
        }

        public override void DoAction()
        {
            Weapon weapon = Player.MH;

            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + Simulation.Normalization(weapon) * Player.AP / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + Simulation.Normalization(weapon) * Player.AP / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1))
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * (1 + (0.02 * Player.GetTalentPoints("Agg")))
                * (res == ResultType.Crit ? 1 + (0.06 * Player.GetTalentPoints("Letha")) : 1)
                * (1 + (0.01 * Player.GetTalentPoints("Murder")))
                * Player.DamageMod
                * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless") || b.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                * (1 + (Player.Class == Player.Classes.Rogue && res == ResultType.Crit && Player.OH.Type == Weapon.WeaponType.Mace ? 0.01 * Player.GetTalentPoints("Mace") : 0))
                );

            CommonAction();

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Cost / 2;
            }
            else
            {
                Player.Resource -= Cost;
            }

            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                Player.Combo++;
            }
            
            if (res == ResultType.Crit && Randomer.NextDouble() < 0.2 * Player.GetTalentPoints("SF"))
            {
                Player.Combo++;
            }

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res, false, null, this);

            BladeFlurryBuff.CheckProc(Player, damage, res);
        }
    }
}

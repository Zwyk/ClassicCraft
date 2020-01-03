using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HeroicStrike : Spell
    {
        public static int BASE_COST = 15;
        public static int CD = 0;

        public HeroicStrike(Player p)
            : base(p, CD, BASE_COST, true)
        {
        }

        public override bool CanUse()
        {
            return Player.Resource >= Cost && Player.applyAtNextAA == null;
        }

        public override void Cast()
        {
            Player.applyAtNextAA = this;
        }

        public override void DoAction()
        {
            Player.applyAtNextAA = null;

            Weapon weapon = Player.MH;

            LockedUntil = Player.Sim.CurrentTime + weapon.Speed / Player.HasteMod;
            
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + weapon.Speed * (Player.AP + Player.nextAABonus) / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + weapon.Speed * (Player.AP + Player.nextAABonus) / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + 157)
                * Player.Sim.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1)
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Cost / 2;
            }
            else
            {
                Player.Resource -= Cost;
            }

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return "Heroic Strike";
        }
    }
}

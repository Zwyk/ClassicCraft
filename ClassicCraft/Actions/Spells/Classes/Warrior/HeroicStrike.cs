using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HeroicStrike : Spell
    {
        public static int COST = 15;
        public static int CD = 0;

        public HeroicStrike(Player p)
            : base(p, CD, COST, true)
        {
        }

        public override bool CanUse()
        {
            return Player.Resource >= ResourceCost && Player.applyAtNextAA == null;
        }

        public override void Cast()
        {
            Player.applyAtNextAA = this;
        }

        public override void DoAction()
        {
            Player.applyAtNextAA = null;

            Random random = new Random();

            Weapon weapon = Player.MH;

            LockedUntil = Player.Sim.CurrentTime + weapon.Speed / Player.HasteMod;
            
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + weapon.Speed * Player.AP / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + weapon.Speed * Player.AP / 14);

            int damage = (int)Math.Round((random.Next(minDmg, maxDmg + 1) + 157)
                * Player.Sim.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1)
                * (Player.DualWielding() ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= ResourceCost / 2;
            }
            else
            {
                Player.Resource -= ResourceCost;
            }

            RegisterDamage(new ActionResult(res, damage));

            if (Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Player, res, Player.GetTalentPoints("DW"));
            }
            if (Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Player, res, Player.GetTalentPoints("Flurry"));
            }
            if (Player.GetTalentPoints("UW") > 0)
            {
                UnbridledWrath.CheckProc(Player, res, Player.GetTalentPoints("UW"));
            }
            if (Player.MH.Enchantment != null && Player.MH.Enchantment.Name == "Crusader")
            {
                Crusader.CheckProc(Player, res, Player.MH.Speed);
            }
        }

        public override string ToString()
        {
            return "Heroic Strike";
        }
    }
}

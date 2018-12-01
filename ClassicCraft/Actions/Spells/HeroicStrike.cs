using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HeroicStrike : Spell
    {
        public HeroicStrike(Player p, double baseCD = 0, int ressourceCost = 15, bool gcd = true)
            : base(p, baseCD, ressourceCost, gcd)
        {
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

            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Player.Ressource -= RessourceCost;
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
        }

        public override string ToString()
        {
            return "Heroic Strike";
        }
    }
}

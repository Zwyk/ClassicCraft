using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Whirlwind : Spell
    {
        static Random random = new Random();

        public Whirlwind(Player p, double baseCD = 10, int ressourceCost = 25)
            : base(p, baseCD, ressourceCost)
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            CommonAction();
            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Player.Ressource -= RessourceCost;
            }

            int minDmg = (int)Math.Round(Player.MH.DamageMin + Program.Normalization(Player.MH) * Player.AP / 14);
            int maxDmg = (int)Math.Round(Player.MH.DamageMax + Program.Normalization(Player.MH) * Player.AP / 14);

            if (Player.OH != null)
            {
                minDmg += (int)Math.Round(Player.OH.DamageMin + Program.Normalization(Player.OH) * Player.AP / 14);
                maxDmg += (int)Math.Round(Player.OH.DamageMax + Program.Normalization(Player.OH) * Player.AP / 14);
            }

            int damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1 )
                * (Player.DualWielding() ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            RegisterDamage(new ActionResult(res, damage));

            if (Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Player, res, Player.GetTalentPoints("DW"));
            }
            if (Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Player, res, Player.GetTalentPoints("Flurry"));
            }
        }

        public override string ToString()
        {
            return "Whirlwind";
        }
    }
}

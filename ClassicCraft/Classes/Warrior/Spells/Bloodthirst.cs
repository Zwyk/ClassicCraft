using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodthirst : Spell
    {
        public static int COST = 30;
        public static int CD = 6;

        static Random random = new Random();

        public Bloodthirst(Player p)
            : base(p, CD, COST) {}

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);
            
            int damage = (int)Math.Round(0.45 * Player.AP
                * Player.Sim.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Player.GetTalentPoints("Impale")) : 1)
                * (Player.DualWielding() ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            CommonAction();
            if(res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Cost / 2;
            }
            else
            {
                Player.Resource -= Cost;
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
            if (Player.MH.Enchantment != null && Player.MH.Enchantment.Name == "Crusader")
            {
                Crusader.CheckProc(Player, res, Player.MH.Speed);
            }
        }

        public override string ToString()
        {
            return "Bloodthirst";
        }
    }
}

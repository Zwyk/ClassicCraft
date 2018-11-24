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

        public Whirlwind(double baseCD = 10, int ressourceCost = 25)
            : base(baseCD, ressourceCost)
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Program.Player.YellowAttackEnemy(Program.Boss);

            CommonAction();
            if (res != ResultType.Parry && res != ResultType.Dodge)
            {
                Program.Player.Ressource -= RessourceCost;
            }

            int minDmg = (int)Math.Round(Program.Player.MH.DamageMin + Program.Normalization(Program.Player.MH) * Program.Player.AP / 14);
            int maxDmg = (int)Math.Round(Program.Player.MH.DamageMax + Program.Normalization(Program.Player.MH) * Program.Player.AP / 14);

            if (Program.Player.OH != null)
            {
                minDmg += (int)Math.Round(Program.Player.OH.DamageMin + Program.Normalization(Program.Player.OH) * Program.Player.AP / 14);
                maxDmg += (int)Math.Round(Program.Player.OH.DamageMax + Program.Normalization(Program.Player.OH) * Program.Player.AP / 14);
            }

            int damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Program.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Program.Player.GetTalentPoints("Impale")) : 1 )
                * (Program.Player.DualWielding() ? 1 : (1 + 0.01 * Program.Player.GetTalentPoints("2HS"))));

            if (Program.Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(res, Program.Player.GetTalentPoints("DW"));
            }
            if (Program.Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(res, Program.Player.GetTalentPoints("Flurry"));
            }

            RegisterDamage(new ActionResult(res, damage));
        }

        public override string ToString()
        {
            return "WW";
        }
    }
}

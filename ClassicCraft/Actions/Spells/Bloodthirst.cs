using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodthirst : Spell
    {
        static Random random = new Random();

        public Bloodthirst(double baseCD = 6, int ressourceCost = 30)
            : base(baseCD, ressourceCost) {}

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Program.Player.YellowAttackEnemy(Program.Boss);

            int damage = (int)Math.Round(0.45 * Program.Player.AP
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Program.Boss.Armor)
                * (res == ResultType.Crit ? 1 + (0.1 * Program.Player.GetTalentPoints("Impale")) : 1)
                * (Program.Player.DualWielding() ? 1 : (1 + 0.01 * Program.Player.GetTalentPoints("2HS"))));

            CommonAction();
            if(res != ResultType.Parry && res != ResultType.Dodge)
            {
                Program.Player.Ressource -= RessourceCost;
            }

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
            return "BT";
        }
    }
}

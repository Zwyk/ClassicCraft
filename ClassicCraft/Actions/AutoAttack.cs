using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class AutoAttack : Action
    {
        static Random random = new Random();

        public bool MH { get; set; }
        public Weapon Weapon { get; set; }

        public AutoAttack(Player p, Weapon weapon, bool mh)
            : base(p, weapon.Speed)
        {
            Weapon = weapon;
            MH = mh;
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            NextAA();

            ResultType res = Player.WhiteAttackEnemy(Player.Sim.Boss, MH);

            int minDmg = (int)Math.Round(Weapon.DamageMin + Weapon.Speed * Player.AP / 14);
            int maxDmg = (int)Math.Round(Weapon.DamageMax + Weapon.Speed * Player.AP / 14);

            int damage = 0;

            damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * Program.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (Player.DualWielding() ? (MH ? 1 : 0.5 * (1 + (0.05 * Player.GetTalentPoints("DWS")))) : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            Player.Ressource += (int)Math.Round(Program.RageGained(damage, Weapon.Speed, res, MH));

            RegisterDamage(new ActionResult(res, damage));

            if (Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Player, res, Player.GetTalentPoints("DW"));
            }
            if(Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Player, res, Player.GetTalentPoints("Flurry"));
            }
            if(Player.GetTalentPoints("UW") > 0)
            {
                UnbridledWrath.CheckProc(Player, res, Player.GetTalentPoints("UW"));
            }
        }

        public void NextAA()
        {
            LockedUntil += Weapon.Speed / Player.HasteMod;
        }

        public override string ToString()
        {
            return "AA " + (MH ? "MH" : "OH");
        }
    }
}

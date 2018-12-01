using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class AutoAttack : Action
    {
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
            DoAA();
            NextAA();
        }

        public void DoAA(int bonusAP = 0, bool didWf = false)
        {
            ResultType res = Player.WhiteAttackEnemy(Player.Sim.Boss, MH);

            int minDmg = (int)Math.Round(Weapon.DamageMin + Weapon.Speed * (Player.AP + bonusAP) / 14);
            int maxDmg = (int)Math.Round(Weapon.DamageMax + Weapon.Speed * (Player.AP + bonusAP) / 14);

            int damage = 0;

            damage = (int)Math.Round(Player.Sim.random.Next(minDmg, maxDmg + 1)
                * Player.Sim.DamageMod(res)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (Player.DualWielding() ? (MH ? 1 : 0.5 * (1 + (0.05 * Player.GetTalentPoints("DWS")))) : (1 + 0.01 * Player.GetTalentPoints("2HS"))));

            Player.Ressource += (int)Math.Round(Simulation.RageGained(damage, Weapon.Speed, res, MH));

            RegisterDamage(new ActionResult(res, damage));

            if (Player.GetTalentPoints("DW") > 0)
            {
                DeepWounds.CheckProc(Player, res, Player.GetTalentPoints("DW"));
            }
            if (Player.GetTalentPoints("Flurry") > 0)
            {
                Flurry.CheckProc(Player, res, Player.GetTalentPoints("Flurry"), didWf);
            }
            if (Player.GetTalentPoints("UW") > 0)
            {
                UnbridledWrath.CheckProc(Player, res, Player.GetTalentPoints("UW"));
            }
            if((MH && Player.MH.Enchantment.Name == "Crusader") || (!MH && Player.OH.Enchantment.Name == "Crusader"))
            {
                Crusader.CheckProc(Player, res, Weapon.Speed);
            }

            if(MH && !didWf && Player.WindfuryTotem)
            {
                if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glancing)
                {
                    if (Player.Sim.random.NextDouble() < 0.2)
                    {
                        if(Program.logFight)
                        {
                            Program.Log(string.Format("{0:N2} : Windfury procs", Player.Sim.CurrentTime));
                        }
                        DoAA(315, true);
                    }
                }
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

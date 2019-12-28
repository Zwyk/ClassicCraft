﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class AutoAttack : Action
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

        public void DoAA(bool extra = false, bool wf = false)
        {
            ResultType res = Player.WhiteAttackEnemy(Player.Sim.Boss, MH);

            int minDmg = (int)Math.Round((Player.Form == Player.Forms.Cat ? Player.Level * 0.85 : (Weapon.DamageMin + Weapon.Speed)) + (Player.AP + Player.nextAABonus) / 14);
            int maxDmg = (int)Math.Round((Player.Form == Player.Forms.Cat ? Player.Level * 1.25 : (Weapon.DamageMax + Weapon.Speed)) + (Player.AP + Player.nextAABonus) / 14);

            Player.nextAABonus = 0;

            double baseDamage = 
                Randomer.Next(minDmg, maxDmg + 1)
                * Entity.ArmorMitigation(Player.Sim.Boss.Armor)
                * (Player.DualWielding() ? (MH ? 1 : 0.5 * (1 + (0.05 * Player.GetTalentPoints("DWS")))) : (1 + 0.01 * Player.GetTalentPoints("2HS")));

            int damage = (int)Math.Round(baseDamage * Player.Sim.DamageMod(res));

            if (Player.Class == Player.Classes.Warrior)
            {
                Player.Resource += (int)Math.Round(Simulation.RageGained(damage, Player.Level));
            }

            RegisterDamage(new ActionResult(res, damage));
            
            Player.CheckOnHits(MH, res, extra, wf);
        }

        public void NextAA()
        {
            LockedUntil += CurrentSpeed();
        }

        public double CurrentSpeed()
        {
            return (Player.Form == Player.Forms.Cat ? 1 : BaseCD) / Player.HasteMod;
        }

        public override string ToString()
        {
            return "AA " + (MH ? "MH" : "OH");
        }
    }
}

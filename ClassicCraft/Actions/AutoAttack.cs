using System;
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
            CastNextSwing();
        }

        public void DoAA(List<string> alreadyProc = null, bool extra = false)
        {
            ResultType res;
            if(Player.Class == Player.Classes.Warrior && !MH && Player.applyAtNextAA != null)
            {
                res = Player.YellowAttackEnemy(Player.Sim.Boss);
            }
            else
            {
                res = Player.WhiteAttackEnemy(Player.Sim.Boss, MH);
            }

            int minDmg = (int)Math.Round(
                Player.Form == Player.Forms.Cat ? Player.Level * 0.85 + (Player.AP + Player.nextAABonus) / 14 :
                Weapon.DamageMin + Weapon.Speed * (Player.AP + Player.nextAABonus) / 14);
            int maxDmg = (int)Math.Round(
                Player.Form == Player.Forms.Cat ? Player.Level * 1.25 + (Player.AP + Player.nextAABonus) / 14 : 
                Weapon.DamageMax + Weapon.Speed * (Player.AP + Player.nextAABonus) / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor)
                * Player.DamageMod
                * (Player.DualWielding ? (MH ? 1 : 0.5 * (1 + (0.05 * Player.GetTalentPoints("DWS")))) : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                );

            if (Player.Class == Player.Classes.Warrior || Player.Form == Player.Forms.Bear)
            {
                Player.Resource += (int)Math.Round(Simulation.RageGained(damage, Player.Level));
                //Player.Resource += (int)Math.Round(Simulation.RageGained2(damage, Weapon.Speed, MH, res == ResultType.Crit, Player.Level));
            }

            RegisterDamage(new ActionResult(res, damage));
            
            Player.CheckOnHits(MH, true, res, extra, alreadyProc);
        }

        public void ResetSwing()
        {
            LockedUntil = Player.Sim.CurrentTime;
        }

        public void CastNextSwing()
        {
            Program.Log(LockedUntil + " + " + CurrentSpeed());
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

        public override bool CanUse()
        {
            return Available();
        }
    }
}

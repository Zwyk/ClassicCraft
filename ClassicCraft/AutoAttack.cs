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

        public AutoAttack(Weapon weapon, bool mh)
            : base(weapon.Speed)
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

            bool crit = random.NextDouble() < Player.Instance.CritChance;

            int minDmg = (int)Math.Round(Weapon.DamageMin + Weapon.Speed * Player.Instance.AP / 14);
            int maxDmg = (int)Math.Round(Weapon.DamageMax + Weapon.Speed * Player.Instance.AP / 14);

            int damage = (int)Math.Round(random.Next(minDmg, maxDmg + 1)
                * (1 + (crit ? 1 : 0))
                * (Player.Instance.OH != null ? 1.03 : (MH ? 1 : 0.625)));

            Player.Instance.Ressource += (int)Math.Round(Program.RageGained(damage, Weapon.Speed, crit, MH));

            FlurryCheck(crit);

            RegisterDamage(new ActionResult(crit ? ResultType.Crit : ResultType.Hit, damage));
        }

        public void NextAA()
        {
            LockedUntil = Program.currentTime + Weapon.Speed / Program.speedMod;
        }

        public void FlurryCheck(bool crit)
        {
            if (crit)
            {
                Flurry flurry = new Flurry(Program.currentTime, Program.currentTime + 12);
                if (Player.Instance.buffs.ContainsKey(Flurry.Name))
                {
                    Player.Instance.buffs[Flurry.Name] = flurry;
                }
                else
                {
                    Player.Instance.buffs.Add(Flurry.Name, flurry);
                }
            }
            else if (Player.Instance.buffs.ContainsKey(Flurry.Name) && Player.Instance.buffs[Flurry.Name].End >= Program.currentTime)
            {
                Player.Instance.buffs[Flurry.Name].Stacks--;

                if (Player.Instance.buffs[Flurry.Name].Stacks == 0)
                {
                    Player.Instance.buffs[Flurry.Name].End = Program.currentTime;
                }
            }

            if (Player.Instance.buffs.ContainsKey(Flurry.Name) && Player.Instance.buffs[Flurry.Name].End >= Program.currentTime)
            {
                Program.speedMod = 1.3;
            }
            else
            {
                Program.speedMod = 1;
            }
        }

        public override string ToString()
        {
            return "AA " + (MH ? "MH" : "OH");
        }
    }
}

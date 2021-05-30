using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Cleave : Skill
    {
        public static int BASE_COST = 20;
        public static int CD = 0;
        
        public static int BONUS_DMG = Program.version == Version.TBC ? 70 : 50;

        public Cleave(Player p)
            : base(p, CD, BASE_COST, true)
        {
        }

        public override bool CanUse()
        {
            return Player.Resource >= Cost;
        }

        public override void Cast()
        {
            Player.applyAtNextAA = this;
        }

        public override void DoAction()
        {
            Player.applyAtNextAA = null;

            Weapon weapon = Player.MH;

            LockedUntil = Player.Sim.CurrentTime + weapon.Speed / Player.HasteMod;

            List<int> damages = new List<int>(); ;
            List<ResultType> ress = new List<ResultType>();

            for (int i = 1; i <= Math.Min(2, Player.Sim.NbTargets); i++)
            {
                ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

                int minDmg = (int)Math.Round(weapon.DamageMin + weapon.Speed * (Player.AP + Player.nextAABonus) / 14);
                int maxDmg = (int)Math.Round(weapon.DamageMax + weapon.Speed * (Player.AP + Player.nextAABonus) / 14);

                Player.nextAABonus = 0;

                int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + BONUS_DMG)
                    * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                    * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                    * Player.DamageMod
                    * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                    * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                    * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless") || b.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                    * (Player.Sim.Boss.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                    );

                if(i == 1)
                {
                    if (res == ResultType.Parry || res == ResultType.Dodge)
                    {
                        Player.Resource -= (int)(Cost * 0.2);
                    }
                    else
                    {
                        Player.Resource -= Cost;
                    }
                }

                RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));

                Player.CheckOnHits(true, true, res);

                damages.Add(damage);
                ress.Add(res);
            }

            for (int i = 0; i < damages.Count; i++)
            {
                SweepingStrikesBuff.CheckProc(Player, damages[i], ress[i]);
            }
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Cleave";
    }
}

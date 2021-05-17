using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class HeroicStrike : Skill
    {
        public static int BASE_COST = 15;
        public static int CD = 0;

        public static int BONUS_THREAT = Program.version == Version.TBC ? 196 : 175;

        public HeroicStrike(Player p)
            : base(p, CD, BASE_COST - p.GetTalentPoints("IHS") - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0), true)
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
            
            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + weapon.Speed * (Player.AP + Player.nextAABonus) / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + weapon.Speed * (Player.AP + Player.nextAABonus) / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + (Program.version == Version.TBC ? 208 : 157))
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless")) ? 1.03 : 1)
                );

            int threat = (int)Math.Round((damage + BONUS_THREAT) * Player.ThreatMod);

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Cost / 2;
            }
            else
            {
                Player.Resource -= Cost;
            }

            RegisterDamage(new ActionResult(res, damage, threat));

            Player.CheckOnHits(true, true, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Heroic Strike";
    }
}

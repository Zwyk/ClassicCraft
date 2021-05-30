using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class SinisterStrike : Skill
    {
        public static int BASE_COST = 45;
        public static int CD = 0;

        public static int BASE_DMG = Program.version == Version.Vanilla ? 68 : 98;

        public SinisterStrike(Player p)
            : base(p, CD, BASE_COST - (p.GetTalentPoints("ISS") > 0 ? (p.GetTalentPoints("ISS") > 1 ? 5 : 3) : 0)) { }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            Weapon weapon = Player.MH;

            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = (int)Math.Round(weapon.DamageMin + Simulation.Normalization(weapon) * Player.AP / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax + Simulation.Normalization(weapon) * Player.AP / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + BASE_DMG)
                * Player.DamageMod
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * (1 + (0.02 * Player.GetTalentPoints("Agg")))
                * (res == ResultType.Crit ? 1 + (0.06 * Player.GetTalentPoints("Letha")) : 1)
                * (1 + (0.01 * Player.GetTalentPoints("Murder")))
                * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless") || b.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                * (1 + (Player.NbSet("Slayer's")>=4 ? 0.06 : 0))
                * (1 + (Player.Class == Player.Classes.Rogue && res == ResultType.Crit && Player.MH.Type == Weapon.WeaponType.Mace ? 0.01 * Player.GetTalentPoints("Mace") : 0))
                );

            CommonAction();

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Cost / 2;
            }
            else
            {
                Player.Resource -= Cost;
            }

            if (res == ResultType.Hit || res == ResultType.Crit || res == ResultType.Block || res == ResultType.Glance)
            {
                Player.Combo++;
            }
            
            if (res == ResultType.Crit && Randomer.NextDouble() < 0.2 * Player.GetTalentPoints("SF"))
            {
                Player.Combo++;
            }

            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));

            Player.CheckOnHits(true, false, res);

            BladeFlurryBuff.CheckProc(Player, damage, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Sinister Strike";
    }
}

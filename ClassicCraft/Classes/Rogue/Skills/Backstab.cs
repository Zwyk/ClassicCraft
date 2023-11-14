using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Backstab : Skill
    {
        public static int BASE_COST = 60;
        public static int CD = 0;

        public static int BASE_DMG = Program.version == Version.Vanilla ? 210 : 255;

        public Backstab(Player p)
            : base(p, CD, BASE_COST) { }

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            Weapon weapon = Player.MH;

            ResultType res = Player.YellowAttackEnemy(Target, ToString());

            int minDmg = (int)Math.Round(weapon.DamageMin * 1.5 + Simulation.Normalization(weapon) * Player.AP / 14);
            int maxDmg = (int)Math.Round(weapon.DamageMax * 1.5 + Simulation.Normalization(weapon) * Player.AP / 14);

            Player.nextAABonus = 0;

            int damage = (int)Math.Round((Randomer.Next(minDmg, maxDmg + 1) + BASE_DMG)
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * (1 + (0.04 * Player.GetTalentPoints("Oppo")))
                * (res == ResultType.Crit ? 1 + (0.06 * Player.GetTalentPoints("Letha")) : 1)
                * (1 + (0.01 * Player.GetTalentPoints("Murder")))
                * Player.DamageMod
                * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
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
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Backstab";
    }
}

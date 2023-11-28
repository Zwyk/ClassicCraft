using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Envenom : Skill
    {
        public static int BASE_COST = 35;
        public static int CD = 0;

        public static int DMG = 180;
        public static double AP_RATIO_PER_POINTS = 0.03;

        public Envenom(Player p)
            : base(p, CD, BASE_COST - (p.NbSet("Assassination") >= 4 ? 10 : 0)) { }

        public override void DoAction()
        {
            CommonAction();

            ResultType res = Player.YellowAttackEnemy(Target);

            int damage = (int)Math.Round(
                (DMG * Player.Combo + Player.AP * AP_RATIO_PER_POINTS * Player.Combo)
                * Player.Sim.DamageMod(res)
                * Simulation.MagicMitigation(Target.ResistChances[School.Nature])
                * Player.DamageMod
                * (1 + (0.02 * Player.GetTalentPoints("Agg")))
                * (1 + (0.05 * Player.GetTalentPoints("IE")))
                * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                * (1 + (Player.Class == Player.Classes.Rogue && res == ResultType.Crit && Player.MH.Type == Weapon.WeaponType.Mace ? 0.01 * Player.GetTalentPoints("Mace") : 0))
                );

            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                // TODO à vérifier
                Player.Resource -= Player.Effects.ContainsKey("CdG") ? 0 : Cost / 2;
                if (Player.Effects.ContainsKey("CdG")) Player.Effects["CdG"].EndEffect();
            }
            else
            {
                Player.Resource -= Player.Effects.ContainsKey("CdG") ? 0 : Cost;
                if (Player.Effects.ContainsKey("CdG")) Player.Effects["CdG"].EndEffect();

                if (Target.Effects.ContainsKey("Deadly Poison"))
                {
                    Target.Effects["Deadly Poison"].StackRemove(Player.Combo);
                }

                if (Player.GetTalentPoints("RS") > 0 && Randomer.NextDouble() < 0.2 * Player.Combo)
                {
                    Player.Resource += 25;
                }

                Player.Combo = 0;

                if (Randomer.NextDouble() < 0.2 * Player.GetTalentPoints("Ruth"))
                {
                    Player.Combo++;
                }
                if (Player.NbSet("Netherblade") >= 4 && Randomer.NextDouble() < 0.15)
                {
                    Player.Combo++;
                }
            }

            RegisterDamage(new ActionResult(res, damage, (int)(damage * Player.ThreatMod)));

            Player.CheckOnHits(true, false, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Eviscerate";
    }
}

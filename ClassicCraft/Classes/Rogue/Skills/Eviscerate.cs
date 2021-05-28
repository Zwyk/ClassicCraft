using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Eviscerate : Skill
    {
        public static int BASE_COST = 35;
        public static int CD = 0;

        // rank 8
        public static int[] min =
        {
            199,
            350,
            501,
            652,
            803,
        };
        // rank 8
        public static int[] max =
        {
            295,
            446,
            597,
            748,
            899,
        };

        public static int MIN_TBC = 245;
        public static int MAX_TBC = 365;
        public static double AP_RATIO_PER_POINTS = 0.03;

        public Eviscerate(Player p)
            : base(p, CD, BASE_COST) { }

        public override void Cast()
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            DoAction();
        }

        public override void DoAction()
        {
            CommonAction();

            ResultType res = Player.YellowAttackEnemy(Player.Sim.Boss);

            int minDmg = Program.version == Version.Vanilla ? min[Player.Combo - 1] : MIN_TBC;
            int maxDmg = Program.version == Version.Vanilla ? max[Player.Combo - 1] : MAX_TBC;

            int damage = (int)Math.Round(
                (Randomer.Next(minDmg, maxDmg + 1) + Player.AP * (Program.version == Version.Vanilla ? 0.15 : AP_RATIO_PER_POINTS * Player.Combo)
                    + (Player.NbSet("Deathmantle") >= 2 ? 40 : 0))
                * Player.Sim.DamageMod(res)
                * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (1 + (0.02 * Player.GetTalentPoints("Agg")))
                * (1 + (0.05 * Player.GetTalentPoints("IE")))
                * (1 + (0.01 * Player.GetTalentPoints("Murder")))
                * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless") || b.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
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

            RegisterDamage(new ActionResult(res, damage));

            Player.CheckOnHits(true, false, res);

            BladeFlurryBuff.CheckProc(Player, damage, res);
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Eviscerate";
    }
}

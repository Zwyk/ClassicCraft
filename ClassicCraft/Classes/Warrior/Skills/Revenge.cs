using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Revenge : Skill
    {
        public static int BASE_COST = 5;
        public static int CD = 5;

        public static int DMG_MIN = Program.version == Version.TBC ? 414 : 81;
        public static int DMG_MAX = Program.version == Version.TBC ? 506 : 99;

        public static int BONUS_THREAT = Program.version == Version.TBC ? 200 : 355;

        public Revenge(Player p)
            : base(p, CD, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") : 0)) { }

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Target);

            int damage = (int)Math.Round((Randomer.Next(DMG_MIN, DMG_MAX + 1) + (Player.NbSet("Dreadnaught") >= 2 ? 75 : 0))
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                * (Target.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                * (Player.Effects.ContainsKey("T4 4P") ? 1.1 : 1)
                );

            int threat = (int)Math.Round((damage + BONUS_THREAT) * Player.ThreatMod);

            CommonAction();
            if (res == ResultType.Parry || res == ResultType.Dodge)
            {
                Player.Resource -= (int)(Cost * 0.2);
            }
            else
            {
                Player.Resource -= Cost;
            }

            RegisterDamage(new ActionResult(res, damage, threat));

            Player.CheckOnHits(true, false, res);

            SweepingStrikesBuff.CheckProc(Player, damage, res);

            if (Player.NbSet("Warbringer") >= 4)
            {
                string procName = "T4 4P";
                if (Player.Effects.ContainsKey(procName))
                {
                    Player.Effects[procName].Refresh();
                }
                else
                {
                    CustomEffect buff = new CustomEffect(Player, Player, procName, true, 15);   // Unsure about duration
                    buff.StartEffect();
                }
            }
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Revenge";
    }
}

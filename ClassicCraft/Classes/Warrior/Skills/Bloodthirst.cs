using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Bloodthirst : Skill
    {
        public static int BASE_COST = 30;
        public static int CD = 6;

        public Bloodthirst(Player p)
            : base(p, CD, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") + (p.NbSet("Destroyer") >= 4 ? 5 : 0) : 0)) {}

        public override void Cast(Entity t)
        {
            DoAction();
        }

        public override void DoAction()
        {
            ResultType res = Player.YellowAttackEnemy(Target);

            int damage = (int)Math.Round(0.45 * Player.AP
                * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                * Player.DamageMod
                * (Player.DualWielding ? 1 : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                * (Player.NbSet("Onslaught") >= 4 ? 1.05 : 1)
                * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                * (Target.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                * (Player.Effects.ContainsKey("T4 4P") ? 1.1 : 1)
                );

            int threat = (int)Math.Round(damage * (1 + 0.21 * Player.GetTalentPoints("TM")) * Player.ThreatMod);

            CommonAction();
            if(res == ResultType.Parry || res == ResultType.Dodge)
            {
                Player.Resource -= (int)(Cost * 0.2);
            }
            else
            {
                Player.Resource -= Cost;
            }

            RegisterDamage(new ActionResult(res, damage, threat));

            if (Player.Effects.ContainsKey("T4 4P")) Player.Effects["T4 4P"].EndEffect();

            Player.CheckOnHits(true, false, res);

            SweepingStrikesBuff.CheckProc(Player, damage, res);

            if ((Player.Equipment[Player.Slot.Trinket1]?.Name.ToLower() == "ashtongue talisman of valor" || Player.Equipment[Player.Slot.Trinket2]?.Name.ToLower() == "ashtongue talisman of valor")
                && Randomer.NextDouble() < 0.25)
            {
                string procName = "Ashtongue Talisman of Valor";
                Dictionary<Attribute, double> attributes = new Dictionary<Attribute, double>()
                        {
                            { Attribute.Strength, 55 }
                        };
                int procDuration = 12;

                if (Player.Effects.ContainsKey(procName))
                {
                    Player.Effects[procName].Refresh();
                }
                else
                {
                    CustomStatsBuff buff = new CustomStatsBuff(Player, procName, procDuration, 1, attributes);
                    buff.StartEffect();
                }
            }
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Bloodthirst";
    }
}

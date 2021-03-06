﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Thunderclap : Skill
    {
        public static int BASE_COST = 20;
        public static int CD = 4;

        public static int DAMAGE = 123;

        public static double THREAT_MOD = 1.75;
        public static int MAX_TARGETS = 4;

        public Thunderclap(Player p)
            : base(p, CD, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("ITC") > 2 ? 4 : p.GetTalentPoints("ITC") : 0))
        {
        }

        public override void Cast()
        {
            DoAction();
        }

        public override void DoAction()
        {
            CommonAction();
            Player.Resource -= Cost;

            for (int i = 1; i <= Math.Min(MAX_TARGETS, Player.Sim.NbTargets); i++)
            {
                ResultType res = Randomer.NextDouble() < Player.SpellCritChance ? ResultType.Crit : ResultType.Hit;

                int damage = (int)Math.Round(DAMAGE
                    * (Player.Sim.DamageMod(res) + (res == ResultType.Crit ? 0.1 * Player.GetTalentPoints("Impale") : 0))
                    * Simulation.ArmorMitigation(Player.Sim.Boss.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                    * Player.DamageMod
                    * (res == ResultType.Crit && Player.Buffs.Any(b => b.Name.ToLower().Contains("relentless") || b.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                    * (Player.Sim.Boss.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                    * (1 + (Player.GetTalentPoints("ITC") > 0 ? 0.1 + 0.3 * Player.GetTalentPoints("ITC") : 0))
                    * (Player.Effects.ContainsKey("T4 4P") ? 1.1 : 1)
                    );

                RegisterDamage(new ActionResult(res, damage, (int)(damage * THREAT_MOD * Player.ThreatMod)));
            }

            if (Player.Effects.ContainsKey("T4 4P")) Player.Effects["T4 4P"].EndEffect();
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Thunder Clap";
    }
}

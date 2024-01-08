using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class MightyRageBuff : Effect
    {
        public override string ToString() { return NAME; } public static new string NAME = "Mighty Rage Buff";

        public static int LENGTH = 20;

        private double Bonus { get; set; }

        public MightyRageBuff(Player p, double baseLength = 20)
            : base(p, p, true, baseLength, 1)
        {
            Bonus = 60 * (p.Buffs.Any(b => b.Name.ToLower().Contains("blessing of kings")) ? 1.1 : 1) * (1 + 0.02 * p.GetTalentPoints("Vitality"));
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.Resource += Randomer.Next(45, 75 + 1);
            Player.Attributes.AddToValue(Attribute.Strength, Bonus);
            Player.Attributes.AddToValue(Attribute.AP, Bonus * 2 * (Program.version == Version.TBC ? 1 + 0.02 * Player.GetTalentPoints("IBStance") : 1));
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.Attributes.AddToValue(Attribute.Strength, -Bonus);
            Player.Attributes.AddToValue(Attribute.AP, -Bonus * 2 * (Program.version == Version.TBC ? 1 + 0.02 * Player.GetTalentPoints("IBStance") : 1));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class CustomStatsBuff : Effect
    {
        public string Name { get; set; }
        public Dictionary<Attribute, double> Buffs { get; set; }

        public CustomStatsBuff(Player p, string name, double baseLength, int baseStacks = 1, Dictionary<Attribute, double> buffs = null)
            : base(p, p, true, baseLength, baseStacks)
        {
            Name = name;
            Buffs = buffs;
        }

        public override string ToString()
        {
            return Name;
        }

        public override void StartEffect()
        {
            base.StartEffect();

            foreach (Attribute a in Buffs?.Keys)
            {
                if (a == Attribute.Haste)
                {
                    Program.Log("Haste : " + Player.HasteMod + " -> " + (Player.HasteMod * 1 + Buffs[a]));
                    Player.HasteMod *= 1 + Buffs[a];
                }
                else ApplyAttribute(a, Buffs[a]);
            }
        }

        public override void EndEffect()
        {
            base.EndEffect();

            foreach (Attribute a in Buffs?.Keys)
            {
                if(a == Attribute.Haste)
                {
                    Program.Log("Haste : " + Player.HasteMod + " -> " + (Player.HasteMod * 1 + Buffs[a]));
                    Player.HasteMod /= 1 + Buffs[a];
                }
                else ApplyAttribute(a, -Buffs[a]);
            }
        }

        private void ApplyAttribute(Attribute a, double bonus)
        {
            double mult = 1;

            if ((a == Attribute.Agility || a == Attribute.Strength || a == Attribute.Intellect || a == Attribute.Spirit || a == Attribute.Stamina)
                && Player.Buffs.Any(b => b.Name.ToLower().Contains("blessing of kings")))
            {
                mult *= 1.1;
            }
            switch (Player.Class)
            {
                case Player.Classes.Warrior:
                    if ((a == Attribute.Strength || a == Attribute.Stamina)
                        && Player.GetTalentPoints("Vitality") > 0)
                    {
                        mult *= 1 + (a == Attribute.Stamina ? 0.01 : 0.02) * Player.GetTalentPoints("Vitality");
                    }
                    else if (a == Attribute.AP && Player.GetTalentPoints("IBStance") > 0)
                    {
                        mult *= 1 + 0.02 * Player.GetTalentPoints("IBStance");
                    }
                    break;
            }

            bonus *= mult;

            Player.BonusAttributes.SetValue(a, Player.BonusAttributes.GetValue(Attribute.Strength) + bonus);

            if (a == Attribute.Strength) Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + bonus * Player.StrToAPRatio(Player.Class));
            else if (a == Attribute.Agility)
            {
                Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.Agility) + bonus * Player.AgiToAPRatio(Player) * (1 + 0.02 * Player.GetTalentPoints("IBStance")));
                Player.BonusAttributes.SetValue(Attribute.RangedAP, Player.BonusAttributes.GetValue(Attribute.RangedAP) + bonus * Player.AgiToRangedAPRatio(Player.Class));
                Player.BonusAttributes.SetValue(Attribute.CritChance, Player.BonusAttributes.GetValue(Attribute.CritChance) + bonus * Player.AgiToCritRatio(Player.Class));
            }
            else if (a == Attribute.Intellect) Player.BonusAttributes.SetValue(Attribute.SpellCritChance, Player.BonusAttributes.GetValue(Attribute.SpellCritChance) + bonus * Player.IntToCritRatio(Player.Class));
        }
    }
}
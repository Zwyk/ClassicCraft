﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowBolt : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Shadow Bolt";

        public static int CD = 0;

        public static int BASE_COST(int level)
        {
            if (level >= 60) return 380;
            //else if (level >= 60) return 370; // Rank 9
            else if (level >= 52) return 315;
            else if (level >= 44) return 265;
            else if (level >= 36) return 210;
            else if (level >= 28) return 160;
            else if (level >= 20) return 110;
            else if (level >= 12) return 70;
            else if (level >= 6) return 40;
            else return 25;
        }

        public static double CAST_TIME(int level)
        {
            if (level >= 20) return 3;
            else if (level >= 12) return 2.8;
            else if (level >= 6) return 2.2;
            else return 1.7;
        }

        public static int MIN_DMG(int level)
        {
            if (level >= 60) return 482;
            //else if (level >= 60) return 455; // Rank 9
            else if (level >= 52) return 373;
            else if (level >= 44) return 292;
            else if (level >= 36) return 213;
            else if (level >= 28) return 150;
            else if (level >= 20) return 92;
            else if (level >= 12) return 52;
            else if (level >= 6) return 26;
            else return 13;
        }

        public static int MAX_DMG(int level)
        {
            if (level >= 60) return 538;
            //else if (level >= 60) return 507; // Rank 9
            else if (level >= 52) return 415;
            else if (level >= 44) return 327;
            else if (level >= 36) return 240;
            else if (level >= 28) return 170;
            else if (level >= 20) return 104;
            else if (level >= 12) return 61;
            else if (level >= 6) return 32;
            else return 18;
        }

        public static int VOLLEY_MAX_TARGETS = 5;
        public static double VOLLEY_DAMAGE = 0.8;

        public ShadowBolt(Player p)
            : base(p, CD, School.Shadow, 
                  new SpellData(SpellType.Magical, (int)(BASE_COST(p.Level) * 1 - (0.01 * p.GetTalentPoints("Cata"))), true, CAST_TIME(p.Level) - 0.1 * p.GetTalentPoints("ISB"), SMI.Reset, 1, p.Runes.Contains("Shadow Bolt Volley") ? VOLLEY_MAX_TARGETS : 1),
                  new EndDmg((p.Runes.Contains("Shadow Bolt Volley") ? VOLLEY_DAMAGE : 1) * MIN_DMG(p.Level), (p.Runes.Contains("Shadow Bolt Volley") ? VOLLEY_DAMAGE : 1) * MAX_DMG(p.Level), Math.Max(1.5, CAST_TIME(p.Level)) / 3.5, RatioType.SP))
        {
        }

        public override void Cast(Entity t)
        {
            bool st = Player.Effects.ContainsKey(ShadowTrance.NAME);
            if (st) Player.Effects[ShadowTrance.NAME].EndEffect();
            Cast(t, st, false);
        }
    }
}

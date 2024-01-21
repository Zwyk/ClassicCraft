using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Thunderclap : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Thunder Clap";

        public static int BASE_COST = 20;
        public static int CD = 4;

        public static int DAMAGE = 123;

        public static double THREAT_MOD = 1.75;
        public static int MAX_TARGETS = 4;

        public Thunderclap(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Magical, BASE_COST - (Program.version == Version.TBC ? p.GetTalentPoints("FR") + (p.GetTalentPoints("ITC") > 2 ? 4 : p.GetTalentPoints("ITC")) : 0), true, 0, SMI.None, MAX_TARGETS, THREAT_MOD),
                  new EndDmg(DAMAGE, DAMAGE, 1.5/3.5/MAX_TARGETS, RatioType.AP))
        {
        }
    }
}

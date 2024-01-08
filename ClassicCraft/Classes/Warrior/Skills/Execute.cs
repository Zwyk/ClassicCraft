using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Execute : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Execute";

        public static int BASE_COST = 15;
        public static int CD = 0;

        public static int BASE_DMG = Program.version == Version.TBC ? 925 : 600;
        public static int DMG_BY_RAGE = Program.version == Version.TBC ? 21 : 15;

        public Execute(Player p)
            : base(p, CD, School.Physical, 
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("FR") - (p.NbSet("Onslaught")>=2?3:0) - (int)(p.GetTalentPoints("IE")*2.5)),
                  new EndDmg(BASE_DMG, BASE_DMG, 0, RatioType.None))
        {
        }

        public override bool CanUse()
        {
            return Player.Target.LifePct <= 0.2 && base.CanUse();
        }

        public override double GetEndDmgBase(bool mh = true)
        {
            return BASE_DMG + (Player.Resource - Cost) * DMG_BY_RAGE;
        }

        public override void OnNonManaResourceUse(ResultType res)
        {
            Player.Resource = Cost;
            base.OnNonManaResourceUse(res);
        }
    }
}

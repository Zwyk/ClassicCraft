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

        public static int BASE_DMG(int level)
        {
            if (level >= 70) return 925;
            else if (level >= 56) return 600;
            else if (level >= 48) return 450;
            else if (level >= 40) return 325;
            else if (level >= 32) return 200;
            else if (level >= 24) return 125;
            else return 0;
        }

        public static int DMG_BY_RAGE(int level)
        {
            if (level >= 70) return 21;
            else if (level >= 56) return 15;
            else if (level >= 48) return 12;
            else if (level >= 40) return 9;
            else if (level >= 32) return 6;
            else if (level >= 24) return 3;
            else return 0;
        }

        public Execute(Player p)
            : base(p, CD, School.Physical, 
                  new SpellData(SpellType.Melee, BASE_COST - p.GetTalentPoints("FR") - (p.NbSet("Onslaught")>=2?3:0) - (int)(p.GetTalentPoints("IE")*2.5)),
                  new EndDmg(BASE_DMG(p.Level), BASE_DMG(p.Level), 0, RatioType.None))
        {
        }

        public override bool CanUse()
        {
            return Player.Target.LifePct <= 0.2 && base.CanUse();
        }

        public override double GetEndDmgBase(bool mh = true)
        {
            return BASE_DMG(Player.Level) + (Player.Resource - Cost) * DMG_BY_RAGE(Player.Level);
        }

        public override void OnNonManaResourceUse(ResultType res)
        {
            Player.Resource = Cost;
            base.OnNonManaResourceUse(res);
        }
    }
}

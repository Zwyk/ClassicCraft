using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Envenom : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Eviscerate";

        public static int BASE_COST = 35;
        public static int CD = 0;

        public static int DMG_PER_POINT = 180;
        public static double AP_RATIO_PER_POINTS = 0.03;

        public Envenom(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (p.NbSet("Assassination") >= 4 ? 10 : 0), true, 0, SMI.None, 1, 1, 0, EnergyType.ComboSpend),
                  new EndDmg(1, 1, 0, RatioType.AP)) 
        {
        }

        public override double GetEndDmgBase(bool mh = true)
        {
            return DMG_PER_POINT * Player.Combo + Player.AP * AP_RATIO_PER_POINTS * Player.Combo;
        }
    }
}

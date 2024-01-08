using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Eviscerate : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Eviscerate";

        public static int BASE_COST = 35;
        public static int CD = 0;

        // rank 8
        public static int[] min =
        {
            199,
            350,
            501,
            652,
            803,
        };
        // rank 8
        public static int[] max =
        {
            295,
            446,
            597,
            748,
            899,
        };

        public static double VANILLA_AP_RATIO = 0.15;


        // rank 8
        public static int[] minTBC =
        {
            60 + 185,
            60 + 370,
            60 + 555,
            60 + 740,
            60 + 925,
        };
        // rank 8
        public static int[] maxTBC =
        {
            180 + 185,
            180 + 370,
            180 + 555,
            180 + 740,
            180 + 925,
        };
        public static double AP_RATIO_PER_POINTS = 0.03;

        public Eviscerate(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST - (p.NbSet("Assassination") >= 4 ? 10 : 0), true, 0, SMI.None, 1, 1, 0, EnergyType.ComboSpend),
                  new EndDmg(1, 1, 0, RatioType.None))
        {
        }

        public override double GetEndDmgBase(bool mh = true)
        {
            double flatMin = (Program.version == Version.Vanilla ? min[Player.Combo - 1] : minTBC[Player.Combo - 1]) + (Player.NbSet("Deathmantle") >= 2 ? 40 : 0);
            double flatMax = (Program.version == Version.Vanilla ? max[Player.Combo - 1] : maxTBC[Player.Combo - 1]) + (Player.NbSet("Deathmantle") >= 2 ? 40 : 0);

            return Randomer.Next(flatMin, flatMax + 1)
                + Player.AP * (Program.version == Version.Vanilla ? VANILLA_AP_RATIO : AP_RATIO_PER_POINTS * Player.Combo);
        }
    }
}

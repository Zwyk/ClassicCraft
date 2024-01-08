using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassicCraft
{
    class SavageRoar : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Savage Roar";

        public static double CD = 0;
        public static int BASE_COST = 25;

        public SavageRoar(Player p)
            : base(p, CD, School.Physical,
                  new SpellData(SpellType.Melee, BASE_COST, true, 0, SMI.None, 1, 1, 0, EnergyType.ComboSpend),
                  null,
                  new EndEffect(SavageRoarBuff.NAME))
        {
        }
    }
}

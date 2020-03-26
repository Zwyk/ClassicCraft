using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
	public class Consecration_Base : Spell
	{
        public override string ToString() { return NAME; }
        public static new string NAME = "Consecration (Base)";

        public static int CD = 8;

        public Consecration_Base(Player p, int cost)
               : base(p, CD, cost, true, true)
        {
        }

        public override void Cast()
        {
            CommonManaSpell();
            DoAction();
        }
    }
}

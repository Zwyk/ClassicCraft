using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class DemonicGrace : Spell
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Demonic Grace";

        public static int CD = 20;

        public DemonicGrace(Player p)
               : base(p, CD, School.Shadow,
                     new SpellData(SpellType.Magical, 0),
                     null,
                     new EndEffect(DemonicGraceBuff.NAME))
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Item
    {
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }

        public Item(int str = 0, int agi = 0, int intel = 0)
        {
            Strength = str;
            Agility = agi;
            Intelligence = intel;
        }
    }
}

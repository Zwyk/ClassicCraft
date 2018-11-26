using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Aura : PlayerObject
    {
        //public Entity Source { get; set; }
        public Entity Target { get; set; }

        public Aura(Player p, Entity target)
            : base(p)
        {
            Target = target;
        }
    }
}

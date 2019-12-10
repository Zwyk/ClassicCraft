using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Spell : Action
    {
        public int ResourceCost { get; set; }
        public bool AffectedByGCD { get; set; }

        public Spell(Player p, double baseCD, int resourceCost, bool gcd = true)
            : base(p, baseCD)
        {
            ResourceCost = resourceCost;
            AffectedByGCD = gcd;
        }

        public void CommonSpell()
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            Player.Resource -= ResourceCost;
        }

        public virtual bool CanUse()
        {
            return Player.Resource >= ResourceCost && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override string ToString()
        {
            return "Undefined Spell";
        }
    }
}

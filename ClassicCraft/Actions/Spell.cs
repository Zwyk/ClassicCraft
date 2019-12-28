using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Spell : Action
    {
        public int Cost { get; set; }
        public bool AffectedByGCD { get; set; }
        public bool UseMana { get; set; }

        public Spell(Player p, double baseCD, int resourceCost, bool gcd = true, bool useMana = false)
            : base(p, baseCD)
        {
            Cost = resourceCost;
            AffectedByGCD = gcd;
            UseMana = useMana;
        }

        public virtual void CommonRessourceSpell()
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            Player.Resource -= Cost;
        }

        public virtual void CommonManaSpell()
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            Player.Mana -= Cost;
        }

        public override bool CanUse()
        {
            return (UseMana ? Player.Mana >= Cost : Player.Resource >= Cost) && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override string ToString()
        {
            return "Undefined Spell";
        }
    }
}

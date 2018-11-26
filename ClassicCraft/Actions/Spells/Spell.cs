using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Spell : Action
    {
        public int RessourceCost { get; set; }
        public bool AffectedByGCD { get; set; }

        public Spell(Player p, double baseCD, int ressourceCost, bool gcd = true)
            : base(p, baseCD)
        {
            RessourceCost = ressourceCost;
            AffectedByGCD = gcd;
        }

        public void CommonSpell()
        {
            CommonAction();
            Player.Ressource -= RessourceCost;
        }

        public virtual bool CanUse()
        {
            return Player.Ressource >= RessourceCost && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }

        public override string ToString()
        {
            return "Undefined Spell";
        }
    }
}

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

        public Spell(double baseCD, int ressourceCost, bool gcd = true)
            : base(baseCD)
        {
            RessourceCost = ressourceCost;
            AffectedByGCD = gcd;
        }

        public void CommonSpell()
        {
            CommonAction();
            Program.Player.Ressource -= RessourceCost;
        }

        public virtual bool CanUse()
        {
            return Program.Player.Ressource >= RessourceCost && Available() && (AffectedByGCD ? Program.Player.HasGCD() : true);
        }

        public override string ToString()
        {
            return "Undefined Spell";
        }
    }
}

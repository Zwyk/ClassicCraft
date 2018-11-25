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

        public Spell(Simulation s, double baseCD, int ressourceCost, bool gcd = true)
            : base(s, baseCD)
        {
            RessourceCost = ressourceCost;
            AffectedByGCD = gcd;
        }

        public void CommonSpell()
        {
            CommonAction();
            Sim.Player.Ressource -= RessourceCost;
        }

        public virtual bool CanUse()
        {
            return Sim.Player.Ressource >= RessourceCost && Available() && (AffectedByGCD ? Sim.Player.HasGCD() : true);
        }

        public override string ToString()
        {
            return "Undefined Spell";
        }
    }
}

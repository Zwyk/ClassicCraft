using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    abstract class Spell : Action
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
            Player.Instance.Ressource -= RessourceCost;
        }

        public bool CanUse()
        {
            return Player.Instance.Ressource >= RessourceCost && Available() && (AffectedByGCD ? Player.Instance.HasGCD() : true);
        }

        public override string ToString()
        {
            return "Undefined Spell";
        }
    }
}

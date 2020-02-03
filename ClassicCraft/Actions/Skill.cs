using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public abstract class Skill : Action
    {
        public int Cost { get; set; }
        public bool AffectedByGCD { get; set; }
        public bool UseMana { get; set; }

        public Skill(Player p, double baseCD, int resourceCost, bool gcd = true, bool useMana = false, School school = School.Physical)
            : base(p, baseCD, school)
        {
            Cost = resourceCost;
            AffectedByGCD = gcd;
            UseMana = useMana;
        }

        public void CommonRessourceSkill()
        {
            CDAction();

            if (AffectedByGCD)
            {
                Player.StartGCD();
            }

            Player.Resource -= Cost;
        }

        public override bool CanUse()
        {
            return (UseMana ? Player.Mana >= Cost : Player.Resource >= Cost) && Available() && (AffectedByGCD ? Player.HasGCD() : true);
        }
    }
}

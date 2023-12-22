using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassicCraft
{
    class SavageRoar : Skill
    {
        public override string ToString() { return NAME; }
        public static new string NAME = "Savage Roar";

        public static double[] DURATION =
        {
            14,
            19,
            24,
            29,
            34
        };

        public static Effect NewEffect(Player p)
        {
            return new CustomEffect(p, p, NAME, false, DURATION[p.Combo-1]);
        }

        public static int BASE_COST = 25;

        public SavageRoar(Player p)
            : base(p, 0, BASE_COST)
        {
        }

        public override void Cast(Entity t)
        {
            CommonRessourceSkill();
            DoAction();
        }

        public override void DoAction()
        {
            if (Player.Effects.ContainsKey(NAME))
            {
                Effect current = Player.Effects[NAME];
                current.Duration = DURATION[Player.Combo-1];
                current.Refresh();
            }
            else
            {
                NewEffect(Player).StartEffect();
            }

            Player.Resource -= Cost;
            Player.Combo = 0;

            LogAction(0);
        }
    }
}

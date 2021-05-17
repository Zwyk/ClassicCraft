using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class ActiveItemBuff : ActiveItem
    {
        string Name;
        Dictionary<Attribute, double> Attributes;
        double Duration;

        public ActiveItemBuff(Player p, double baseCD, double duration, string name, Dictionary<Attribute, double> attributes)
            : base(p, baseCD)
        {
            Name = name;
            Attributes = attributes;
            Duration = duration;
        }

        public override void DoAction()
        {
            new CustomStatsBuff(Player, Name, Duration, 1, Attributes).StartEffect();
        }
    }
}

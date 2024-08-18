using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassicCraft
{
    public class Pet : Entity
    {
        public Pet(string name, Simulation s, MobType type, int level, int armor, int maxLife, Dictionary<School, int> magicResist, Dictionary<string, Effect> baseEffects)
            : base(name, s, type, level, armor, maxLife, magicResist, baseEffects)
        {
        }
    }
}

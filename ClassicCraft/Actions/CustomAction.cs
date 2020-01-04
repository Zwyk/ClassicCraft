using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class CustomAction : Action
    {
        public string Name { get; set; }

        public CustomAction(Player p, string name, bool magic = false)
            : base(p, 0, magic)
        {
            Name = name;
        }

        public override bool CanUse()
        {
            throw new NotImplementedException();
        }

        public override void Cast()
        {
            throw new NotImplementedException();
        }

        public override void DoAction()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

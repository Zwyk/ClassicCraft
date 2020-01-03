using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class TempAction : Action
    {
        public string Name { get; set; }

        public TempAction(Player p, string name, bool magic = false)
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

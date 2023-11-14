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

        public CustomAction(Player p, string name, School school = School.Physical)
            : base(p, 0, school)
        {
            Name = name;
        }

        public override bool CanUse()
        {
            throw new NotImplementedException();
        }

        public override void Cast(Entity t)
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

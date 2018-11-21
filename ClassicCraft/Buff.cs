using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    abstract class Buff
    {
        public static string Name = "Undefined Buff";

        public double Start { get; set; }
        public double End { get; set; }
        public int Stacks { get; set; }

        public Buff(double start, double end, int stacks = 1)
        {
            Start = start;
            End = end;
            Stacks = 1;
        }

        public void StartBuff()
        {

        }

        public void StackAdd()
        {

        }

        public void StackRemove()
        {

        }

        public void EndBuff()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class ShadowTrance : Effect
    {
        public static double PROC_RATE_BY_RANK = 0.02;
        public static int LENGTH = 10;

        public double ProcRate { get; set; }

        public ShadowTrance(Player p, int rank = 2)
            : base(p, p, true, LENGTH, 1)
        {
            ProcRate = PROC_RATE_BY_RANK * rank;
        }

        public override string ToString()
        {
            return "Shadow Trance";
        }
    }
}

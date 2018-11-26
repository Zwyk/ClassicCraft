using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class PlayerObject
    {
        public Player Player { get; set; }

        public PlayerObject(Player player)
        {
            Player = player;
        }
    }
}

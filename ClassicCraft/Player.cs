using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class Player
    {
        public enum Slot
        {
            Head,
            Neck,
            Shoulders,
            Back,
            Chest,
            Wrist,
            Hands,
            Waist,
            Legs,
            Feet,
            Ring1,
            Ring2,
            Trinket1,
            Trinket2,
            MH,
            OH
        }

        public static double GCD = 1.5;

        private static Player instance = null;
        public static Player Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Player();
                }
                return instance;
            }
        }

        private int ressource;
        public int Ressource
        {
            get
            {
                return ressource;
            }
            set
            {
                if(value > 100)
                {
                    ressource = 100;
                }
                else if(value < 0)
                {
                    ressource = 0;
                }
                else
                {
                    ressource = value;
                }
            }
        }

        public Dictionary<Slot, Item> Items { get; set; }

        public Weapon MH
        {
            get
            {
                return (Weapon)Items[Slot.MH];
            }
            set
            {
                Items[Slot.MH] = value;
            }
        }

        public Weapon OH
        {
            get
            {
                return (Weapon)Items[Slot.OH];
            }
            set
            {
                Items[Slot.OH] = value;
            }
        }

        public double GCDUntil { get; set; }

        public int AP { get; set; }

        public double CritChance { get; set; }

        public double HitChance { get; set; }

        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }

        public Dictionary<string, Buff> buffs { get; set; }

        public Action applyAtNextAA = null;

        private Player()
        {
            Items = new Dictionary<Slot, Item>();
            foreach (Slot slot in (Slot[])Enum.GetValues(typeof(Slot)))
            {
                Items[slot] = null;
            }

            Reset();
        }

        public void Reset()
        {

            buffs = new Dictionary<string, Buff>();

            Ressource = 0;

            GCDUntil = 0;
        }

        public void StartGCD()
        {
            GCDUntil = Program.currentTime + GCD;
        }

        public bool HasGCD()
        {
            return GCDUntil <= Program.currentTime;
        }

        public void CalculateAttributes()
        {
            CalculateStrength();
            CalculateAgility();
            CalculateIntelligence();
        }

        public void CalculateStrength()
        {
            foreach(Item i in Items.Values)
            {
                Strength += i.Strength;
            }
        }

        public void CalculateAgility()
        {
            foreach (Item i in Items.Values)
            {
                Agility += i.Agility;
            }
        }

        public void CalculateIntelligence()
        {
            foreach (Item i in Items.Values)
            {
                Intelligence += i.Intelligence;
            }
        }
    }
}

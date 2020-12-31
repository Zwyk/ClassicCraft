using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    class RampageBuff : Effect
    {
        public static int LENGTH = 30;

        public static int AP_PER_STACK = 50;

        public RampageBuff(Player p)
            : base(p, p, true, LENGTH, 1, 5)
        {
        }

        public static void CheckProc(Player p, ResultType type, double weaponSpeed)
        {
            if (type == ResultType.Hit || type == ResultType.Crit || type == ResultType.Block || type == ResultType.Glance)
            {
                if (true) // TODO : proc chance ?
                {
                    if (p.Effects.ContainsKey(RampageBuff.NAME))
                    {
                        p.Effects[RampageBuff.NAME].StackAdd();
                    }
                    else
                    {
                        new RampageBuff(p).StartEffect();
                    }
                }
            }
        }

        public override void StartEffect()
        {
            base.StartEffect();

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + AP_PER_STACK * BaseStacks * (1 + 0.02 * Player.GetTalentPoints("IBStance")));
        }

        public override void StackAdd(int nb = 1)
        {
            int stacks = CurrentStacks;

            base.StackAdd(nb);
            
            if(stacks < CurrentStacks)
            {
                Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + AP_PER_STACK * (CurrentStacks - stacks) * BaseStacks * (1 + 0.02 * Player.GetTalentPoints("IBStance")));
            }
        }

        public override void StackRemove(int nb = 1)
        {
            int stacks = CurrentStacks;

            base.StackRemove(nb);

            if (stacks > CurrentStacks)
            {
                Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) + AP_PER_STACK * (CurrentStacks - stacks) * BaseStacks * (1 + 0.02 * Player.GetTalentPoints("IBStance")));
            }
        }

        public override void EndEffect()
        {
            base.EndEffect();

            Player.BonusAttributes.SetValue(Attribute.AP, Player.BonusAttributes.GetValue(Attribute.AP) - AP_PER_STACK * CurrentStacks * BaseStacks * (1 + 0.02 * Player.GetTalentPoints("IBStance")));
        }

        public override string ToString()
        {
            return NAME;
        }
        public static new string NAME = "Rampage's Buff";
    }
}

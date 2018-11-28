using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Simulation
    {
        public static double RATE = 100;

        public Player Player { get; set; }
        public Boss Boss { get; set; }
        public double FightLength { get; set; }

        private List<RegisteredAction> Actions { get; set; }
        private List<RegisteredEffect> Effects { get; set; }
        private double Damage { get; set; }

        public double CurrentTime { get; set; }

        public bool Ended { get; set; }

        public Simulation(Player player, Boss boss, double fightLength)
        {
            Player = player;
            Boss = boss;
            player.Sim = this;
            Boss.Sim = this;
            FightLength = fightLength;
            Actions = new List<RegisteredAction>();
            Effects = new List<RegisteredEffect>();
            Damage = 0;
            CurrentTime = 0;
            Ended = false;
        }

        public void StartSim()
        {
            List<AutoAttack> autos = new List<AutoAttack>();

            autos.Add(new AutoAttack(Player, Player.MH, true));
            if (Player.OH != null)
            {
                autos.Add(new AutoAttack(Player, Player.OH, false));
            }

            CurrentTime = 0;

            Whirlwind ww = new Whirlwind(Player);
            Bloodthirst bt = new Bloodthirst(Player);
            HeroicStrike hs = new HeroicStrike(Player);
            hs.RessourceCost -= Player.GetTalentPoints("IHS");
            Recklessness r = new Recklessness(Player);
            DeathWish dw = new DeathWish(Player);
            Execute exec = new Execute(Player);
            BloodFury bf = new BloodFury(Player);
            Bloodrage br = new Bloodrage(Player);
            BattleShout bs = new BattleShout(Player);

            Boss.LifePct = 1;

            // Pre-cast Battle Shout (starts GCD as Charge would)
            bs.Cast();

            // Charge
            Player.Ressource += 15;

            int rota = 1;

            while (CurrentTime < FightLength)
            {
                Boss.LifePct = Math.Max(0, 1 - (CurrentTime / FightLength) * (16.0 / 17.0));
                
                foreach (Effect e in Player.Effects)
                {
                    e.CheckEffect();
                }
                foreach (Effect e in Boss.Effects)
                {
                    e.CheckEffect();
                }

                Player.Effects.RemoveAll(e => e.Ended);
                Boss.Effects.RemoveAll(e => e.Ended);

                if (br.CanUse())
                {
                    br.Cast();
                }
                if(bf.CanUse())
                {
                    bf.Cast();
                }

                if (bs.CanUse() && (!Player.Effects.Any(e => e is BattleShoutBuff) || ((BattleShoutBuff)Player.Effects.Where(e => e is BattleShoutBuff).First()).RemainingTime() < Player.GCD))
                {
                    bs.Cast();
                }

                if (dw.CanUse() &&
                    (((Boss.LifePct - 0.22) * FightLength > dw.BaseCD)
                    || Boss.LifePct <= 0.2))
                {
                    dw.Cast();
                }
                if (r.CanUse() && 
                    (((Boss.LifePct - 0.22) * FightLength > r.BaseCD)
                    || (Boss.LifePct <= 0.2 && Player.Effects.Any(e => e is DeathWishBuff))))
                {
                    r.Cast();
                }

                if (rota == 0)
                {

                }
                else if (rota == 1)
                {
                    if (Boss.LifePct > 0.2)
                    {
                        if (ww.CanUse())
                        {
                            ww.Cast();
                        }
                        else if (bt.CanUse() && Player.Ressource >= ww.RessourceCost + bt.RessourceCost)
                        {
                            bt.Cast();
                        }

                        if (Player.Ressource >= 75)
                        {
                            hs.Cast();
                        }
                    }
                    else
                    {
                        if (exec.CanUse())
                        {
                            exec.Cast();
                        }
                    }
                }
                else if (rota == 2)
                {
                    if (bt.CanUse())
                    {
                        bt.Cast();
                    }
                    else if (ww.CanUse() && Player.Ressource >= ww.RessourceCost + bt.RessourceCost)
                    {
                        ww.Cast();
                    }

                    if (Player.Ressource >= 75)
                    {
                        hs.Cast();
                    }
                }

                foreach (AutoAttack a in autos)
                {
                    if (a.Available())
                    {
                        if (a.MH && Player.applyAtNextAA != null)
                        {
                            Player.applyAtNextAA.DoAction();
                            a.NextAA();
                        }
                        else
                        {
                            a.Cast();
                        }
                    }
                }

                Player.Effects.RemoveAll(e => e.Ended);
                Boss.Effects.RemoveAll(e => e.Ended);

                CurrentTime += 1 / RATE;
            }

            Program.damages.Add(Damage);
            Program.totalActions.Add(Actions);
            Program.totalEffects.Add(Effects);

            Ended = true;
        }

        public void RegisterAction(RegisteredAction action)
        {
            Actions.Add(action);
            Damage += action.Result.Damage;
        }

        public void RegisterEffect(RegisteredEffect effect)
        {
            Effects.Add(effect);
            Damage += effect.Damage;
        }
    }
}

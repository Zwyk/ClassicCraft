using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class Simulation
    {
        public static double RATE = 30;

        public Player Player { get; set; }
        public Boss Boss { get; set; }
        public double FightLength { get; set; }

        private List<RegisteredAction> Actions { get; set; }
        private List<RegisteredEffect> Effects { get; set; }
        private double Damage { get; set; }

        public double CurrentTime { get; set; }

        public bool AutoLife { get; set; }
        public double LowLifeTime { get; set; }

        public bool Ended { get; set; }

        public Random random = new Random();

        public Simulation(Player player, Boss boss, double fightLength, bool autoBossLife = true, double lowLifeTime = 0)
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
            AutoLife = autoBossLife;
            LowLifeTime = lowLifeTime;

            Ended = false;
        }

        public void StartSim()
        {
            switch (Player.Class)
            {
                case Player.Classes.Warrior: Warrior(); break;
                case Player.Classes.Druid: Druid(); break;
            }
        }

        private void Druid()
        {
            List<AutoAttack> autos = new List<AutoAttack>();

            autos.Add(new AutoAttack(Player, Player.MH, true));
            CurrentTime = 0;
            Boss.LifePct = 1;
            
            Shred shred = new Shred(Player);
            shred.ResourceCost -= Player.GetTalentPoints("IS") * 6;
            FerociousBite fb = new FerociousBite(Player);

            // Charge
            Player.Resource = 100;

            int rota = 1;

            while (CurrentTime < FightLength)
            {
                if (AutoLife)
                {
                    Boss.LifePct = Math.Max(0, 1 - (CurrentTime / FightLength) * (16.0 / 17.0));
                }
                else if (CurrentTime >= LowLifeTime && Boss.LifePct == 1)
                {
                    Boss.LifePct = 0.10;
                }

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

                Player.CheckEnergyTick();

                if (rota == 0)
                {

                }
                else if (rota == 1)
                {
                    if(Player.Combo > 4 && fb.CanUse())
                    {
                        fb.Cast();
                    }
                    else if(Player.Combo < 5 && shred.CanUse())
                    {
                        shred.Cast();
                    }
                    // TODO powershift
                }

                foreach (AutoAttack a in autos)
                {
                    if (a.Available())
                    {
                        a.Cast();
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

        private void Warrior()
        {
            List<AutoAttack> autos = new List<AutoAttack>();

            autos.Add(new AutoAttack(Player, Player.MH, true));
            if (Player.OH != null)
            {
                autos.Add(new AutoAttack(Player, Player.OH, false));
            }

            CurrentTime = 0;
            Boss.LifePct = 1;

            Whirlwind ww = new Whirlwind(Player);
            Bloodthirst bt = new Bloodthirst(Player);
            HeroicStrike hs = new HeroicStrike(Player);
            hs.ResourceCost -= Player.GetTalentPoints("IHS");
            Execute exec = new Execute(Player);
            Bloodrage br = new Bloodrage(Player);
            BattleShout bs = new BattleShout(Player);
            Hamstring ham = new Hamstring(Player);

            Dictionary<Spell, int> cds = null;
            if (Player.Cooldowns != null)
            {
                cds = new Dictionary<Spell, int>();
                foreach (string s in Player.Cooldowns)
                {
                    switch (s)
                    {
                        case "Death Wish": cds.Add(new DeathWish(Player), DeathWishBuff.LENGTH); break;
                        case "Juju Flurry": cds.Add(new JujuFlurry(Player), JujuFlurryBuff.LENGTH); break;
                        case "Mighty Rage": cds.Add(new MightyRage(Player), MightyRageBuff.LENGTH); break;
                        case "Recklessness": cds.Add(new Recklessness(Player), RecklessnessBuff.LENGTH); break;
                        case "Racial":
                            if (Player.Race == Player.Races.Orc)
                            {
                                cds.Add(new BloodFury(Player), BloodFuryBuff.LENGTH);
                            }
                            else if (Player.Race == Player.Races.Troll)
                            {
                                cds.Add(new Berserking(Player), BerserkingBuff.LENGTH);
                            }
                            break;
                    }
                }
            }

            // Pre-cast Battle Shout (starts GCD as Charge would)
            bs.Cast();

            // Charge
            Player.Resource += 15;

            int rota = 1;

            while (CurrentTime < FightLength)
            {
                if(AutoLife)
                {
                    Boss.LifePct = Math.Max(0, 1 - (CurrentTime / FightLength) * (16.0 / 17.0));
                }
                else if(CurrentTime >= LowLifeTime && Boss.LifePct == 1)
                {
                    Boss.LifePct = 0.10;
                }
                
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

                if (br.CanUse() && Player.Resource <= 90)
                {
                    br.Cast();
                }

                if (bs.CanUse() && (!Player.Effects.Any(e => e is BattleShoutBuff) || ((BattleShoutBuff)Player.Effects.Where(e => e is BattleShoutBuff).First()).RemainingTime() < Player.GCD))
                {
                    bs.Cast();
                }


                if(cds != null)
                {
                    foreach (Spell cd in cds.Keys)
                    {
                        if (cd.CanUse() &&
                            (FightLength - CurrentTime <= cds[cd]
                            || FightLength - CurrentTime >= cd.BaseCD + cds[cd]))
                        {
                            cd.Cast();
                        }
                    }
                }

                if (rota == 0)
                {

                }
                else if (rota == 1)
                {
                    if (Boss.LifePct > 0.2)
                    {
                        if (bt.CanUse())
                        {
                            bt.Cast();
                        }
                        else if (ww.CanUse() && Player.Resource >= ww.ResourceCost + bt.ResourceCost && bt.RemainingCD() >= Player.GCD)
                        {
                            ww.Cast();
                        }
                        else if(ham.CanUse() && Player.Resource >= Bloodthirst.COST + Whirlwind.COST + Hamstring.COST && ww.RemainingCD() >= Player.GCD && bt.RemainingCD() >= Player.GCD && (!Player.Effects.Any(e => e is Flurry) || ((Flurry)Player.Effects.Where(f => f is Flurry).First()).CurrentStacks < 3))
                        {
                            ham.Cast();
                        }

                        if (Player.Resource >= Bloodthirst.COST + Whirlwind.COST + HeroicStrike.COST && hs.CanUse())
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
                    if (ww.CanUse())
                    {
                        ww.Cast();
                    }
                    else if (bt.CanUse() && Player.Resource >= ww.ResourceCost + bt.ResourceCost)
                    {
                        bt.Cast();
                    }
                    else if (ham.CanUse() && Player.Resource >= Bloodthirst.COST + Whirlwind.COST + Hamstring.COST && ww.RemainingCD() >= Player.GCD && bt.RemainingCD() >= Player.GCD && (!Player.Effects.Any(e => e is Flurry) || ((Flurry)Player.Effects.Where(f => f is Flurry).First()).CurrentStacks < 3))
                    {
                        ham.Cast();
                    }

                    if (Player.Resource >= Bloodthirst.COST + Whirlwind.COST + HeroicStrike.COST && hs.CanUse())
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

        public static double Normalization(Weapon w)
        {
            if (w.Type == Weapon.WeaponType.Dagger)
            {
                return 1.7;
            }
            else if (w.TwoHanded)
            {
                return 3.3;
            }
            else
            {
                return 2.4;
            }
        }

        public double DamageMod(ResultType type, int level = 60, int enemyLevel = 63)
        {
            switch (type)
            {
                // TODO BLOCK / BLOCKCRIT
                case ResultType.Crit: return 2;
                case ResultType.Hit: return 1;
                case ResultType.Glancing: return GlancingDamage(level, enemyLevel);
                default: return 0;
            }
        }

        public double RageDamageMod(ResultType type, int level = 60, int enemyLevel = 63)
        {
            switch (type)
            {
                case ResultType.Crit: return 2;
                case ResultType.Glancing: return GlancingDamage(level, enemyLevel);
                case ResultType.Miss: return 0;
                default: return 1;
            }
        }

        public double GlancingDamage(int level = 60, int enemyLevel = 63)
        {
            double low = Math.Max(0.01, Math.Min(0.91, 1.3 - 0.05 * (enemyLevel - level)));
            double high = Math.Max(0.2, Math.Min(0.99, 1.2 - 0.03 * (enemyLevel - level)));
            return random.NextDouble() * (high - low) + low;
        }

        public static double RageGained(int damage, int level = 60)
        {
            return Math.Max(1, damage / RageConversionValue(level) * 7.5);
        }

        /*
        public static double RageGained2(int damage, double weaponSpeed, ResultType type, bool mh = true, int level = 60)
        {
            return Math.Max(1, (15 * damage) / (4 * RageConversionValue(level)) + (RageWhiteHitFactor(mh, type == ResultType.Crit) * weaponSpeed) / 2);
        }
        */

        public static double RageConversionValue(int level = 60)
        {
            return 0.0091107836 * Math.Pow(level,2) + 3.225598133 * level + 4.2652911;
        }

        public static double RageWhiteHitFactor(bool mh, bool crit)
        {
            if (mh)
            {
                if (crit)
                {
                    return 7.0;
                }
                else
                {
                    return 3.5;
                }
            }
            else
            {
                if (crit)
                {
                    return 3.5;
                }
                else
                {
                    return 1.75;
                }
            }
        }
    }
}

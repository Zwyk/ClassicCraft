using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
    public class SimResult
    {
        public double FightLength;
        public List<RegisteredAction> Actions;
        public List<RegisteredEffect> Effects;

        public SimResult(double fightLength)
        {
            FightLength = fightLength;
            Actions = new List<RegisteredAction>();
            Effects = new List<RegisteredEffect>();
        }
    }

    public class Simulation
    {
        public static double RATE = 30;

        public Player Player { get; set; }
        public Boss Boss { get; set; }
        public double FightLength { get; set; }

        private SimResult Results { get; set; }
        private double Damage { get; set; }

        public double CurrentTime { get; set; }

        public bool AutoLife { get; set; }
        public double LowLifeTime { get; set; }

        public bool Ended { get; set; }

        public List<AutoAttack> autos = new List<AutoAttack>();

        public Simulation(Player player, Boss boss, double fightLength, bool autoBossLife = true, double lowLifeTime = 0, double fightLengthMod = 0.2)
        {
            Player = player;
            Boss = boss;
            player.Sim = this;
            Boss.Sim = this;
            FightLength = fightLength * (1 + fightLengthMod/2 - (Randomer.NextDouble() * fightLengthMod));
            Results = new SimResult(FightLength);
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
                case Player.Classes.Rogue: Rogue(); break;
            }
        }

        private void Rogue()
        {
            autos.Add(new AutoAttack(Player, Player.MH, true));
            if (Player.DualWielding)
            {
                autos.Add(new AutoAttack(Player, Player.OH, false));
            }

            CurrentTime = 0;
            Boss.LifePct = 1;

            Ambush am = new Ambush(Player);
            Backstab bs = new Backstab(Player);
            Eviscerate ev = new Eviscerate(Player);
            SinisterStrike ss = new SinisterStrike(Player);
            SliceAndDice sad = new SliceAndDice(Player);
            AdrenalineRush ar = new AdrenalineRush(Player);
            BladeFlurry bf = new BladeFlurry(Player);

            Player.Stealthed = true;

            Player.HasteMod = Player.CalcHaste();

            Player.Resource = Player.MaxResource;

            int rota = 0;
            if (Player.MH.Type == Weapon.WeaponType.Dagger)
            {
                rota = 1;
            }
            
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

                double sadleft = 0;
                if(Player.Effects.Any(e => e is SliceAndDiceBuff))
                {
                    sadleft = Player.Effects.Where(e => e is SliceAndDiceBuff).First().RemainingTime();
                }

                if(sadleft > 0)
                {
                    if (bf.CanUse())
                    {
                        bf.Cast();
                    }
                    if (ar.CanUse())
                    {
                        ar.Cast();
                    }
                }

                if (rota == 0) // SS + EV
                {
                    if (FightLength - CurrentTime > SliceAndDiceBuff.DurationCalc(Player) && Player.Combo > 0 && sadleft == 0 && sad.CanUse())
                    {
                        sad.Cast();
                    }
                    else if (Player.Combo > 4)
                    {
                        if (FightLength - CurrentTime > SliceAndDiceBuff.DurationCalc(Player) && sadleft < 10)
                        {
                            if (Player.Resource >= 80 && sad.CanUse())
                            {
                                sad.Cast();
                            }
                        }
                        else if (ev.CanUse())
                        {
                            ev.Cast();
                        }
                    }
                    else if (ss.CanUse())
                    {
                        ss.Cast();
                    }
                }
                else if(rota == 1) // BS + EV
                {
                    if (FightLength - CurrentTime > SliceAndDiceBuff.DurationCalc(Player) && Player.Combo > 1 && sadleft == 0 && sad.CanUse())
                    {
                        sad.Cast();
                    }
                    else if (Player.Combo > 4)
                    {
                        if (FightLength - CurrentTime > SliceAndDiceBuff.DurationCalc(Player) && sadleft < 10)
                        {
                            if (Player.Resource >= 80 && sad.CanUse())
                            {
                                sad.Cast();
                            }
                        }
                        else if (ev.CanUse())
                        {
                            ev.Cast();
                        }
                    }
                    else if (bs.CanUse())
                    {
                        bs.Cast();
                    }
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

            Program.AddSimDps(Damage / FightLength);
            Program.AddSimResult(Results);

            Ended = true;
        }

        private void Druid()
        {
            autos.Add(new AutoAttack(Player, Player.MH, true));

            CurrentTime = 0;
            Boss.LifePct = 1;
            
            Shred shred = new Shred(Player);
            FerociousBite fb = new FerociousBite(Player);
            Shift shift = new Shift(Player);
            MCP mcp = new MCP(Player);
            Innervate innerv = new Innervate(Player);
            Player.Form = Player.Forms.Cat;

            Player.HasteMod = Player.CalcHaste();

            Player.Resource = Player.MaxResource;
            Player.Mana = Player.MaxMana;

            if (Player.Equipment[Player.Slot.MH].Name == "Manual Crowd Pummeler")
            {
                mcp.Cast();
            }

            int rota = 0;
            
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
                Player.CheckManaTick();

                if (rota == 0) //SHRED + FB + SHIFT + INNERV
                {
                    if(Player.Form == Player.Forms.Cat)
                    {
                        if (shred.CanUse() && (
                            Player.Combo < 5
                            || Player.Effects.Any(e => e is ClearCasting)
                            || (Player.Combo > 4 && Player.Resource > fb.Cost + shred.Cost - (20 * (Player.GCDUntil - CurrentTime) / Player.GCD))
                            ))
                        {
                            shred.Cast();
                        }
                        else if (Player.Combo > 4 && fb.CanUse())
                        {
                            fb.Cast();
                        }
                        else if (Player.Resource < shred.Cost - 20 && shift.CanUse() && (innerv.Available() || Player.Effects.Any(e => e is InnervateBuff) || ((int)((double)Player.Mana / shift.Cost)) * 4 + 5 >= FightLength - CurrentTime || !(Player.ManaTicking() && Player.Mana + Player.MPT() < Player.MaxMana)))
                        {
                            shift.Cast();
                        }
                        else if (Player.Mana < shift.Cost && innerv.CanUse())
                        {
                            Player.Form = Player.Forms.Human;
                            ResetAATimer(autos[0]);
                            innerv.Cast();
                        }
                    }
                    else if(Player.Form == Player.Forms.Human && shift.CanUse())
                    {
                        Player.Form = Player.Forms.Cat;
                        ResetAATimer(autos[0]);
                        shift.Cast();
                    }
                }
                
                if (autos[0].Available())
                {
                    autos[0].Cast();
                }

                Player.Effects.RemoveAll(e => e.Ended);
                Boss.Effects.RemoveAll(e => e.Ended);

                CurrentTime += 1 / RATE;
            }

            Program.AddSimDps(Damage / FightLength);
            Program.AddSimResult(Results);

            Ended = true;
        }

        private void Warrior()
        {
            autos.Add(new AutoAttack(Player, Player.MH, true));
            if (Player.DualWielding)
            {
                autos.Add(new AutoAttack(Player, Player.OH, false));
            }

            CurrentTime = 0;
            Boss.LifePct = 1;

            Whirlwind ww = new Whirlwind(Player);
            Bloodthirst bt = new Bloodthirst(Player);
            HeroicStrike hs = new HeroicStrike(Player);
            hs.Cost -= Player.GetTalentPoints("IHS");
            Execute exec = new Execute(Player);
            Bloodrage br = new Bloodrage(Player);
            BattleShout bs = new BattleShout(Player);
            Hamstring ham = new Hamstring(Player);

            Player.HasteMod = Player.CalcHaste();

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
            //bs.Cast();

            // Charge
            Player.Resource += 15;

            int rota = 0;

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

                /*
                if (bs.CanUse() && (!Player.Effects.Any(e => e is BattleShoutBuff) || ((BattleShoutBuff)Player.Effects.Where(e => e is BattleShoutBuff).First()).RemainingTime() < Player.GCD))
                {
                    bs.Cast();
                }
                */


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

                if (rota == 0) //BT > WW > HAM + HS + EXEC
                {
                    if (Boss.LifePct > 0.2)
                    {
                        if (bt.CanUse())
                        {
                            bt.Cast();
                        }
                        else if (ww.CanUse() && Player.Resource >= ww.Cost + bt.Cost && bt.RemainingCD() >= Player.GCD)
                        {
                            ww.Cast();
                        }
                        else if(ham.CanUse() && Player.Resource >= bt.Cost + ww.Cost + hs.Cost && ww.RemainingCD() >= Player.GCD && bt.RemainingCD() >= Player.GCD && (!Player.Effects.Any(e => e is Flurry) || ((Flurry)Player.Effects.Where(f => f is Flurry).First()).CurrentStacks < 3))
                        {
                            ham.Cast();
                        }

                        if (!Player.MH.TwoHanded && Player.Resource >= bt.Cost + ww.Cost + hs.Cost && hs.CanUse())
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

                foreach (AutoAttack a in autos)
                {
                    if (a.Available())
                    {
                        if (a.MH && Player.applyAtNextAA != null)
                        {
                            if (Player.applyAtNextAA.CanUse())
                            {
                                Player.applyAtNextAA.DoAction();
                                a.CastNextSwing();
                            }
                            else
                            {
                                Player.applyAtNextAA = null;
                                a.Cast();
                            }  
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
            
            Program.AddSimDps(Damage / FightLength);
            Program.AddSimResult(Results);

            Ended = true;
        }

        private void ResetAATimer(AutoAttack auto)
        {
            auto.CastNextSwing();
        }

        private void ResetAATimers(List<AutoAttack> autos)
        {
            foreach (AutoAttack a in autos)
            {
                ResetAATimer(a);
            }
        }

        public void RegisterAction(RegisteredAction action)
        {
            Results.Actions.Add(action);
            Damage += action.Result.Damage;
        }

        public void RegisterEffect(RegisteredEffect effect)
        {
            Results.Effects.Add(effect);
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
                case ResultType.Glance: return GlancingDamage(level, enemyLevel);
                default: return 0;
            }
        }

        public double RageDamageMod(ResultType type, int level = 60, int enemyLevel = 63)
        {
            switch (type)
            {
                case ResultType.Crit: return 2;
                case ResultType.Glance: return GlancingDamage(level, enemyLevel);
                case ResultType.Miss: return 0;
                default: return 1;
            }
        }

        public double GlancingDamage(int level = 60, int enemyLevel = 63)
        {
            double low = Math.Max(0.01, Math.Min(0.91, 1.3 - 0.05 * (enemyLevel - level)));
            double high = Math.Max(0.2, Math.Min(0.99, 1.2 - 0.03 * (enemyLevel - level)));
            return Randomer.NextDouble() * (high - low) + low;
        }

        public static double RageGained(int damage, int level = 60)
        {
            return Math.Max(1, 7.5 * damage / RageConversionValue(level));
        }

        /*
        public static double RageGained2(int damage, double speed, bool mh, bool crit, int level = 60)
        {
            return Math.Max(15 * damage / RageConversionValue(level), 15 * damage / (4 * RageConversionValue(level)) + (RageWhiteHitFactor(mh, crit) * speed / 2));
        }
        */

        public static double RageConversionValue(int level = 60)
        {
            return 0.0091107836 * Math.Pow(level,2) + 3.225598133 * level + 4.2652911;
        }

        public static double RageWhiteHitFactor(bool mh, bool crit)
        {
            return 1.25 * (mh ? 2 : 1) * (crit ? 2 : 1);
        }
    }
}

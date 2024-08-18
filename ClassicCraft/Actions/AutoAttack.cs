using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{

    public class AutoAttack : Action
    {
        public enum AAType
        {
            Melee,
            Ranged,
            Wand
        }

        public bool isMH { get; set; }
        public Weapon Weapon { get; set; }
        public AAType Type { get; set; }

        public static double CAST_WAND = 0.75;

        public AutoAttack(Player p, Weapon weapon, bool ismh, AAType type = AAType.Melee)
            : base(p, weapon.Speed, weapon.School)
        {
            Weapon = weapon;
            isMH = ismh;
            Type = type;
        }

        public void WandFirstCast()
        {
            LockedUntil = Player.Sim.CurrentTime + CAST_WAND;
            if (Program.logFight)
            {
                string log = string.Format("{0:N2} : {1} started", Player.Sim.CurrentTime, ToString());
                if (Player.Sim.NbTargets > 1)
                {
                    for (int i = 0; i < Player.Sim.Boss.Count; i++)
                    {
                        if (Player.Sim.Boss[i] == Target)
                        {
                            log += string.Format(" on Target {0}", i + 1);
                        }
                    }
                }
                if (Player.CurrentMainResource != Player.Resources.Mana)
                {
                    log += string.Format(" ({0} {1}/{2})", Player.CurrentMainResource, Player.Resource, Player.MaxResource);
                }
                if (Player.MaxMana > 0)
                {
                    log += " - Mana " + Player.Mana + "/" + Player.MaxMana;
                }
                Program.Log(log);
            }
        }

        public override void Cast(Entity t)
        {
            Target = t;
            DoAction();
        }

        public override void DoAction()
        {
            DoAA();
            CastNextSwing();
            if(Type == AAType.Wand)
            {
                Player.StartGCD(CurrentSpeed(), true);
            }
        }

        public void DoAA(List<string> alreadyProc = null, bool extra = false, bool isYellow = false)
        {
            double mitigation = 1;
            ResultType res;
            if (Type == AAType.Wand)
            {
                mitigation = Simulation.MagicMitigation(Target.ResistChances[School]);
                res = mitigation == 0 ? ResultType.Resist : Player.RangedMagicAttackEnemy(Target);
            }
            else if (Type == AAType.Ranged)
            {
                res = ResultType.Hit;
                // TODO
            }
            else if (isYellow || (Player.Class == Player.Classes.Warrior && !isMH && Player.applyAtNextAA != null))
            {
                res = Player.YellowAttackEnemy(Target);
            }
            else
            {
                res = Player.WhiteAttackEnemy(Target, isMH);
            }

            int minDmg, maxDmg, damage;
            if(Type == AAType.Wand)
            {
                minDmg = (int)Math.Round(Weapon.DamageMin);
                maxDmg = (int)Math.Round(Weapon.DamageMax);
                damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                    * Player.Sim.DamageMod(res, Weapon.School, true, true)
                    * Player.DamageMod
                    * (1 + (Player.Class == Player.Classes.Priest ? 0.05 : 0.125) * Player.GetTalentPoints("Wand"))
                    * mitigation
                    * (School == School.Shadow && Player.Class == Player.Classes.Priest ? 1.15 * 1.15 : 1) // shadow weaving + form
                    );
            }
            else
            {
                minDmg = (int)Math.Round(
                    Player.Form == Player.Forms.Cat ? Player.Level * 0.85 + (Player.AP + Player.nextAABonus) / 14 :
                    (Player.Form == Player.Forms.Bear ? Player.Level * 0.85 + 2.5 * (Player.AP + Player.nextAABonus) / 14 :
                    (Weapon.DamageMin + Weapon.Speed * (Player.AP + Player.nextAABonus) / 14)));
                maxDmg = (int)Math.Round(
                    Player.Form == Player.Forms.Cat ? Player.Level * 1.25 + (Player.AP + Player.nextAABonus) / 14 :
                    (Player.Form == Player.Forms.Bear ? Player.Level * 1.25 + 2.5 * (Player.AP + Player.nextAABonus) / 14 :
                    (Weapon.DamageMax + Weapon.Speed * (Player.AP + Player.nextAABonus) / 14)));
                damage = (int)Math.Round(Randomer.Next(minDmg, maxDmg + 1)
                    * Player.Sim.DamageMod(res, Weapon.School, isMH, true)
                    * Simulation.ArmorMitigation(Target.Armor, Player.Level, Player.Attributes.GetValue(Attribute.ArmorPen))
                    * Player.DamageMod
                    * (Player.DualWielding ? (isMH ? 1 : 0.5 * (1 + ((Player.Class == Player.Classes.Rogue ? 0.1 : 0.05) * Player.GetTalentPoints("DWS")))) : (1 + 0.01 * Player.GetTalentPoints("2HS")))
                    * (Program.version == Version.TBC && !Player.MH.TwoHanded ? 1 + 0.02 * Player.GetTalentPoints("1HS") : 1)
                    * (1 + (Player.Class == Player.Classes.Druid ? Player.GetTalentPoints("NW") * 0.02 : 0))
                    * mitigation
                    * (res == ResultType.Crit && Player.Buffs.Any(bu => bu.Name.ToLower().Contains("relentless") || bu.Name.ToLower().Contains("chaotic")) ? 1.03 : 1)
                    * (Target.Effects.ContainsKey("Blood Frenzy") ? 1.04 : 1)
                    * (1 + (Player.Class == Player.Classes.Rogue && res == ResultType.Crit && Weapon.Type == Weapon.WeaponType.Mace ? 0.01 * Player.GetTalentPoints("Mace") : 0))
                    );
            }
            Player.nextAABonus = 0;

            int threat = (int)Math.Round(damage * Player.ThreatMod);

            if (Player.CurrentMainResource == Player.Resources.Rage)
            {
                double gain = Simulation.RageGained(damage, Player.Level, isMH, res == ResultType.Crit, Weapon.Speed);
                gain *= Player.Runes.Contains("Endless Rage") ? 1.25 : 1;
                Player.Resource += (int)Math.Round(gain);
            }

            RegisterDamage(new ActionResult(res, damage, threat));
            
            if(Type == AAType.Melee)
            {
                Player.CheckOnHits(isMH, true, res, extra, alreadyProc);
            }

            if(Player.Class == Player.Classes.Warrior)
            {
                SweepingStrikesBuff.CheckProc(Player, damage, res);
            }
            else if(Player.Class == Player.Classes.Rogue)
            {
                BladeFlurryBuff.CheckProc(Player, damage, res);
            }
        }

        public void Swing()
        {
            LockedUntil = Player.Sim.CurrentTime;
        }

        public void ResetSwing()
        {
            LockedUntil = Player.Sim.CurrentTime + CurrentSpeed();
        }

        public void CastNextSwing()
        {
            LockedUntil += CurrentSpeed();
        }

        public double CurrentSpeed()
        {
            return (Player.Form == Player.Forms.Cat ? 1 : (Player.Form == Player.Forms.Bear ? 2.5 : BaseCD)) / Player.HasteMod;
        }

        public override string ToString()
        {
            return "AA " + (Type == AAType.Wand ? "Wand" : (Type == AAType.Ranged ? "Ranged" : (isMH ? "MH" : "OH")));
        }

        public override bool CanUse()
        {
            return Available();
        }
    }
}

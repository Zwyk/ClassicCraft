using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicCraft
{
	class Paladin : Player
	{
		//  Buffs
		private Vengeance vng;

		//	Spells
		private SealOfTheCrusader sotc;
		private SealOfRighteousness sor;
		private SealOfCommand_Rank5 soc5;
		private SealOfCommand_Rank1 soc1;
		private Judgement jdg;
		private HammerOfWrath how;
		private Consecration_Rank5 cons5;
		private Consecration_Rank1 cons1;

		//  Consumes
		private ManaPotion mpot;

		public Paladin(Player p)
			: base(p)
		{
		}

		public Paladin(Simulation s, Player p)
			: base(s, p)
		{
		}

		public Paladin(Simulation s = null, Races r = Races.Human, int level = 60, Dictionary<Slot, Item> items = null, Dictionary<string, int> talents = null, List<Enchantment> buffs = null)
			: base(s, Classes.Paladin, r, level, items, talents, buffs)
		{
		}

		#region Rota

		public override void PrepFight()
		{
			base.PrepFight();

			Mana = MaxMana;

			vng = new Vengeance(this);

			sotc = new SealOfTheCrusader(this);
			sor = new SealOfRighteousness(this);
			soc5 = new SealOfCommand_Rank5(this);
			soc1 = new SealOfCommand_Rank1(this);

			jdg = new Judgement(this);
			how = new HammerOfWrath(this);
			cons5 = new Consecration_Rank5(this);
			cons1 = new Consecration_Rank1(this);

			mpot = new ManaPotion(this);

			HasteMod = CalcHaste();

			if (Cooldowns != null)
			{
				foreach (string s in Cooldowns)
				{
					switch (s)
					{
						//case "Juju Flurry": cds.Add(new JujuFlurry(this), JujuFlurryBuff.LENGTH); break;
						case "Racial":
							//  No combat racial abilities for Paladins
							break;
					}
				}
			}
		}

		public override void Rota()
		{
			//
			//	Rotations
			//
			//	0: JotC, SoC, JoC (AP build)
			//	1: JotC, SoR, JoC (nightfall max proc chance)
			//	2: JotC, SoR, JoR (spelldmg build)
			//
			//	All rotations cast max-rank Consecration when Vengeance is up,
			//		otherwise cast rank 1 Consecration on cooldown.
			//

			//  use a mana pot if we're low
			if (mpot.CanUse() && (MaxMana - Mana) >= ManaPotion.MAX)
			{
				mpot.Cast();
			}

			/*	This isn't ready yet, apparently
            if (bom.CanUse() && (!Effects.Any(e => e is BlessingOfMightBuff) || ((BlessingOfMightBuff)Effects.Where(e => e is BlessingOfMightBuff).First()).RemainingTime() < GCD))
            {
				bom.Cast();
            }
			*/

			if (cds != null)
			{
				foreach (Skill cd in cds.Keys)
				{
					if (cd.CanUse() &&
						(Sim.FightLength - Sim.CurrentTime <= cds[cd]
						|| Sim.FightLength - Sim.CurrentTime >= cd.BaseCD + cds[cd]))
					{
						cd.Cast();
					}
				}
			}

#if false  //	HoW is not currently part of AP Ret max DPS
			//	Use HoW on CD if boss is under 20%
			if (Sim.Boss.LifePct < 0.2)
			{
				if (how.CanUse() && Mana >= how.Cost)
				{
					how.Cast();
				}
			}
#endif

			//	If crusader is already on teh boss, seal up a damage seal
			if (Sim.Boss.Effects.ContainsKey(JudgementOfTheCrusaderDebuff.NAME))
			{
				//	Judge command or righteousness, if possible
				if (
					Effects.ContainsKey(SealOfCommandBuff_Rank5.NAME)
					|| Effects.ContainsKey(SealOfCommandBuff_Rank1.NAME)
					|| (rota == 2 && Effects.ContainsKey(SealOfRighteousnessBuff.NAME))
					)
				{
					if (jdg.CanUse() && Mana >= jdg.Cost)
					{
						jdg.Cast();
					}
				}
				else
				{
					//
					//	SoC is not currently active
					//

					//
					//	If using Nightfall, a different rotation is used: SoR, SoC, JoC
					//
					if (rota == 1)
					{
						//	If judgement is available (or will be available in the next half second), seal up with Command
						if (jdg.RemainingCD() <= 0.5) {
							if (Mana >= soc5.Cost)
							{
								soc5.Cast();
							}
							else if (Mana >= soc1.Cost)
							{
								soc1.Cast();
							}
						}
						else {
							//	Judgement isn't available, so ensure SoR is active
							if (!Effects.ContainsKey(SealOfRighteousnessBuff.NAME) && sor.CanUse() && Mana >= sor.Cost)
							{
								sor.Cast();
							}
						}
					}
					else
					{
						//
						//	Non-Nightfall rotation
						//

						//	SoR rotation
						if (rota == 2)
						{
							//	Turn on SoR
							if (sor.CanUse() && Mana >= sor.Cost)
							{
								sor.Cast();
							}
						}
						else
						{
							//	Otherwise, seal up command
							if (soc5.CanUse() && Mana >= soc5.Cost)
							{
								//	Rank 5 if mana...
								soc5.Cast();
							}
							else if (soc1.CanUse() && Mana >= soc1.Cost)
							{
								//	Otherwise, Rank 1
								soc1.Cast();
							}
						}
					}
				}
			}
			else
			{
				//	Otherwise, judge crusader
				if (Effects.ContainsKey(SealOfTheCrusaderBuff.NAME))
				{
					if (jdg.CanUse() && Mana >= jdg.Cost)
					{
						jdg.Cast();
					}
				}
				else
				{
					//	Or apply the seal
					if (sotc.CanUse() && Mana >= sotc.Cost)
					{
						sotc.Cast();
					}
				}
			}

			//	Cast max-rank consecration if Vengeance is up
			if (GetTalentPoints("Consecration") > 0)
			{
				if (Effects.ContainsKey(Vengeance.NAME) && cons5.CanUse() && Mana >= cons5.Cost)
				{
					cons5.Cast();

					//	Trigger CD on other ranks
					cons1.CDAction();
				}
				else if (cons1.CanUse() && Mana >= cons1.Cost)
				{
					cons1.Cast();

					//	Trigger CD on other ranks
					cons5.CDAction();
				}
			}

			CheckAAs();
		}

#endregion

#region Talents

		public override void SetupTalents(string ptal)
		{
			if (ptal == null || ptal == "")
			{
				// DPS Ret			505031--55235051200315
				// Nightfall Ret	550531--55230051200315
				ptal = "505031--55235051200315";
			}

			string[] talents = ptal.Split('-');
			string holy = talents.Length > 0 ? talents[0] : "";
			string prot = talents.Length > 1 ? talents[1] : "";
			string ret = talents.Length > 2 ? talents[2] : "";

			Talents = new Dictionary<string, int>();

			// Holy
			Talents.Add("DivStr", holy.Length > 0 ? (int)Char.GetNumericValue(holy[0]) : 0);
			Talents.Add("DivInt", holy.Length > 1 ? (int)Char.GetNumericValue(holy[1]) : 0);
			Talents.Add("SpiFocus", holy.Length > 2 ? (int)Char.GetNumericValue(holy[2]) : 0);
			Talents.Add("ImpSoR", holy.Length > 3 ? (int)Char.GetNumericValue(holy[3]) : 0);
			Talents.Add("HealingLight", holy.Length > 4 ? (int)Char.GetNumericValue(holy[4]) : 0);
			Talents.Add("Consecration", holy.Length > 5 ? (int)Char.GetNumericValue(holy[5]) : 0);

			// Ret
			Talents.Add("ImpBoM", ret.Length > 0 ? (int)Char.GetNumericValue(ret[0]) : 0);
			Talents.Add("Bene", ret.Length > 1 ? (int)Char.GetNumericValue(ret[1]) : 0);
			Talents.Add("ImpJ", ret.Length > 2 ? (int)Char.GetNumericValue(ret[2]) : 0);
			Talents.Add("ImpSotC", ret.Length > 3 ? (int)Char.GetNumericValue(ret[3]) : 0);
			Talents.Add("Defl", ret.Length > 4 ? (int)Char.GetNumericValue(ret[4]) : 0);
			Talents.Add("Conv", ret.Length > 6 ? (int)Char.GetNumericValue(ret[6]) : 0);
			Talents.Add("SoC", ret.Length > 7 ? (int)Char.GetNumericValue(ret[7]) : 0);
			Talents.Add("PoJ", ret.Length > 10 ? (int)Char.GetNumericValue(ret[10]) : 0);
			Talents.Add("2HWS", ret.Length > 11 ? (int)Char.GetNumericValue(ret[11]) : 0);
			Talents.Add("Sanc", ret.Length > 12 ? (int)Char.GetNumericValue(ret[12]) : 0);
			Talents.Add("Veng", ret.Length > 13 ? (int)Char.GetNumericValue(ret[13]) : 0);
		}

#endregion
	}
}

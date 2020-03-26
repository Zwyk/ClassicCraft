# ClassicCraft
*A WoW Classic simulation tool*

### How to use :
- Download and extract the content of the .zip file anywhere on your computer
- Modify `sim.json` to setup the simulation
	- As of 0.3.1, you can use the [ClassicCraft Addon on Curse](https://www.curseforge.com/wow/addons/classiccraft) to speed up the process (but at the moment some stats like Crit or Hit rating aren't supported by the addon, so you need to fill them by hand afterwards)
	- You can use the examples in the folder `sim.json examples` if needed
- Execute `ClassicCraft.exe`
	- A console should appear and the simulations start
	- Depending on the desired accuracy (`TargerErrorPct` or `NbSim`) and if `StatsWeights` is activated, the process might take some time
- A new .txt file with the logs of the simulation is created in the `Logs` folder, press any key in the console window to close it

### Current features :
- DPS Simulation
	- Character setup
		- Class, Race
		- Talents (use Addon export or classic.wowhead.com/talent-calc/*your_class*/*copy-paste_that_part*)
		- Gear (some special sets like Rogue's T0 6P and T1 5P)
		- Enchantments (Crusader)
		- Consumables
		- CDs to use (Racial, Recklessness, etc.)
		- Raid buffs (Windfury, etc.)
	- Encounter setup
		- Boss Level/Armor
		- Length (+ boss low life length)
	- Statistics output as raw text
		- Overall DPS
		- DPS and Hit stats by spell (Hit, Miss, Crit, Glancing, etc.)
		- Stats weights (1 AP = X DPS, 1 STR = X DPS or X AP, etc.)
- Classes supported
	- Warrior
		- Fury BT (1H or 2H)
	- Druid
		- Feral (Powershifting)
	- Rogue
		- Combat (SS or BS)
	- Paladin
		- Retribution
			- Rotations
				- 0: SotC+SoC/JoC + Vengeance+Cons5|Cons1 [Attack Power]
				- 1: SotC+SoR/JoC + Vengeance+Cons5|Cons1 [Nightfall Max-Uptime]
				- 2: SotC+SoR/JoR + Vengeance+Cons5|Cons1 [SpellDmg]
			- Equipment Samples
				- AP
				- HitCap for Nightfall
				- SpellDmg
- Built-In Weapons
	- Nightfall (debuff functional, use Nightfall max-uptime rotation)
	- Hanzo Sword (proc TBD)
	- The Unstoppable Force (proc TBD)
	- Obsidian Edged Blade (weapon skill dropoff TBD)
	- Bonereaver's Edge (proc TBD)
	- Ashkandi, Greatsword of the Brotherhood
	- Sulfuras, Hand of Ragnaros (proc TBD)

### JSON Attributes List
- `Sta` : Stamina
- `Str` : Strength
- `Agi` : Agility
- `Int` : Intellect
- `Spi` : Spirit
- `HP` : Health
- `Mana` : Mana
- `Armor` : Armor
- `ArmorPen` : Armor Penetration
- `Haste` : Haste(Attack Speed) %
- `AP` : Attack Power
- `RAP` : Ranged Attack Power
- `SP` : Spell Power
- `HSP` : Healing Spell Power
- `Hit` : Hit Chance %
- `SpellHit` : Spell Hit Chance %
- `Crit` : Crit Chance %
- `SpellCrit` : Spell Crit Chance %
- `MP5` : MP5
- `Sword` : Sword Weapon Skill
- `Axe` : Axe Weapon Skill
- `Mace` : Mace Weapon Skill
- `Polearm` : Polearm Weapon Skill
- `Staff` : Staff Weapon Skill
- `Dagger` : Dagger Weapon Skill
- `Fist` : Fist Weapon Skill
- `WDmg` : Weapons Additional Damage

### FAQ
- When will X class be implemented ?
	- Not a class order but here is a rough roadmap : Popular DPS Melee Specs -> Popular DPS Caster Specs -> GUI -> Alternative DPS Melee and Caster Specs -> Multiple Targets -> Bonus features (multiple players, tanking/healing sim, ...)
- Is X item implemented yet ?
	- Try it yourself and you'll see if it works! The program detects special effects by their item's name, so check for typos if it doesn't. A complete list of supported items will be written, one day.
- Is X set implemented yet ?
	- For now, sets providing stats boost have to be added manually in the `sim.json` file (either along with one of the items or as a Buff). Special set effects are detected by checking pieces names, so write them carefuly. A complete list of supported special sets will be written, one day.


Please report any issue or suggestion either here on [GitHub](https://github.com/Zwyk/ClassicCraft/issues) or on the [Discord](https://discord.gg/tG4q7HE)

You can also PM me on Discord : Zwyk#5555

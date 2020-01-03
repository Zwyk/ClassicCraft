# ClassicCraft
*A WoW Classic simulation tool*

### How to use :
- Download and extract the content of the .zip file anywhere on your computer
- Modify `sim.json` to setup the simulation
	- You can use the examples in the folder `sim.json examples`
- Execute `ClassicCraft.exe`
	- A console should appear and the simulations start
	- Depending on the desired accuracy (`TargerErrorPct` or `NbSim`) and if `StatsWeights` is activated, the process might take some time
- A new .txt file with the logs of the simulation is created in the `Logs` folder, press any key in the console window to close it

### Current features :
- DPS Simulation
	- Character setup
		- Class, Race
		- Gear (some special sets like Rogue's T0 6P and T1 5P)
		- Enchantements
		- Consumables (Crusader)
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
		- Fury (1H or 2H)
	- Druid
		- Feral (Powershifting)
	- Rogue
		- Combat (SS or BS)

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
- `WDmg` : Weapons Additionnal Damage

### FAQ
- When will X class be implemented ?
	- Not a class order but here is a rough roadmap : Popular DPS Melee Specs -> Popular DPS Caster Specs -> Alternative DPS Melee and Caster Specs -> Multiple Targets -> Full Encounter Simulation
- Is X item implemented yet ?
	- Try it yourself and you'll see if it works! The program detects special effects by their item's name, so check for typos if it doesn't. A complete list of supported items will be written, one day.
- Is X set implemented yet ?
	- For now, sets providing stats boost have to be added manually in the `sim.json` file (either along with one of the items or as a Buff). Special set effects are detected by checking pieces names, so write them carefuly. A complete list of supported special sets will be written, one day.


Please report any issue or suggestion on GitHub : https://github.com/Zwyk/ClassicCraft/issues

Join the community : https://discord.gg/tG4q7HE

You can also contact me on Discord : Zwyk#5555

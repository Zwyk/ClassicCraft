# ClassicCraft
*A WoW Classic simulation tool*

### How to use :
- Download and extract the content of the release .zip file anywhere on your computer
- Launch `ClassicCraft.exe`
	- A window will appear, letting you set-up your character (`Player setup` tab) and the simulation (`Simulation setup` tab)
	- You can create items in the `Database` tab if you don't find them.
	- If something doesn't seem to be currently editable using the GUI and/or you'd rather setup things in the json files directly, you can directly edit `player.json` and `sim.json` in the directory. No need to reload the GUI once you're done, just save the files before running the simulation!
	- Click the `Let's go` button on the bottom-right when you're ready!
	- Depending on the desired accuracy (`TargerErrorPct` or `NbSim`) and if `StatsWeights` is activated, the process might take a while. You can follow the progression by looking at the bottom bar!
	- You'll be redirected automatically to a new `Results` tab at the end of the process, where the results will be displayed. A new .txt file with them is also created in the `Logs` folder.

### Current features :
- DPS/TPS Simulation
	- Character setup
		- Class, Race
		- Talents (rotation detection from them)
		- Gear (special sets and trinkets auto detection)
		- Enchantements (Crusader, Poisons, ...)
		- Consumables
		- CDs to use
		- Raid buffs
		- World buffs
	- Encounter setup
		- Boss Level/Armor
		- Length (+ boss low life length)
		- Number of targets (AoE rotations)
		- Is the player tanking ? (activates TPS stats and rage from boss hits)
		- Raid debuffs
	- Statistics
		- Target error for automatic ±X% accurate results at 95% CI
		- Overall DPS/TPS
		- Detailed DPS/TPS and Hit stats for each source (% of overall, Hit %, Crit %, etc.)
		- Stats weights (1 AP = X DPS, 1 AGI = X DPS = X AP, etc.)
- Classes supported
	- Vanilla Classic
		- Druid
			- Feral DPS (with Powershifting)
			- Feral Tank
		- Rogue
			- Combat (SS or BS)
		- Priest
			- Shadow
		- Warlock
			- SM/DS
		- Warrior
			- Furyprot
			- Fury 1H/2H
			- Slam
	- Burning Crusade Classic
		- Warrior
			- Fury
			- Arms 1H/2H

### FAQ
- I have an issue / I have a question / I have a suggestion / I want to help you
	- Either use tickets on GitHub or hit me on Discord, see the links below!
- Is there a Wiki with all the information about the program (mechanics, proc-rates, etc.) ?
	- No, I'm focusing on the program's features right now, but it would be great. Scrap the code or ask me!
- When will X class/thing be implemented ?
	- Not a proper roadmap but here is a rough order of priority (purely personnal and indicative) : Popular DPS Melee Specs > Popular DPS Caster Specs > GUI -> Alternative DPS Melee and Caster Specs -> Extra features (multiple players, tanking/healing sim, ...)
- Is X item's/set's special effect implemented yet ?
	- Either scrap the code for a mention of the item/set, or just try running a simulation with the item/set equipped (its name properly written) and activating "LogFight" ("Full Fight Log" on the GUI) to see if it procs or is being used! For sets, you if you want to test the X parts effect, you'll need to have X items with the set's name in it.


Please report any issue or suggestion either here on [GitHub](https://github.com/Zwyk/ClassicCraft/issues) or on the [Discord](https://discord.gg/tG4q7HE) where you can discuss, hang-out and ask me anything about ClassicCraft!
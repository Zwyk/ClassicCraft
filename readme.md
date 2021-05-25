# ClassicCraft
*A WoW Vanilla and BC Classic simulation tool*

![Showcase GIF](/showcase.gif)

### How to use :
- Download and extract the content of the release .zip file anywhere on your computer
- Launch `ClassicCraft.exe`
	- A window will appear, letting you set-up the simulation in the `Simulation setup` and `Player setup` tabs
		- You save and choose between multiple Simulation and Player files.
		- The `Player setup` tab is under development, you'll have to edit your file `\Config\Player\*yourfile*.json` manually to change the equipped items and buffs.
			- The buffs and raid debuffs providing raw stats (AP, Agility, etc.) have to be filled manually with their attributes, just like items.
			- The buffs and raid debuffs providing special effects (% stats, % dmg, etc.) just have to be informed with their names (e.g. "Blessing of Kings")
			- Check the examples in the folder `\Config\Player\` if you're unsure about how to write some keywords.
		- The `Boss debuffs` tab is under development, you'll have to edit your file `\Config\Sim\*yourfile*.json` manually to change them.
	- Click the `Let's go` button on the bottom-right when you're ready!
	- Depending on the desired accuracy (`TargerErrorPct` or `NbSim`) and if `StatsWeights` is activated, the process might take a while. You can follow the progress by looking at the bottom bar!
	- You'll be redirected automatically to a new `Results` tab at the end of the process, where the results will be displayed. A new .txt file with them is also saved in the `Logs` folder.

### Current features :
- DPS/TPS Simulation
	- Character setup
		- Class, Race
		- Talents (rotation detection)
		- Gear (special sets and trinkets auto detection)
		- Enchantements (Crusader, Mongoose, ...)
		- Consumables
		- CDs to use
		- Raid buffs
		- World buffs
	- Encounter setup
		- Boss Level/Armor/Magic resistances
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
			- Any DPS spec (Fury, Arms, Slam, 21/40/0, 0/31/30, ...)

### FAQ
- When I try to run a simultion there seems to be an error
	- First, make sure your `.json` config files (in `\Config\Player\` and `\Config\Sim\`) don't have any typos or are badly formatted (e.g. missing a comma). Try running a simulation with one of the example json files, and if you're still having issues please copy/paste (or screenshot) the error and open a ticket on [GitHub](https://github.com/Zwyk/ClassicCraft/issues)!
- Is X item's/set's special effect implemented yet ?
	- Either scrap the code for a mention of the item/set, or just try running a simulation with the item/set equipped (__its name properly written__) and activating "Full Fight Log" ("LogFight" in the json) to see if it procs or is being used! For sets, you if you want to test the X parts effect, you'll need to have X items with the set's name in it.
- Is there a Wiki with all the information about the program (mechanics, proc-rates, etc.) ?
	- Not yet, but it would be great. Scrap the code or ask me!
- When will X class/thing be implemented ?
	- Not a proper roadmap but here is a rough order of priority (purely personnal and indicative) : Popular DPS Melee Specs > Popular DPS Caster Specs > GUI > Alternative DPS Specs -> Extra features (multiple players, tanking/healing sim, ...)


For anything else related to ClassicCraft and if you want to make contact with me, join the [Discord](https://discord.gg/tG4q7HE)!
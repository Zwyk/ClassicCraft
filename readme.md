# ClassicCraft
*A WoW Vanilla and BC Classic simulation tool*

### How to use :
- Download and extract the content of the release .zip file anywhere on your computer
- Launch `ClassicCraft.exe`
	- A window will appear, letting you set-up the simulation in the `Simulation setup` and `Player setup` tabs
		- The `Player setup` tab is under development, you'll have to edit the `player.json` file manually to change the equipped items and buffs.
			- The buffs and raid debuffs providing raw stats (AP, Agility, etc.) have to be filled manually with their attributes, just like items.
			- The buffs and raid debuffs providing special effects (% stats, % dmg, etc.) just have to be informed with their names (e.g. "Blessing of Kings")
			- You'll find examples of all the buffs in the `player.json examples` folder!
			- I suggest saving backups of your player.json files manually if you want to keep and switch between your gears.
		- The `Boss debuffs` tab is under development, you'll have to edit the `sim.json` file manually to change them.
		- If you've made manual edits in the json files, you don't need to restart the program : simply click the `load` button before starting the simulation!
	- Click the `Let's go` button on the bottom-right when you're ready!
	- Depending on the desired accuracy (`TargerErrorPct` or `NbSim`) and if `StatsWeights` is activated, the process might take a while. You can follow the progression by looking at the bottom bar!
	- You'll be redirected automatically to a new `Results` tab at the end of the process, where the results will be displayed. A new .txt file with them is also saved in the `Logs` folder.

### Current features :
- DPS/TPS Simulation
	- Character setup
		- Class, Race
		- Talents (rotation detection from them)
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
	- First, make sure your `player.json` and `sim.json` files don't have any typos or are badly formatted (e.g. missing a comma). Try running a simultion with one of the example json files. If you still have an issue, please copy/paste (or screenshot) the error and open a ticket on [GitHub](https://github.com/Zwyk/ClassicCraft/issues)!
- Is X item's/set's special effect implemented yet ?
	- Either scrap the code for a mention of the item/set, or just try running a simulation with the item/set equipped (its name properly written) and activating "LogFight" ("Full Fight Log" on the GUI) to see if it procs or is being used! For sets, you if you want to test the X parts effect, you'll need to have X items with the set's name in it.
- Is there a Wiki with all the information about the program (mechanics, proc-rates, etc.) ?
	- Not yet, but it would be great. Scrap the code or ask me!
- When will X class/thing be implemented ?
	- Not a proper roadmap but here is a rough order of priority (purely personnal and indicative) : Popular DPS Melee Specs > Popular DPS Caster Specs > GUI > Alternative DPS Specs -> Extra features (multiple players, tanking/healing sim, ...)


For anything else related to ClassicCraft and if you want to make contact with me, join the [Discord](https://discord.gg/tG4q7HE)!
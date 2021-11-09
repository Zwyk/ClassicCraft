# ClassicCraft
*A WoW Vanilla and BC Classic simulation tool*

![Showcase GIF](/showcase.gif)

### How to use :
- Download and extract the content of the release .zip file anywhere on your computer
- Launch `ClassicCraft.exe`
	- A window will appear, letting you set-up the simulation in the `Simulation setup` and `Player setup` tabs
		- You can select, create and save Simulation and Player profiles.
		- The `Player equipment` tab is under development, you'll have to edit your file `\Config\Player\*yourfile*.json` manually to change the equipped items (providing its name, stats, enchant and gems).
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
		- Raid buffs/debuffs
		- World buffs
	- Encounter setup
		- Boss Level/Armor/Magic resistances
		- Length (+ boss low life length)
		- Number of targets (AoE rotations)
		- Player tanking the boss (rage generation, on-hit effects)
		- Boss facing the player
	- Statistics
		- Target error for automatic ±X% accurate results at 95% CI
		- Overall DPS/TPS
		- Detailed DPS/TPS and Hit stats for each source (% of overall, Hit %, Crit %, etc.)
		- Stats weights / EP Values (1 AP = X DPS, 1 AGI = X DPS = X AP, etc.)
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
			- Any 1H/2H DPS or Tank spec (Fury, Arms, Slam, Prot, 21/40/0, 0/31/30, ...)

### FAQ
- When I try to run a simualtion there seems to be an error
	- Try searching for corrupted files in `\Config\Player\` and `\Config\Sim\` folders, and resetting them to the example files in the release. If you're still having issues please copy/paste (or screenshot) the error and open a ticket on [GitHub](https://github.com/Zwyk/ClassicCraft/issues)!
- Is X item's/set's special effect implemented yet ?
	- Try running a simulation with the item/set equipped (__its name properly written__) and activating "Full Fight Log" ("LogFight" in the json) to see if it procs or is being used! For sets, you if you want to test the X parts effect, you'll need to have X items with the set's name in it. If it seems to be not implemented, open a ticket.
- Is there a Wiki with all the information about the program (mechanics, proc-rates, etc.) ?
	- Not yet, but it would be great. Scrap the code for now, or ask me!
- When will X class/thing be implemented ?
	- Not a proper roadmap but here is a rough order of priority (purely personnal and indicative) : Popular Melee Specs > GUI > Popular Caster Specs > Alternative DPS Specs > Extra features (multiple players, healing sim, ...)


For anything else related to ClassicCraft and if you want to make contact with me, join the [Discord](https://discord.gg/tG4q7HE)!
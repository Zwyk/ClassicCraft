# ClassicCraft
*A WoW Classic simulation tool*

### How to use :
- Download and extract the content of the release .zip file anywhere on your computer
- Launch `ClassicCraft.exe`
	- A window will appear, letting you set-up your character (`Player setup` tab) and the simulation (`Pimulation setup` tab)
	- You can create items in the `Database` tab if you don't find them.
	- Click the `Let's go` button on the bottom-right when you're ready!
		- Depending on the desired accuracy (`TargerErrorPct` or `NbSim`) and if `StatsWeights` is activated, the process might take some time. You can follow the progression looking the bottom bar.
	- You'll also be redirected automatically to a new `Console` tab, where the output of the simulations will be written. A new .txt file with the logs of the simulation is also created in the `Logs` folder (found in the same directory than ClassicCraft.exe)

### Current features :
- DPS Simulation
	- Character setup
		- Class, Race
		- Talents
		- Gear (special set bonuses are WIP, Rogue T0 6P and T1 5P working)
		- Enchantements (Crusader, Poisons)
		- Consumables
		- CDs to use
		- Raid/World buffs
	- Encounter setup
		- Boss Level/Armor
		- Length (+ boss low life length)
		- Is the player tanking ? (activates TPS stats and rage from boss hits)
	- Statistics output (stats, graphs)
		- Overall DPS/TPS
		- DPS/TPS and Hit stats by spell (Hit, Miss, Crit, Glancing, etc.)
		- Stats weights (1 AP = X DPS, 1 STR = X DPS or X AP, etc.)
- Classes supported
	- Warrior
		- Fury BT (1H or 2H)
	- Druid
		- Feral (Powershifting)
	- Rogue
		- Combat (SS or BS)

### FAQ
- Is there a Wiki with all the information about the program ?
	- No, I'm focusing on the program's features right now, but it would be great. You can ask me anything on Discord though!
- When will X class be implemented ?
	- Not a class order but here is a rough roadmap : Popular DPS Melee Specs -> Popular DPS Caster Specs -> GUI -> Alternative DPS Melee and Caster Specs -> Multiple Targets -> Bonus features (multiple players, tanking/healing sim, ...)
- Is X item's special effect implemented yet ?
	- If it's in the Database when you download the release, it's probably supported. If not, try to create it with its exact name and try it yourself to see if it works!
- Is X set implemented yet ?
	- For now, sets providing stats boost have to be added manually in the `sim.json` file (either along with one of the items or as a Buff). Special set effects are detected by checking pieces names, so write them carefuly. A complete list of supported special sets will be written, one day.


Please report any issue or suggestion either here on [GitHub](https://github.com/Zwyk/ClassicCraft/issues) or on the [Discord](https://discord.gg/tG4q7HE) where you can also come, hang-out and ask me anything about ClassicCraft!
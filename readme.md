# ClassicCraft
*A WoW Classic simulation tool*

## Current features :
- DPS Simulation
	- Character setup
		- Class, Race
		- Gear
			- *Specials : HoJ, Flurry Axe, Thrash Blade*
		- Enchantements
			- *Specials : Crusader*
		- Consumables
		- CDs to use (Racial, Recklessness, etc.)
		- Raid buffs (windfury, etc.)
	- Encounter setup
		- Boss Level/Armor
		- Length (+ boss low life length)
	- Statistics output as raw text
		- Overall DPS
		- DPS and Hit stats by spell (Hit, Miss, Crit, Glancing, etc.)
		- Stats weights (1 AP = X DPS, 1 STR = X DPS or X AP, etc.)
- Classes supported
	- Warrior
		- Bloodthirst 1H / 2H
	- Druid
		- Feral

How to use :
- Download and extract the content of the .zip file anywhere on your computer
- Modify `sim.json` to setup the simulation
	- You can use the examples in the folder `sim.json examples`
- Execute `ClassicCraft.exe`
	- A console should appear and the simulations start
	- Depending on the desired accuracy (`TargerErrorPct` or `NbSim`) and if `StatsWeights` is activated, the process might take some time
- A new .txt file with the logs of the simulation is created in the `Logs` folder, press any key in the console window to close it

Please report any issue or suggestion on GitHub : https://github.com/Zwyk/ClassicCraft/issues

Join the community : https://discord.gg/tG4q7HE

You can also contact me on Discord : Zwyk#5555

# Legendary-Pickaxe
A console-based F# resource management game about mining ores and crafting the Legendary Pickaxe.

Legendary Pickaxe: Requirements Document
	Project Overview
Legendary Pickaxe is a console-based resource management game. The player mines ores, sells ores for money, repairs and upgrades the pickaxe step by step. The goal of the game is to craft the final pickaxe, called the Legendary Pickaxe.
At the start of the game, the game prints a short intro message that mentions the village, the player's goal, and the Legendary Pickaxe.
When the player wins, the game prints an ending message saying that the village is saved by the Legendary Pickaxe.

	Game Objective
The player wins the game by upgrading the current pickaxe to the Legendary Pickaxe. When the player obtains the Legendary Pickaxe, the game prints a victory message and terminates.

	Initial Game State
At the start of the game, the player has the following state.
State	Initial Value
Money	0
Current Pickaxe	Wooden Pickaxe
Current Durability	10
Maximum Durability	10
Mine	Surface Mine
Inventory	0 of every ore

	Ores
The game has six ore types.
Ore	Selling Price
Stone	1
Copper	3
Iron	7
Gold	15
Diamond	40
Mythril	100

Selling price means the amount of money the player receives for selling one unit of the ore.
	Pickaxe Levels
The game contains six pickaxe levels.
Pickaxe level	Maximum Durability	Repair Cost Per Durability	Upgrade Condition
Wooden
Pickaxe	10	1		(basic)
Stone
Pickaxe	15	2	Money 20, Stone 10
Iron
Pickaxe	20	3	Money 60,
Copper 5, Iron 3
Gold
Pickaxe	25	5	Money 150,
Iron 8, Gold 3
Diamond
Pickaxe	30	8	Money 400,
Gold 10, Diamond 3
Legendary
Pickaxe	40	-	Money 1000,
Diamond 5, Mythril 2

When the pickaxe is upgraded, its maximum durability changes to the maximum durability of the new level. When the pickaxe is upgraded, its current durability is restored to the new maximum durability. For example, if the player upgrades from Wooden Pickaxe with durability 3 out of 10 to Stone Pickaxe, the new durability becomes 15 out of 15.
If the player does not have enough required money or ores, the game does not change money, inventory, durability, or pickaxe level, and prints a message explaining that the upgrade requirements are not satisfied.
Durability represents how many times the current pickaxe can still be used for mining. Each successful mining action decreases the current durability by 1. For example, if the player mines once with durability 10 out of 10, the durability becomes 9 out of 10.
If the current durability becomes 0, the pickaxe is considered broken. A broken pickaxe cannot be used for mining. If the player selects Mine while the current durability is 0, the game does not generate any ore, does not change the inventory, and prints a message telling the player to repair the pickaxe.
The player can restore durability by selecting Repair pickaxe from the main menu. Repairing a pickaxe restores the current durability to the maximum durability of the current pickaxe level if the player has enough money to pay the repair cost.

Each pickaxe level has a repair cost per durability point.
repair cost=(maximum durability-current durability)*repair cost per durability
For example, if the player has an Iron Pickaxe with durability 12 out of 20, the repair cost is: (20-12) *3 = 24.
If the current durability is already equal to the maximum durability, the game prints a message saying that the pickaxe is already fully repaired.
If the player does not have enough money to pay the repair cost, the game does not change money or durability and prints a message saying that the player does not have enough money.

The Legendary Pickaxe does not need to be repaired because the game ends immediately when the player obtains it.

When the player selects Upgrade pickaxe, the game checks whether the player has the required money and ores for the next pickaxe level.
If the player has all required resources, the game performs the upgrade.
A successful upgrade must do all of the following.
	Subtract the required money
	Remove the required ores from the inventory
	Change the pickaxe to the next level
	Set current durability to the maximum durability of the new pickaxe level
	Print an upgrade success message
	Mines
The game contains four mines.
Mine	Required Pickaxe Level	Available Ores
Surface Mine	Wooden Pickaxe or higher	Stone, Copper, Iron
Iron Cave	Stone Pickaxe or higher	Stone, Copper, Iron, Gold
Crystal Mine	Iron Pickaxe or higher	Copper, Iron, Gold, Diamond
Ancient Mine	Diamond Pickaxe or higher	Iron, Gold, Diamond, Mythril
The player can select only a mine whose required pickaxe level is less than or equal to the current pickaxe level.
If the player tries to select a locked mine, the game does not change the current mine and prints a message explaining that the mine is locked.
When the player mines successfully, exactly one ore is generated according to the probability table of the current mine.
6-1. Surface Mine
Ore	Probability
Stone	60%
Copper	30%
Iron	10%
6-2. Iron Cave
Ore	Probability
Stone	35%
Copper	30%
Iron	25%
Gold	10%
6-3. Crystal Mine
Ore	Probability
Copper	30%
Iron	35%
Gold	25%
Diamond	10%
6-4. Ancient Mine
Ore	Probability
Iron	35%
Gold	30%
Diamond	25%
Mythril	10%

	Main Menu
On each turn, the game displays the following menu.
1. Mine
2. Choose mine
3. Sell ores
4. Repair pickaxe
5. Upgrade pickaxe 
6. Show status
7. Quit
Commands 1~2, 4~5 follow the rules described in the previous sections. Commands 3, 6 and 7 are described below because they are not covered by the previous resource, mining, repair, and upgrade.

7-3 Sell ores
If the player selects Sell ores, the game displays the following sell menu.

1. Sell a selected ore
2. Sell all ores except ores needed for the next upgrade
3. Cancel
If the player selects Sell a selected ore, the game asks the player to select an ore type and an amount. If the selected amount is greater than 0 and less than or equal to the current count of that ore, the game removes that amount from the inventory and adds money equal to selected amount * selling price of the selected ore.
If the selected amount is invalid or the player does not have enough of that ore, the game does not change the inventory or money and prints an error message.
If the player selects Sell all ores except ores needed for the next upgrade, the game keeps the ores required for the next upgrade and sells only the extra ores. For example, if the next upgrade requires Iron 8 and the player has Iron 12, the game keeps 8 Iron and sells 4 Iron.
If the player selects Cancel, the game returns to the main menu without changing the game state.

7-6. Show status
The status output must include all of the following information:
1. Current money.
2. Current pickaxe level.
3. Current durability and maximum durability.
4. Current mine.
5. Inventory count for every ore type. 
Show status does not change money, durability, current mine, inventory, or pickaxe level.

7-7. Quit 
If the player selects Quit, the game prints a goodbye message and terminates.
After the Quit command is selected, the game must not display the main menu again.

	Victory Condition
The player wins immediately after successfully upgrading from Diamond Pickaxe to Legendary Pickaxe.
When the player wins, the game must print a message containing the following phrase.
“You crafted the Legendary Pickaxe!”
After printing the victory message, the game terminates.


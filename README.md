# Legendary Pickaxe

Legendary Pickaxe is a console-based resource management game written in F# for .NET 10.
The player mines ores, sells resources, repairs and upgrades pickaxes, and wins by crafting the Legendary Pickaxe.

The game begins with a weak Wooden Pickaxe and no money. On each turn, the player chooses an action from the menu: mine for ores, move to another mine, sell ores, repair the pickaxe, upgrade the pickaxe, check the current status, or quit. Mining gives one ore based on the current mine's probability table and reduces pickaxe durability by 1. Ores can be sold for money, and money plus specific ores are used to upgrade the pickaxe.

As the pickaxe becomes stronger, new mines become available with better ores. The final objective is to upgrade from the Diamond Pickaxe to the Legendary Pickaxe. When this happens, the game prints the victory message and ends.

## Requirements

- .NET SDK 10
- A terminal that can run `dotnet`

## How to Run

From this directory:

```powershell
dotnet run
```

The game starts with a main menu:

1. Mine
2. Choose mine
3. Sell ores
4. Repair pickaxe
5. Upgrade pickaxe
6. Show status
7. Quit

Enter the number of the action you want to perform.
Each turn also displays the current status, including money, current pickaxe, durability, current mine, and inventory. When an upgrade succeeds, the new pickaxe image opens with the system image viewer.

## Notes on Requirements

This implementation follows the submitted requirements document for Legendary Pickaxe.
No requirement changes were made.
Opening a pickaxe image after a successful upgrade was added as additional visual feedback, but it does not change any game rules.

## LLM Usage

I used an LLM to help draft parts of this README, create the pickaxe images, write the F# code that opens the pickaxe image file after a successful upgrade, and review whether that image-opening code should work on macOS.

After finishing the implementation, I also used the LLM to compare the code against the requirements PDF and check for possible mismatches in the initial state, ore prices, pickaxe upgrades, durability, repair costs, mine unlock rules, selling behavior, and victory condition.

I manually reviewed the LLM's suggestions and rechecked the implementation against the PDF myself. One limitation was that the LLM repeatedly claimed there was an error in the ore-selling logic even though the implementation was correct, so I had to inspect that part manually and verify that selling selected ores and selling extra ores behaved as required.

open System
open System.Diagnostics
open System.IO

type Ore =
    | Stone
    | Copper
    | Iron
    | Gold
    | Diamond
    | Mythril

type PickaxeLevel =
    | WoodenPickaxe
    | StonePickaxe
    | IronPickaxe
    | GoldPickaxe
    | DiamondPickaxe
    | LegendaryPickaxe

type Mine =
    | SurfaceMine
    | IronCave
    | CrystalMine
    | AncientMine

type PickaxeInfo =
    { Name: string
      MaxDurability: int
      RepairCostPerDurability: int option }

type UpgradeRequirement =
    { Money: int
      Ores: Map<Ore, int> }

type MineInfo =
    { Name: string
      RequiredPickaxe: PickaxeLevel
      Drops: (Ore * int) list }

type GameState =
    { Money: int
      Pickaxe: PickaxeLevel
      Durability: int
      Mine: Mine
      Inventory: Map<Ore, int> }

let allOres = [ Stone; Copper; Iron; Gold; Diamond; Mythril ]
let allMines = [ SurfaceMine; IronCave; CrystalMine; AncientMine ]

let oreName ore =
    match ore with
    | Stone -> "Stone"
    | Copper -> "Copper"
    | Iron -> "Iron"
    | Gold -> "Gold"
    | Diamond -> "Diamond"
    | Mythril -> "Mythril"

let orePrice ore =
    match ore with
    | Stone -> 1
    | Copper -> 3
    | Iron -> 7
    | Gold -> 15
    | Diamond -> 40
    | Mythril -> 100

let pickaxeInfo pickaxe =
    match pickaxe with
    | WoodenPickaxe ->
        { Name = "Wooden Pickaxe"
          MaxDurability = 10
          RepairCostPerDurability = Some 1 }
    | StonePickaxe ->
        { Name = "Stone Pickaxe"
          MaxDurability = 15
          RepairCostPerDurability = Some 2 }
    | IronPickaxe ->
        { Name = "Iron Pickaxe"
          MaxDurability = 20
          RepairCostPerDurability = Some 3 }
    | GoldPickaxe ->
        { Name = "Gold Pickaxe"
          MaxDurability = 25
          RepairCostPerDurability = Some 5 }
    | DiamondPickaxe ->
        { Name = "Diamond Pickaxe"
          MaxDurability = 30
          RepairCostPerDurability = Some 8 }
    | LegendaryPickaxe ->
        { Name = "Legendary Pickaxe"
          MaxDurability = 40
          RepairCostPerDurability = None }

let pickaxeRank pickaxe =
    match pickaxe with
    | WoodenPickaxe -> 0
    | StonePickaxe -> 1
    | IronPickaxe -> 2
    | GoldPickaxe -> 3
    | DiamondPickaxe -> 4
    | LegendaryPickaxe -> 5

let canUseMine currentPickaxe requiredPickaxe =
    pickaxeRank currentPickaxe >= pickaxeRank requiredPickaxe

let mineInfo mine =
    match mine with
    | SurfaceMine ->
        { Name = "Surface Mine"
          RequiredPickaxe = WoodenPickaxe
          Drops = [ Stone, 60; Copper, 30; Iron, 10 ] }
    | IronCave ->
        { Name = "Iron Cave"
          RequiredPickaxe = StonePickaxe
          Drops = [ Stone, 35; Copper, 30; Iron, 25; Gold, 10 ] }
    | CrystalMine ->
        { Name = "Crystal Mine"
          RequiredPickaxe = IronPickaxe
          Drops = [ Copper, 30; Iron, 35; Gold, 25; Diamond, 10 ] }
    | AncientMine ->
        { Name = "Ancient Mine"
          RequiredPickaxe = DiamondPickaxe
          Drops = [ Iron, 35; Gold, 30; Diamond, 25; Mythril, 10 ] }

let nextUpgrade pickaxe =
    match pickaxe with
    | WoodenPickaxe ->
        Some(
            StonePickaxe,
            { Money = 20
              Ores = Map.ofList [ Stone, 10 ] }
        )
    | StonePickaxe ->
        Some(
            IronPickaxe,
            { Money = 60
              Ores = Map.ofList [ Copper, 5; Iron, 3 ] }
        )
    | IronPickaxe ->
        Some(
            GoldPickaxe,
            { Money = 150
              Ores = Map.ofList [ Iron, 8; Gold, 3 ] }
        )
    | GoldPickaxe ->
        Some(
            DiamondPickaxe,
            { Money = 400
              Ores = Map.ofList [ Gold, 10; Diamond, 3 ] }
        )
    | DiamondPickaxe ->
        Some(
            LegendaryPickaxe,
            { Money = 1000
              Ores = Map.ofList [ Diamond, 5; Mythril, 2 ] }
        )
    | LegendaryPickaxe -> None

let emptyInventory =
    allOres |> List.map (fun ore -> ore, 0) |> Map.ofList

let initialState =
    { Money = 0
      Pickaxe = WoodenPickaxe
      Durability = 10
      Mine = SurfaceMine
      Inventory = emptyInventory }

let oreCount ore inventory =
    inventory |> Map.tryFind ore |> Option.defaultValue 0

let setOreCount ore count inventory =
    inventory |> Map.add ore count

let addOre ore amount inventory =
    setOreCount ore (oreCount ore inventory + amount) inventory

let removeOre ore amount inventory =
    setOreCount ore (oreCount ore inventory - amount) inventory

let readTrimmedLine () =
    match Console.ReadLine() with
    | null -> None
    | text -> Some(text.Trim())

let tryParseInt (text: string) =
    match Int32.TryParse text with
    | true, value -> Some value
    | false, _ -> None

let readNumber prompt =
    printf "%s" prompt
    readTrimmedLine () |> Option.bind tryParseInt

let printIntro () =
    printfn "Welcome to the mining village."
    printfn "The village needs your help: mine ores, earn money, repair and upgrade your pickaxe."
    printfn "Your goal is to craft the Legendary Pickaxe and save the village."

let pickaxeImageFileName pickaxe =
    match pickaxe with
    | WoodenPickaxe -> "Wooden Pickaxe.png"
    | StonePickaxe -> "Stone Pickaxe.png"
    | IronPickaxe -> "Iron Pickaxe.png"
    | GoldPickaxe -> "Gold Pickaxe.png"
    | DiamondPickaxe -> "Diamond Pickaxe.png"
    | LegendaryPickaxe -> "Legendary Pickaxe.png"

let tryFindPickaxeImage pickaxe =
    let fileName = pickaxeImageFileName pickaxe

    [ Path.Combine(AppContext.BaseDirectory, "image", fileName)
      Path.Combine(Directory.GetCurrentDirectory(), "image", fileName) ]
    |> List.tryFind File.Exists

let showPickaxeImage pickaxe =
    match tryFindPickaxeImage pickaxe with
    | Some path ->
        try
            let startInfo =
                ProcessStartInfo(
                    FileName = path,
                    UseShellExecute = true
                )

            Process.Start(startInfo) |> ignore
        with ex ->
            printfn "Could not open the pickaxe image: %s" ex.Message
    | None ->
        printfn "Could not find the pickaxe image file for %s." (pickaxeInfo pickaxe).Name

let newlyUnlockedMines previousPickaxe nextPickaxe =
    let previousRank = pickaxeRank previousPickaxe
    let nextRank = pickaxeRank nextPickaxe

    allMines
    |> List.filter (fun mine ->
        let requiredRank = pickaxeRank (mineInfo mine).RequiredPickaxe
        requiredRank > previousRank && requiredRank <= nextRank)

let printUpgradeSuccess previousPickaxe nextPickaxe =
    let nextInfo = pickaxeInfo nextPickaxe
    printfn "You obtained the %s." nextInfo.Name

    match newlyUnlockedMines previousPickaxe nextPickaxe with
    | [] -> ()
    | unlockedMines ->
        let mineNames =
            unlockedMines
            |> List.map (fun mine -> (mineInfo mine).Name)
            |> String.concat ", "

        printfn "You can now enter %s." mineNames

let durabilityBar current maximum =
    let width = 20
    let filled =
        if maximum <= 0 then
            0
        else
            current * width / maximum

    let safeFilled = max 0 (min width filled)
    let empty = width - safeFilled
    String.replicate safeFilled "#" + String.replicate empty "."

let inventorySummary inventory =
    allOres
    |> List.map (fun ore -> sprintf "%s:%d" (oreName ore) (oreCount ore inventory))
    |> String.concat " | "

let printCurrentState state =
    let pickaxe = pickaxeInfo state.Pickaxe
    let mine = mineInfo state.Mine
    let bar = durabilityBar state.Durability pickaxe.MaxDurability

    printfn ""
    printfn "================ Current Status ================"
    printfn "Money: %d" state.Money
    printfn "Pickaxe: %s" pickaxe.Name
    printfn "Durability: [%s] %d / %d" bar state.Durability pickaxe.MaxDurability
    printfn "Current mine: %s" mine.Name
    printfn "Inventory: %s" (inventorySummary state.Inventory)
    printfn "================================================"

let printMainMenu () =
    printfn ""
    printfn "Main Menu"
    printfn "1. Mine"
    printfn "2. Choose mine"
    printfn "3. Sell ores"
    printfn "4. Repair pickaxe"
    printfn "5. Upgrade pickaxe"
    printfn "6. Show status"
    printfn "7. Quit"

let printInventory inventory =
    for ore in allOres do
        printfn "- %s: %d" (oreName ore) (oreCount ore inventory)

let printStatus state =
    let pickaxe = pickaxeInfo state.Pickaxe
    let mine = mineInfo state.Mine

    printfn ""
    printfn "Status"
    printfn "Money: %d" state.Money
    printfn "Current pickaxe: %s" pickaxe.Name
    printfn "Durability: %d / %d" state.Durability pickaxe.MaxDurability
    printfn "Current mine: %s" mine.Name
    printfn "Inventory:"
    printInventory state.Inventory

let chooseRandomOre (rng: Random) mine =
    let roll = rng.Next(100)

    let rec choose cumulative drops =
        match drops with
        | [] -> failwith "Mine drop table must contain at least one ore."
        | [ ore, _ ] -> ore
        | (ore, probability) :: rest ->
            let nextCumulative = cumulative + probability
            if roll < nextCumulative then ore else choose nextCumulative rest

    choose 0 (mineInfo mine).Drops

let mineOre rng state =
    if state.Durability <= 0 then
        printfn "Your pickaxe is broken. Repair the pickaxe before mining."
        state
    else
        let ore = chooseRandomOre rng state.Mine
        let updatedDurability = state.Durability - 1
        let updatedInventory = addOre ore 1 state.Inventory

        printfn "You mined 1 %s." (oreName ore)

        if updatedDurability = 0 then
            printfn "Your pickaxe is now broken."

        { state with
            Durability = updatedDurability
            Inventory = updatedInventory }

let printMineChoices state =
    printfn ""
    printfn "Choose Mine"

    allMines
    |> List.iteri (fun index mine ->
        let info = mineInfo mine
        let required = pickaxeInfo info.RequiredPickaxe
        let status =
            if canUseMine state.Pickaxe info.RequiredPickaxe then
                "available"
            else
                sprintf "locked, requires %s" required.Name

        printfn "%d. %s (%s)" (index + 1) info.Name status)

let chooseMine state =
    printMineChoices state

    match readNumber "Select a mine: " with
    | Some choice when choice >= 1 && choice <= allMines.Length ->
        let selectedMine = allMines[choice - 1]
        let info = mineInfo selectedMine

        if canUseMine state.Pickaxe info.RequiredPickaxe then
            printfn "You moved to %s." info.Name
            { state with Mine = selectedMine }
        else
            let required = pickaxeInfo info.RequiredPickaxe
            printfn "%s is locked. You need at least %s." info.Name required.Name
            state
    | Some _ ->
        printfn "Invalid mine selection."
        state
    | None ->
        printfn "No mine selected."
        state

let printOreChoices state =
    printfn ""
    printfn "Choose Ore"

    allOres
    |> List.iteri (fun index ore ->
        printfn
            "%d. %s (owned: %d, price: %d)"
            (index + 1)
            (oreName ore)
            (oreCount ore state.Inventory)
            (orePrice ore))

let sellSelectedOre state =
    printOreChoices state

    match readNumber "Select an ore: " with
    | Some choice when choice >= 1 && choice <= allOres.Length ->
        let selectedOre = allOres[choice - 1]

        match readNumber "Amount to sell: " with
        | Some amount when amount > 0 && amount <= oreCount selectedOre state.Inventory ->
            let gainedMoney = amount * orePrice selectedOre
            printfn "Sold %d %s for %d money." amount (oreName selectedOre) gainedMoney

            { state with
                Money = state.Money + gainedMoney
                Inventory = removeOre selectedOre amount state.Inventory }
        | Some _ ->
            printfn "Invalid amount or not enough ore."
            state
        | None ->
            printfn "No amount entered."
            state
    | Some _ ->
        printfn "Invalid ore selection."
        state
    | None ->
        printfn "No ore selected."
        state

let requiredOresForNextUpgrade state =
    match nextUpgrade state.Pickaxe with
    | Some(_, requirement) -> requirement.Ores
    | None -> Map.empty

let sellAllExtraOres state =
    let requiredOres = requiredOresForNextUpgrade state

    let updatedInventory, gainedMoney, soldLines =
        allOres
        |> List.fold
            (fun (inventory, money, lines) ore ->
                let currentCount = oreCount ore inventory
                let keepCount =
                    requiredOres
                    |> Map.tryFind ore
                    |> Option.defaultValue 0
                    |> min currentCount

                let sellCount = currentCount - keepCount

                if sellCount > 0 then
                    let oreMoney = sellCount * orePrice ore
                    let line = sprintf "%d %s for %d money" sellCount (oreName ore) oreMoney
                    setOreCount ore keepCount inventory, money + oreMoney, line :: lines
                else
                    inventory, money, lines)
            (state.Inventory, 0, [])

    if gainedMoney = 0 then
        printfn "There are no extra ores to sell."
        state
    else
        printfn "Sold extra ores:"

        soldLines
        |> List.rev
        |> List.iter (printfn "- %s")

        printfn "Total earned: %d money." gainedMoney

        { state with
            Money = state.Money + gainedMoney
            Inventory = updatedInventory }

let sellOres state =
    printfn ""
    printfn "Sell Ores"
    printfn "1. Sell a selected ore"
    printfn "2. Sell all ores except ores needed for the next upgrade"
    printfn "3. Cancel"

    match readNumber "Choose a sell option: " with
    | Some 1 -> sellSelectedOre state
    | Some 2 -> sellAllExtraOres state
    | Some 3 ->
        printfn "Canceled selling ores."
        state
    | Some _ ->
        printfn "Invalid sell option."
        state
    | None ->
        printfn "No sell option selected."
        state

let repairPickaxe state =
    let info = pickaxeInfo state.Pickaxe

    match info.RepairCostPerDurability with
    | None ->
        printfn "The Legendary Pickaxe does not need repairs."
        state
    | Some repairCostPerDurability ->
        if state.Durability >= info.MaxDurability then
            printfn "Your pickaxe is already fully repaired."
            state
        else
            let missingDurability = info.MaxDurability - state.Durability
            let repairCost = missingDurability * repairCostPerDurability

            if state.Money < repairCost then
                printfn "Repair costs %d money, but you only have %d." repairCost state.Money
                state
            else
                printfn "Repaired %s for %d money." info.Name repairCost

                { state with
                    Money = state.Money - repairCost
                    Durability = info.MaxDurability }

let missingUpgradeRequirements (state: GameState) (requirement: UpgradeRequirement) =
    let moneyMissing =
        if state.Money < requirement.Money then
            [ sprintf "money %d/%d" state.Money requirement.Money ]
        else
            []

    let oreMissing =
        requirement.Ores
        |> Map.toList
        |> List.choose (fun (ore, requiredCount) ->
            let currentCount = oreCount ore state.Inventory

            if currentCount < requiredCount then
                Some(sprintf "%s %d/%d" (oreName ore) currentCount requiredCount)
            else
                None)

    moneyMissing @ oreMissing

let spendUpgradeRequirements (state: GameState) (requirement: UpgradeRequirement) =
    let inventory =
        requirement.Ores
        |> Map.fold (fun inventory ore requiredCount -> removeOre ore requiredCount inventory) state.Inventory

    { state with
        Money = state.Money - requirement.Money
        Inventory = inventory }

let upgradePickaxe state =
    match nextUpgrade state.Pickaxe with
    | None ->
        printfn "You already have the Legendary Pickaxe."
        state, true
    | Some(nextPickaxe, requirement) ->
        let missing = missingUpgradeRequirements state requirement

        if not missing.IsEmpty then
            printfn "Upgrade requirements are not satisfied:"
            missing |> List.iter (printfn "- %s")
            state, true
        else
            let nextInfo = pickaxeInfo nextPickaxe

            let upgradedState =
                { spendUpgradeRequirements state requirement with
                    Pickaxe = nextPickaxe
                    Durability = nextInfo.MaxDurability }

            printUpgradeSuccess state.Pickaxe nextPickaxe
            showPickaxeImage nextPickaxe

            if nextPickaxe = LegendaryPickaxe then
                printfn "You crafted the Legendary Pickaxe!"
                printfn "The village is saved by the Legendary Pickaxe."
                upgradedState, false
            else
                upgradedState, true

let rec gameLoop rng state =
    printCurrentState state
    printMainMenu ()

    match readNumber "Choose an action: " with
    | Some 1 ->
        state
        |> mineOre rng
        |> gameLoop rng
    | Some 2 ->
        state
        |> chooseMine
        |> gameLoop rng
    | Some 3 ->
        state
        |> sellOres
        |> gameLoop rng
    | Some 4 ->
        state
        |> repairPickaxe
        |> gameLoop rng
    | Some 5 ->
        let nextState, shouldContinue = upgradePickaxe state
        if shouldContinue then gameLoop rng nextState
    | Some 6 ->
        printStatus state
        gameLoop rng state
    | Some 7 -> printfn "Goodbye. Thanks for playing Legendary Pickaxe."
    | _ ->
        printfn "Invalid command."
        gameLoop rng state

[<EntryPoint>]
let main _ =
    printIntro ()
    gameLoop (Random()) initialState
    0

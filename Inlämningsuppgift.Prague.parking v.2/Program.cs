using Data;
using Spectre.Console;
const int Capacity = 100;
FileManager fileManager = new();
PriceList prices = PriceListManager.LoadPrices();
ParkingGarage garage = fileManager.LoadGarage() ?? new ParkingGarage(100);

if (garage.Spots.Count == 0)
{
    garage = new ParkingGarage(100);
}
bool run = true;
while (run)
{
    int mainChoice = MainMenu();

    switch (mainChoice)
    {
        case 1:
            SearchVehicle();
            break;

        case 2:
            Parkingspots();
            break;

        case 3:
            ParkVehicle();
            break;

        case 4:
            run = ExitProgram();
            break;
    }

    if (run)
        Pause();
}

#region METODER

int MainMenu()
{
    Console.Clear();

    AnsiConsole.Write(new FigletText("Prague Parking").Color(Color.Yellow));
    AnsiConsole.MarkupLine("[grey]Prague Parking v2[/]");
    AnsiConsole.Write(new Panel($"Car: {prices.CarPrice} Kč/h\nMC: {prices.McPrice} Kč/h").Header("Price List").Border(BoxBorder.Rounded));

    Console.WriteLine();

    string choice =
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("[yellow]Main Menu[/]")
            .AddChoices(
                "Search",
                "Garage",
                "Park vehicle",
                "Exit"));

    return choice switch
    {
        "Search" => 1,
        "Garage" => 2,
        "Park vehicle" => 3,
        "Exit" => 4,
        _ => 4
    };
}




void TypeMenychoice(string text, bool isChosen)
{
    if (isChosen)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"> {text}");
        Console.ResetColor();
    }
    else
    {
        Console.WriteLine($"  {text}");
    }
}
void ParkVehicle()
{
    string vehicleType =
        AnsiConsole.Prompt(new SelectionPrompt<string>().Title("[yellow]Choose vehicle type[/]")
            .AddChoices("CAR", "MC"));

    string regnumber =
        AskRegnumber();

    ParkInGarage(
        vehicleType,
        regnumber);
}

void ParkInGarage(string vehicleType, string regnumber)
{
    Vehicle vehicle;

    if (vehicleType == "MC")
        vehicle = new MC(regnumber);
    else
        vehicle = new Car(regnumber);

    if (garage.ParkVehicle(vehicle))
    {
        fileManager.SaveGarage(garage);

        AnsiConsole.Write(new Panel($"{vehicleType} {regnumber} parked").Header("Parking Successful").Border(BoxBorder.Double));

    }
    else
    {
        ShowError("No free parking spot");
    }
}
void SearchVehicle()
{
    Console.Clear();

    string regnumber = AskRegnumber();

    var spot = garage.FindVehicleSpot(regnumber);

    if (spot == null)
    {
        ShowError("Vehicle not found");
        return;
    }

    AnsiConsole.Write(new Panel($"Vehicle is parked at spot {spot.SpotNumber}").Header("Search Result").Border(BoxBorder.Rounded));
}
string AskRegnumber()
{
    while (true)
    {
        string regnumber =
            AnsiConsole.Ask<string>("[yellow]Enter reg nr:[/]").Trim().ToUpperInvariant();

        if (ValidRegnumber(regnumber))
            return regnumber;

        ShowError(
            "Invalid regnumber. Use letters and digits only.");
    }
}
bool ValidRegnumber(string regnumber)
{
    if (string.IsNullOrWhiteSpace(regnumber))
        return false;

    foreach (char c in regnumber)
    {
        if (!char.IsLetterOrDigit(c))
            return false;
    }

    return true;
}
void Pause()
{
    AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
    Console.ReadKey(true);
}

bool ExitProgram()
{
    Console.Clear();
    AnsiConsole.Markup("[yellow]Exit? (Y/N)[/]");

    return Console.ReadKey(true).Key != ConsoleKey.Y;
}

void ShowSuccess(string message)
{
    AnsiConsole.MarkupLine($"[green]{message}[/]");
}
void ShowError(string message)
{
    AnsiConsole.MarkupLine($"[red]{message}[/]");
}
void Parkingspots()
{
    const int columns = 10;
    int selectedSpot = 0;

    while (true)
    {
        Console.Clear();

        AnsiConsole.Write(new Rule("Garage Overview").RuleStyle("yellow"));
        AnsiConsole.MarkupLine(
            "[green][[Empty]][/]" +
            " [yellow][[One MC]][/]" +
            " [red][[Full]][/]");

        DrawInteractiveGarage(selectedSpot, -1);

        Console.WriteLine();
        AnsiConsole.Write(new Panel($"Selected spot: {selectedSpot + 1}").Header("Garage").Border(BoxBorder.Rounded));

        var vehicles = GetVehiclesAtSpot(selectedSpot);

        if (vehicles.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]Empty[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Vehicles at spot:[/]");

            foreach (var v in vehicles)
            {
                AnsiConsole.MarkupLine($"[green]{v.GetType().Name}[/] {v.RegistrationNumber}");
            }
        }

        ConsoleKey key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.LeftArrow:
                if (selectedSpot % columns > 0)
                    selectedSpot--;
                break;

            case ConsoleKey.RightArrow:
                if (selectedSpot % columns < columns - 1 && selectedSpot + 1 < Capacity) selectedSpot++;
                break;

            case ConsoleKey.UpArrow:
                if (selectedSpot >= columns)
                    selectedSpot -= columns;
                break;

            case ConsoleKey.DownArrow:
                if (selectedSpot + columns < Capacity)
                    selectedSpot += columns;
                break;

            case ConsoleKey.Enter:

                if (vehicles.Count == 0)
                    break;

                var chosen = SelectVehicleInSpot(selectedSpot);

                if (chosen == null)
                    break;

                int action = 1;

                while (true)
                {
                    Console.Clear();

                    AnsiConsole.Write(new Rule($"{chosen.GetType().Name} {chosen.RegistrationNumber}").RuleStyle("green"));

                    TypeMenychoice("Move vehicle", action == 1);
                    TypeMenychoice("Remove vehicle", action == 2);
                    TypeMenychoice("Cancel", action == 3);

                    var b = Console.ReadKey(true);

                    if (b.Key == ConsoleKey.DownArrow &&
                        action < 3)
                    {
                        action++;
                    }
                    else if (b.Key == ConsoleKey.UpArrow &&
                             action > 1)
                    {
                        action--;
                    }
                    else if (b.Key == ConsoleKey.Enter)
                    {
                        if (action == 1)
                        {
                            string vehicleType = chosen is MC ? "MC" : "CAR";
                            int target = SelectTargetSpot(selectedSpot, vehicleType);

                            if (target != -1)
                            {
                                garage.MoveVehicle(chosen.RegistrationNumber, target + 1);
                                fileManager.SaveGarage(garage);
                                ShowSuccess($"{chosen.RegistrationNumber} moved to spot {target + 1}.");
                                Pause();
                            }

                            break;
                        }

                        else if (action == 2)
                        {
                            decimal price = garage.CalculatePrice(chosen, prices);
                            garage.RemoveVehicle(chosen.RegistrationNumber);
                            fileManager.SaveGarage(garage);
                            AnsiConsole.Write(new Panel($"Vehicle: {chosen.RegistrationNumber}\nFee: {price} Kč").Header("Checkout").Border(BoxBorder.Double));
                            Pause();

                            break;
                        }

                        else
                        {
                            break;
                        }
                    }
                    else if (b.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }

                break;

            case ConsoleKey.Escape:
                return;
        }
    }
}

int SelectTargetSpot(int fromSpot, string vehicleType)
{
    const int columns = 10;
    int selectedSpot = fromSpot;

    while (true)
    {
        Console.Clear();
        AnsiConsole.Write(new Rule("Garage Overview").RuleStyle("yellow"));
        AnsiConsole.Write(new Panel($"Vehicle: {vehicleType}\nCurrent spot: {fromSpot + 1}").Header("Move Vehicle"));
        AnsiConsole.MarkupLine(
            "[green][[Empty]][/]" +
            " [yellow][[One MC]][/]" +
            " [red][[Full]][/]");

        DrawInteractiveGarage(selectedSpot, fromSpot);
        Console.WriteLine();
        AnsiConsole.MarkupLine($"[yellow]Selected spot:[/] {selectedSpot + 1}");

        if (selectedSpot == fromSpot)
            ShowError("Choose a different spot.");
        else if (!CanParkAtSpot(selectedSpot, vehicleType))
            ShowError("This spot does not have room for the vehicle.");
        else
            ShowSuccess("Available. Press Enter to move here.");


        ConsoleKey key = Console.ReadKey(true).Key;

        switch (key)
        {
            case ConsoleKey.LeftArrow:

                if (selectedSpot % columns > 0)
                    selectedSpot--;
                break;

            case ConsoleKey.RightArrow:

                if (selectedSpot % columns < columns - 1 && selectedSpot + 1 < Capacity)
                    selectedSpot++;
                break;

            case ConsoleKey.UpArrow:

                if (selectedSpot >= columns)
                    selectedSpot -= columns;
                break;

            case ConsoleKey.DownArrow:

                if (selectedSpot + columns < Capacity)
                    selectedSpot += columns;
                break;

            case ConsoleKey.Enter:
                if (selectedSpot != fromSpot && CanParkAtSpot(selectedSpot, vehicleType))
                    return selectedSpot;
                break;

            case ConsoleKey.Escape:

                return -1;
        }
    }
}
void DrawInteractiveGarage(int selectedSpot, int fromSpot)
{
    const int columns = 10;

    for (int i = 0; i < Capacity; i++)
    {
        int used = UsedSize(i);
        Console.ForegroundColor = used == 0 ? ConsoleColor.Green
                                : used == 1 ? ConsoleColor.Yellow
                                : ConsoleColor.Red;

        if (i == selectedSpot)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($">{i + 1,3}< ");
        }
        else if (i == fromSpot)
        {
            Console.Write($"*{i + 1,3}* ");
        }
        else
        {
            Console.Write($"[{i + 1,3}] ");
        }

        Console.ResetColor();

        if ((i + 1) % columns == 0)
            Console.WriteLine();
    }
}
Vehicle? SelectVehicleInSpot(int spot)
{
    var vehicles = garage.Spots[spot].Vehicles;

    if (!vehicles.Any())
        return null;

    int idx = 0;

    while (true)
    {
        Console.Clear();

        AnsiConsole.Write(new Rule($"Spot {spot + 1}").RuleStyle("green"));

        for (int i = 0; i < vehicles.Count; i++)
        {
            TypeMenychoice(
                $"{vehicles[i].GetType().Name} {vehicles[i].RegistrationNumber}",
                idx == i);
        }

        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.DownArrow &&
            idx < vehicles.Count - 1)
        {
            idx++;
        }
        else if (key == ConsoleKey.UpArrow && idx > 0)
        {
            idx--;
        }
        else if (key == ConsoleKey.Enter)
        {
            return vehicles[idx];
        }
        else if (key == ConsoleKey.Escape)
        {
            return null;
        }
    }
}
List<Vehicle> GetVehiclesAtSpot(int spot)
{
    return garage.Spots[spot].Vehicles;
}
int UsedSize(int spot)
{
    return garage.Spots[spot]
        .Vehicles
        .Sum(v => v.Size);
}
bool CanParkAtSpot(
    int spot,
    string vehicleType)
{
    Vehicle vehicle =
        vehicleType == "MC"
        ? new MC("TEMP")
        : new Car("TEMP");

    return garage.Spots[spot].CanPark(vehicle);
}
#endregion
using Data;
FileManager fileManager = new();

ParkingGarage garage =
    fileManager.LoadGarage()
    ?? new ParkingGarage(100);
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
            ShowGarage();
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
    int chosemenu = 1;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("= Main menu =");
        TypeMenychoice("Search", chosemenu == 1);
        TypeMenychoice("Garage", chosemenu == 2);
        TypeMenychoice("Park vehicle", chosemenu == 3);
        TypeMenychoice("Exit", chosemenu == 4);

        ConsoleKeyInfo button = Console.ReadKey(true);

        if (button.Key == ConsoleKey.DownArrow && chosemenu < 4)
            chosemenu++;
        else if (button.Key == ConsoleKey.UpArrow && chosemenu > 1)
            chosemenu--;
        else if (button.Key == ConsoleKey.Enter)
            return chosemenu;
    }
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
    Console.Clear();

    Console.Write("Vehicle type (CAR/MC): ");
    string type = Console.ReadLine()!
        .Trim()
        .ToUpper();

    string regnumber = AskRegnumber();

    ParkInGarage(type, regnumber);
}
void ParkInGarage(
    string vehicleType,
    string regnumber)
{
    Vehicle vehicle;

    if (vehicleType == "MC")
        vehicle = new MC(regnumber);
    else
        vehicle = new Car(regnumber);

    if (garage.ParkVehicle(vehicle))
    {
        fileManager.SaveGarage(garage);

        ShowSuccess(
            $"{vehicleType} {regnumber} parked");
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

    ShowSuccess(
        $"Vehicle is parked at spot {spot.SpotNumber}");
}
string AskRegnumber()
{
    while (true)
    {
        Console.Write("Enter reg nr: ");

        string regnumber = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

        if (ValidRegnumber(regnumber))
            return regnumber;

        ShowError("Invalid regnumber. Use letters and digits only.");
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
    Console.WriteLine("\nPress any key to return to main menu...");
    Console.ReadKey(true);
}
bool ExitProgram()
{
    Console.Clear();
    Console.WriteLine("Exit? (Y/N)");

    return Console.ReadKey(true).Key != ConsoleKey.Y;
}

void ShowError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(message);
    Console.ResetColor();
}
void ShowSuccess(string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(message);
    Console.ResetColor();

}

void ShowGarage()
{
    foreach (var spot in garage.Spots)
    {
        Console.Write($"ParkingSpot: {spot.SpotNumber}: ");

        if (!spot.Vehicles.Any())
        {
            Console.WriteLine("Empty");
            continue;
        }

        foreach (var vehicle in spot.Vehicles)
        {
            Console.Write(
                $"{vehicle.RegistrationNumber} ");
        }

        Console.WriteLine();
    }
}
void MoveVehicle()
{
    string regnumber = AskRegnumber();

    Console.Write("New spot: ");

    if (!int.TryParse(
        Console.ReadLine(),
        out int target))
    {
        ShowError("Invalid spot.");
        return;
    }

    if (garage.MoveVehicle(regnumber, target))
    {
        fileManager.SaveGarage(garage);
        ShowSuccess("Vehicle moved.");
    }
    else
    {
        ShowError("Move failed.");
    }
}
void RemoveVehicle()
{
    string regnumber = AskRegnumber();

    var vehicle = garage.FindVehicle(regnumber);

    if (vehicle == null)
    {
        ShowError("Vehicle not found.");
        return;
    }

    decimal price =
        garage.CalculatePrice(vehicle);

    garage.RemoveVehicle(regnumber);

    fileManager.SaveGarage(garage);

    ShowSuccess(
        $"Vehicle removed. Fee: {price} kr");
}
#endregion
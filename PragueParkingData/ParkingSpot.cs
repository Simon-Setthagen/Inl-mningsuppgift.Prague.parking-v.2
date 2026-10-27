using Data;

public class ParkingSpot
{
    public int SpotNumber { get; set; }

    public List<Vehicle> Vehicles { get; set; }
        = new();

    public ParkingSpot(int spotNumber)
    {
        SpotNumber = spotNumber;
    }

    public int UsedSize()
    {
        return Vehicles.Sum(v => v.Size);
    }

    public bool CanPark(Vehicle vehicle)
    {
        return UsedSize() + vehicle.Size <= 2;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        Vehicles.Add(vehicle);
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        Vehicles.Remove(vehicle);
    }
}
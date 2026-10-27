namespace Data
{
    public class ParkingGarage
    {
        public List<ParkingSpot> Spots { get; set; }

        public ParkingGarage()
        {
            Spots = new List<ParkingSpot>();
        }

        public ParkingGarage(int numberOfSpots)
        {
            Spots = new List<ParkingSpot>();

            for (int i = 1; i <= numberOfSpots; i++)
            {
                Spots.Add(new ParkingSpot(i));
            }
        }
        public Vehicle? FindVehicle(string regNumber)
        {
            foreach (var spot in Spots)
            {
                foreach (var vehicle in spot.Vehicles)
                {
                    if (vehicle.RegistrationNumber == regNumber)
                        return vehicle;
                }
            }

            return null;
        }
        public bool VehicleExists(string regNumber)
        {
            return FindVehicle(regNumber) != null;
        }
        public ParkingSpot? FindFreeSpot(Vehicle vehicle)
        {
            return Spots.FirstOrDefault(
                s => s.CanPark(vehicle));
        }
        public bool ParkVehicle(Vehicle vehicle)
        {
            if (VehicleExists(
                vehicle.RegistrationNumber))
                return false;

            var spot = FindFreeSpot(vehicle);

            if (spot == null)
                return false;

            spot.AddVehicle(vehicle);

            return true;
        }
        public bool RemoveVehicle(string regNumber)
        {
            foreach (var spot in Spots)
            {
                var vehicle =
                    spot.Vehicles.FirstOrDefault(
                    v => v.RegistrationNumber == regNumber);

                if (vehicle != null)
                {
                    spot.Vehicles.Remove(vehicle);
                    return true;
                }
            }

            return false;
        }
        public bool MoveVehicle(
    string regNumber,
    int targetSpot)
        {
            var vehicle = FindVehicle(regNumber);

            if (vehicle == null)
                return false;
            var target =
                Spots.FirstOrDefault(
                    s => s.SpotNumber == targetSpot);
            if (target == null)
                return false;

            if (!target.CanPark(vehicle))
                return false;

            RemoveVehicle(regNumber);

            target.AddVehicle(vehicle);

            return true;


        }
        public decimal CalculatePrice(Vehicle vehicle)
        {
            TimeSpan parkedTime =
                DateTime.Now - vehicle.ArrivalTime;

            if (parkedTime.TotalMinutes < 10)
                return 0;

            int hours =
                (int)Math.Ceiling(parkedTime.TotalHours);

            return hours * vehicle.HourPrice;
        }

        public ParkingSpot? FindVehicleSpot(string regNumber)
        {
            return Spots.FirstOrDefault(
                spot => spot.Vehicles.Any(
                    v => v.RegistrationNumber == regNumber));
        }
    }
}

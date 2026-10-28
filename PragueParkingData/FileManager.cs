using System.Text.Json;

namespace Data
{
    public class FileManager
    {
        public void SaveGarage(ParkingGarage garage)
        {
            string json =
                JsonSerializer.Serialize(garage,new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
            File.WriteAllText("garage.json",json);
        }

        public ParkingGarage? LoadGarage()
        {
            if (!File.Exists("garage.json"))
                return null;

            string json =
                File.ReadAllText("garage.json");

            return JsonSerializer.Deserialize<ParkingGarage>(json);
        }
    }
}

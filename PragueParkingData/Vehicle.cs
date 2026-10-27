using Data;
using System.Text.Json.Serialization;
namespace Data
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Car), "car")]
    [JsonDerivedType(typeof(MC), "mc")]
    public abstract class Vehicle
    {
        public string RegistrationNumber { get; set; }
        public DateTime ArrivalTime { get; set; }

        public abstract int Size { get; }
        public abstract decimal HourPrice { get; }

        protected Vehicle(string regNumber)
        {
            RegistrationNumber = regNumber;
            ArrivalTime = DateTime.Now;
        }
    }
}

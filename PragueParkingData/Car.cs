namespace Data
{
    public class Car : Vehicle
    {
        public Car() : base("")
        {

        }
        public Car(string regNumber)
            : base(regNumber)
        {

        }
        public override int Size => 2;
        public override decimal HourPrice => 20;
    }
}
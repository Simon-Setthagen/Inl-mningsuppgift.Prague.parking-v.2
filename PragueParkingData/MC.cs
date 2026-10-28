namespace Data
{
    public class MC : Vehicle
    {
        public MC() : base("")
        {

        }
        public MC(string regNumber)
            : base(regNumber)
        {

        }
        public override int Size => 1;
        public override decimal HourPrice => 10;
    }
}
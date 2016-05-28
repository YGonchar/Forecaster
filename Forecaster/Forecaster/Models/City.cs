namespace Forecaster.Models
{
    public class City
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Country}";
        }
    }
}
using System;

namespace Forecaster.Models
{
    /// <summary>
    ///     Complete weather info.
    /// </summary>
    public class WeatherInfo
    {
        public City City { get; set; }
        public Weather Weather { get; set; }
        public DateTime Time { get; set; }
        public Wind Wind { get; set; }
        public int Humidity { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public GeoCoords GeoCoords { get; set; }

        public override string ToString()
        {
            return City.ToString();
        }
    }
}
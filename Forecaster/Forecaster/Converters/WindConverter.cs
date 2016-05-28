using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Forecaster.Models;
using Forecaster.Utils;
using Xamarin.Forms;

namespace Forecaster.Converters
{
    public class WindConverter : IValueConverter
    {
        private static readonly IEnumerable<WindDirection> Directions;

        static WindConverter()
        {
            Directions = new List<WindDirection>
            {
                new WindDirection("North", iDeg => iDeg > 350 && iDeg < 10),
                new WindDirection("North-East", iDeg => iDeg > 10 && iDeg < 80),
                new WindDirection("East", iDeg => iDeg > 80 && iDeg < 100),
                new WindDirection("South-East", iDeg => iDeg > 100 && iDeg < 170),
                new WindDirection("South", iDeg => iDeg > 170 && iDeg < 190),
                new WindDirection("South-West", iDeg => iDeg > 190 && iDeg < 260),
                new WindDirection("West", iDeg => iDeg > 260 && iDeg < 280),
                new WindDirection("North-West", iDeg => iDeg > 280 && iDeg < 350)
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Wind wind = value as Wind;
            return $"{wind.Speed} m/s\n{GetDegStringRepresentation(wind.Deg)}({wind.Deg})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetDegStringRepresentation(double deg)
        {
            return Directions.FirstOrDefault(direction => direction.IsHit(deg)).ToString();
        }
    }
}
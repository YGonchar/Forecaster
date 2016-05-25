using System;
using System.Globalization;
using Forecaster.Models;
using Xamarin.Forms;

namespace Forecaster.Converters
{
    internal class GeoCoordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GeoCoords coords = (GeoCoords) value;
            return $"[{coords.Latt:F}, {coords.Lon:F}]";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
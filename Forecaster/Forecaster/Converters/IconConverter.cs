using System;
using System.Globalization;
using System.IO;
using Forecaster.Services;
using Xamarin.Forms;
using XLabs.Ioc;

namespace Forecaster.Converters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RestClient client = Resolver.Resolve<RestClient>();
            StreamImageSource src = new StreamImageSource
            {
                Stream = async token =>
                {
                    var bytes = await client.LoadIconAsync(value.ToString());
                    return new MemoryStream(bytes);
                }
            };
            return src;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
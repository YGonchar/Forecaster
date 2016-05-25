using Forecaster.Converters;
using Xamarin.Forms;

namespace Forecaster.Models
{
    [TypeConverter(typeof(WindConverter))]
    public class Wind
    {
        public double Speed { get; set; }
        public double Deg { get; set; }
    }
}
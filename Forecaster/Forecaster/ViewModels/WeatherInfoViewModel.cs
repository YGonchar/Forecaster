using Forecaster.Contracts;
using Forecaster.Models;
using XLabs.Forms.Mvvm;

namespace Forecaster.ViewModels
{
    public class WeatherInfoViewModel : ViewModel, IStatelessViewModel<WeatherInfo>
    {
        public WeatherInfoViewModel(WeatherInfo model)
        {
            Model = model;
        }

        public WeatherInfo Model { get; }
    }
}
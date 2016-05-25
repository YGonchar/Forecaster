using System;
using System.Linq;
using System.Threading.Tasks;
using Forecaster.Models;
using Forecaster.Services;
using Xamarin.Forms;
using XLabs;
using XLabs.Forms.Mvvm;
using XLabs.Ioc;

namespace Forecaster.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        private string _searchText;
        private City _selectedCity;
        private WeatherInfo _weatherInfoModel;
        private WeatherInfoViewModel _weatherInfoViewModel;

        public MainViewModel()
        {
            InitState();
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public WeatherInfoViewModel InfoViewModel
        {
            get { return _weatherInfoViewModel; }
            set { SetProperty(ref _weatherInfoViewModel, value); }
        }

        public RelayCommand<string> SearchCommand { get; set; }

        private City SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                if (_selectedCity == null)
                {
                    SearchText = "City is not found";
                    return;
                }

                IsBusy = true;
                FillWeatherInfo();
            }
        }

        private async Task FillWeatherInfo()
        {
            var tcs = new TaskCompletionSource<object>();
            try
            {
                _weatherInfoModel = await Resolver.Resolve<RestClient>().GetWeatherByCityAsync(_selectedCity);

                Device.BeginInvokeOnMainThread(() =>
                {
                    InfoViewModel = new WeatherInfoViewModel(_weatherInfoModel);
                    IsBusy = false;
                });
                tcs.SetResult(null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }
            await tcs.Task;
        }

        private void InitState()
        {
            SearchCommand = new RelayCommand<string>(ExecuteSearch, s => !string.IsNullOrWhiteSpace(s));
        }

        private void ExecuteSearch(string searchParam)
        {
            RestClient client = Resolver.Resolve<RestClient>();
            IsBusy = true;
            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(async () =>
            {
                var cities = await client.FindCitiesAsync(searchParam);
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = false;
                    SelectedCity = cities.FirstOrDefault();
                });
            });
        }
    }
}
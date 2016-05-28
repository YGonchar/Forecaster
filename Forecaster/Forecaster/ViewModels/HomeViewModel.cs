using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forecaster.Models;
using Forecaster.Resources;
using Forecaster.Services;
using Xamarin.Forms;
using XLabs;
using XLabs.Forms.Mvvm;
using XLabs.Ioc;

namespace Forecaster.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        private List<City> _cities;
        private string _searchText;
        private City _selectedCity;
        private WeatherInfo _weatherInfoModel;

        public HomeViewModel()
        {
            InitState();
        }

        public RelayCommand<string> SearchCommand { get; set; }
        public RelayCommand<WeatherInfo> AddToFavoritesCommand { get; set; }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public List<City> Cities
        {
            get { return _cities; }
            set
            {
                SetProperty(ref _cities, value);
                if (!_cities.Any())
                {
                    MessagingCenter.Send(this, "searchingError", $"There is no city with name {SearchText}");
                    SearchText = string.Empty;
                }
                SelectedCity = _cities?.FirstOrDefault();
            }
        }

        public WeatherInfo InfoModel
        {
            get { return _weatherInfoModel; }
            set
            {
                SetProperty(ref _weatherInfoModel, value);
                AddToFavoritesCommand.RaiseCanExecuteChanged();
            }
        }

        public City SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                if (_selectedCity == null)
                    return;

                SearchText = string.Empty;

                IsBusy = true;
                FillWeatherInfoAsync().Wait();
            }
        }

        private void InitState()
        {
            SearchCommand = new RelayCommand<string>(ExecuteSearch, s => !string.IsNullOrWhiteSpace(s));
            AddToFavoritesCommand = new RelayCommand<WeatherInfo>(Execute, info => { return info != null; });
        }

        private async void Execute(WeatherInfo weatherInfo)
        {
            MessagingCenter.Send(this, MessageIds.AddCityToFavorites, weatherInfo);
        }

        private async Task FillWeatherInfoAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            try
            {
                var info = await Resolver.Resolve<RestClient>().GetWeatherByCityAsync(_selectedCity);

                Device.BeginInvokeOnMainThread(() =>
                {
                    InfoModel = info;
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

        private void ExecuteSearch(string searchParam)
        {
            RestClient client = Resolver.Resolve<RestClient>();
            IsBusy = true;

            Task.Factory.StartNew(async () =>
            {
                var cities = await client.FindCitiesAsync(searchParam);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Cities = new List<City>(cities);
                    IsBusy = false;
                });
            });
        }
    }
}
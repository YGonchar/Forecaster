using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Forecaster.Models;
using Forecaster.Resources;
using Forecaster.Services;
using Xamarin.Forms;
using XLabs;
using XLabs.Forms.Mvvm;
using XLabs.Ioc;
using IViewModel = Forecaster.Contracts.IViewModel;

namespace Forecaster.ViewModels
{
    internal class FavoritesViewModel : ViewModel, IViewModel
    {
        public FavoritesViewModel()
        {
            InitState();
        }

        public ObservableCollection<WeatherInfo> Favorites { get; set; }
        public RelayCommand<WeatherInfo> RemoveFromFavorites { get; set; }

        private async void InitState()
        {
            MessagingCenter.Subscribe<HomeViewModel, WeatherInfo>(this, MessageIds.AddCityToFavorites, AddFavoritesCity);
            RemoveFromFavorites = new RelayCommand<WeatherInfo>(ExecuteRemoveFromFavorites, info => Favorites.Any());
            await LoadFavoritesAsync();
        }

        private void ExecuteRemoveFromFavorites(WeatherInfo weatherInfo)
        {
            Favorites.Remove(weatherInfo);
            Application.Current.Properties.Remove(new KeyValuePair<string, object>(weatherInfo.City.Name,
                weatherInfo.City.Id));
        }

        private void AddFavoritesCity(HomeViewModel sender, WeatherInfo info)
        {
            SaveFavoriteCity(info.City);
            Favorites.Add(info);
        }

        private void SaveFavoriteCity(City city)
        {
            Application.Current.Properties[city.Name] = city.Id;
        }

        private async Task LoadFavoritesAsync()
        {
            var info = await Task.WhenAll(
                Application.Current.Properties.Values
                    .Select(i => Resolver.Resolve<RestClient>().GetWeatherByCityIdAsync((int) i)));

            Device.BeginInvokeOnMainThread(() => Favorites = new ObservableCollection<WeatherInfo>(info));
        }
    }
}
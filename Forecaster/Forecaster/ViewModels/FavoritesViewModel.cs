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

namespace Forecaster.ViewModels
{
    internal class FavoritesViewModel : ViewModel
    {
        public FavoritesViewModel()
        {
            InitState();
        }

        public ObservableCollection<WeatherInfo> Favorites { get; set; }

        public RelayCommand EditCommand { get; set; }

        private async void InitState()
        {
            MessagingCenter.Subscribe<HomeViewModel, WeatherInfo>(this, MessageIds.AddCityToFavorites, AddFavoritesCity);

            await LoadFavoritesAsync();
        }

        private void AddFavoritesCity(HomeViewModel sender, WeatherInfo info)
        {
            PersistFavoriteCity(info.City);
            Favorites.Add(info);
            EditCommand.RaiseCanExecuteChanged();
        }

        private void PersistFavoriteCity(City city)
        {
            Application.Current.Properties[city.Name] = city.Id;
        }

        private async Task LoadFavoritesAsync()
        {
#if !DEBUG
            var info = await Task.WhenAll(
                Enumerable.Range(0, 3)
                .Select(i => Resolver.Resolve<RestClient>().GetWeatherByCityAsync(default(City))));

            Device.BeginInvokeOnMainThread(() => Favorites = new ObservableCollection<WeatherInfo>(info));
#else
            var info = await Task.WhenAll(
                Application.Current.Properties.Keys
                    .Select(i => Resolver.Resolve<RestClient>().GetWeatherByCityIdAsync(int.Parse(i.ToString()))));

            Device.BeginInvokeOnMainThread(() => Favorites = new ObservableCollection<WeatherInfo>(info));
#endif
        }
    }
}
using System.Threading.Tasks;
using Forecaster.Services;
using Xamarin.Forms;
using XLabs;
using XLabs.Forms.Mvvm;

namespace Forecaster.ViewModels
{
    class MainViewModel : ViewModel
    {
        public MainViewModel()
        {
            InitState();
        }

        private void InitState()
        {
            SearchCommand = new RelayCommand<string>(ExecuteSearch, s => !string.IsNullOrWhiteSpace(s));
        }

        private void ExecuteSearch(string searchParam)
        {
            IsBusy = true;
            Task.Factory.StartNew(async () =>
           {
               RestClient client = DependencyService.Get<RestClient>();
               var cities = await client.FindCitiesAsync(searchParam);
               Device.BeginInvokeOnMainThread(() => { IsBusy = false; });
           });
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                SearchCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand<string> SearchCommand { get; set; }
    }
}

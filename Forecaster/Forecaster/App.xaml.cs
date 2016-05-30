using Forecaster.Services;
using Forecaster.Views;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Services;

namespace Forecaster
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            ConfigureIoC();
            MainPage = new NavigationPage(new MainPage());
            CheckConnection();
        }

        private void CheckConnection()
        {
            INetwork network = DependencyService.Get<INetwork>();
            if (network.InternetConnectionStatus() == NetworkStatus.NotReachable)
            {
                MainPage.DisplayAlert("Connection problems",
                    "No internet connection. It may cause to crash. Please check connection and restart the app.", "Ok");
            }
        }

        private void ConfigureIoC()
        {
            IDependencyContainer container = new SimpleContainer();
            Resolver.SetResolver(container.GetResolver());

            container.Register(resolver => DependencyService.Get<RestClient>());
        }
    }
}
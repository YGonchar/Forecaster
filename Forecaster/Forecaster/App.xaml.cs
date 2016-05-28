using Forecaster.Services;
using Forecaster.Views;
using Xamarin.Forms;
using XLabs.Ioc;

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
        }

        private void ConfigureIoC()
        {
            IDependencyContainer container = new SimpleContainer();
            Resolver.SetResolver(container.GetResolver());

            container.Register<RestClient>(resolver =>
            {
#if !DEBUG
                return new RestClientMock();
#else
                return DependencyService.Get<RestClient>();
#endif
            });
        }
    }
}
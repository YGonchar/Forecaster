using Forecaster.ViewModels;
using Forecaster.Views;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
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
            MainPage = Resolver.Resolve<MainPage>();
        }

        private void ConfigureIoC()
        {
            IDependencyContainer container = new SimpleContainer();
            Resolver.SetResolver(container.GetResolver());

            container.Register<MainPage>(typeof(MainPage));
        }
    }
}

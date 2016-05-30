using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XLabs.Platform.Services;

namespace Forecaster.Droid
{
    [Activity(Label = "Forecaster", Icon = "@drawable/appIcon", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            DependencyService.Register<INetwork, Network>();
            Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}
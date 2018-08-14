using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;

using Android.Widget;

namespace PaketGlobal.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, Icon = "@drawable/icon")]
    public class SplashActivity : AppCompatActivity
    {
        private bool isOpen = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SplashScreen);

            FindViewById<TextView>(Resource.Id.AppVersionLabel).Text = $"Version {PackageManager.GetPackageInfo(PackageName, 0).VersionName}" 
                + "\n" + ThisAssembly.Git.Commit;
        }

		// Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();

            if(isOpen==false)
            {
                Task startupWork = new Task(() => { SimulateStartup(); });
                startupWork.Start();
            }
          
        }


        // Simulates background work that happens behind the splash screen
        async void SimulateStartup ()
        {
            await Task.Delay(1000); // Simulate a bit of startup work.

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));

            isOpen = true;

        }
    }
}
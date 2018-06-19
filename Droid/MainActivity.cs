using Android.OS;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;

using Acr.UserDialogs;
using LY.Count.Android.Sdk;
using GalaSoft.MvvmLight.Ioc;

namespace PaketGlobal.Droid
{
	[Activity(Label = "PaketGlobal.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		internal static MainActivity Instance { get; private set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			Countly.SharedInstance().Init(this, Config.CountlyServerURL, Config.CountlyAppKey).EnableCrashReporting();
			//Countly.SharedInstance().SetLoggingEnabled(true);

			Instance = this;

			ZXing.Net.Mobile.Forms.Android.Platform.Init();

			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			RegisterServiceContainers();

			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());

			UserDialogs.Init(this);
		}

		protected override void OnStart()
		{
			base.OnStart();

			Countly.SharedInstance().OnStart(this);
		}

		protected override void OnStop()
		{
			Countly.SharedInstance().OnStop();

			base.OnStop();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		private void RegisterServiceContainers()
		{
			if (!SimpleIoc.Default.IsRegistered<INotificationService>()) {
				SimpleIoc.Default.Register<INotificationService>(() => {
					return new NotificationService();
				});
			}

			if (!SimpleIoc.Default.IsRegistered<IAppInfoService>()) {
				SimpleIoc.Default.Register<IAppInfoService>(() => {
					return new AppInfoService();
				});
			}

			if (!SimpleIoc.Default.IsRegistered<IAccountService>()) {
				SimpleIoc.Default.Register<IAccountService>(() => {
					return new AccountService();
				});
			}
		}
	}
}

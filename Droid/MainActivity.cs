using Android.OS;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Acr.UserDialogs;
using LY.Count.Android.Sdk;
using GalaSoft.MvvmLight.Ioc;
using Android.Graphics;
using Android.Widget;
using Android.Views;
using System;
using RoundedBoxView.Forms.Plugin.Droid;
using Xamarin.Forms.Platform.Android;

namespace PaketGlobal.Droid
{
    [Activity(Label = "PaketGlobal.Droid", ScreenOrientation = ScreenOrientation.Portrait, Icon = "@drawable/icon", Theme = "@style/MyTheme.Base", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		internal static MainActivity Instance { get; private set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);


            XamEffects.Droid.Effects.Init();
            XFGloss.Droid.Library.Init(this, bundle);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

			Countly.SharedInstance().Init(this, Config.CountlyServerURL, Config.CountlyAppKey).EnableCrashReporting();
            //Countly.SharedInstance().SetLoggingEnabled(true);

            Instance = this;


			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			RegisterServiceContainers();

			global::Xamarin.Forms.Forms.Init(this, bundle);

            RoundedBoxViewRenderer.Init();

            LoadApplication(new App());

			UserDialogs.Init(this);

            InitializeUIAsync();
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

        private void InitializeUIAsync()         {
            // from https://forums.xamarin.com/discussion/comment/282515#Comment_282515
            try             {                 if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)                 {                     Window.DecorView.SystemUiVisibility = 0;                      var statusBarHeightInfo = typeof(FormsAppCompatActivity).GetField("_statusBarHeight",                         System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);                      if (statusBarHeightInfo != null)                     {                         statusBarHeightInfo.SetValue(this, 0);                     }

                }             }             catch (Exception ex)             {                 Console.WriteLine(ex);             }         }

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

            if (!SimpleIoc.Default.IsRegistered<IClipboardService>())
            {
                SimpleIoc.Default.Register<IClipboardService>(() => {
                    return new ClipboardService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IDeviceService>())
            {
                SimpleIoc.Default.Register<IDeviceService>(() => {
                    return new DeviceService();
                });
            }
		}
	}

   
}

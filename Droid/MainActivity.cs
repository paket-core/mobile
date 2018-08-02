using Android.OS;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Locations;

using Acr.UserDialogs;

using LY.Count.Android.Sdk;

using GalaSoft.MvvmLight.Ioc;

using System;

using RoundedBoxView.Forms.Plugin.Droid;

using Xamarin.Forms.Platform.Android;

using Plugin.CurrentActivity;
using System.Threading.Tasks;
using Android;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using Plugin.Permissions;

namespace PaketGlobal.Droid
{
    [Activity(Label = "PaketGlobal.Droid", ScreenOrientation = ScreenOrientation.Portrait, Icon = "@drawable/icon", Theme = "@style/MyTheme.Base", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]


	[IntentFilter (new[] { Intent.ActionView },
        Categories = new[] {
            Intent.ActionView,
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "paketglobal",
        DataHost="packages")]

	public class MainActivity : FormsAppCompatActivity
	{
		internal static MainActivity Instance { get; private set; }

        private Dialog progressDialog;

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

            CrossCurrentActivity.Current.Init(this, bundle);

            InitializeUIAsync();

            if (Intent != null && Intent.DataString != null)
            {
                try
                {
                    string package = "";
                    package = Intent.Data.GetQueryParameter("id");

                    if (package != null && package != "")
                    {
                        Xamarin.Forms.MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.APP_LAUNCHED_FROM_DEEP_LINK, package);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
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
            global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);         
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

            if (!SimpleIoc.Default.IsRegistered<ILocationSharedService>())
            {
                SimpleIoc.Default.Register<ILocationSharedService>(() => {
                    return new LocationSharedService();
                });
            }
		}

        #region ProgressBar

        public void ShowProgressDialog()
        {
            if(progressDialog==null)
            {
                progressDialog = new Dialog(this);
                progressDialog.SetCancelable(false);


               // var ring = ProgressRing();

                var dialog = new ProgressBar(this);
                dialog.Indeterminate = true;
                dialog.Visibility = ViewStates.Visible;

                progressDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                progressDialog.SetContentView(dialog);
            }

            progressDialog.Show();
        }

        public void HideProgressDialog()
        {
            progressDialog.Dismiss();
        }

        #endregion

        #region Android Location Service methods

        public void StartLocationUpdate()
        {
            LocationAppManager.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
                LocationAppManager.Current.LocationService.LocationChanged += HandleLocationChanged;
            };

            LocationAppManager.StartLocationService();
        }

        public void StopLocationUpdate()
        {
            LocationAppManager.StopLocationService();
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            
        }


        #endregion
	}

   
}

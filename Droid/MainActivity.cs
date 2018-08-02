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
using System.Threading;

using RoundedBoxView.Forms.Plugin.Droid;

using Xamarin.Forms.Platform.Android;

namespace PaketGlobal.Droid
{
    [Activity(Label = "PaketGlobal.Droid", ScreenOrientation = ScreenOrientation.Portrait, Icon = "@drawable/icon", Theme = "@style/MyTheme.Base", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]


    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] {
            Intent.ActionView,
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "paketglobal",
        DataHost = "packages")]

    public class MainActivity : FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        private Dialog progressDialog;
        private static ProgressBar circularbar;
        private int progressStatus = 0, progressStatus1 = 100;
        private System.Threading.Thread progressThread;

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
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

            InitializeUIAsync();

            if (Intent != null && Intent.DataString != null)
            {
                try
                {
                    string package = "";
                    package = Intent.Data.GetQueryParameter("id");

                    if (package != null && package != "")
                    {
                        Xamarin.Forms.MessagingCenter.Send<string, string>("MyApp", "AppLaunchedFromDeepLink", package);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // This event fires when the ServiceConnection lets the client (our App class) know that
            // the Service is connected. We use this event to start updating the UI with location
            // updates from the Service
            //LocationAppManager.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
            //    // notifies us of location changes from the system
            //    LocationManager.Current.LocationService.LocationChanged += HandleLocationChanged;
            //};

            //LocationAppManager.StartLocationService();

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
            if (!SimpleIoc.Default.IsRegistered<INotificationService>())
            {
                SimpleIoc.Default.Register<INotificationService>(() => {
                    return new NotificationService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IAppInfoService>())
            {
                SimpleIoc.Default.Register<IAppInfoService>(() => {
                    return new AppInfoService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IAccountService>())
            {
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

        #region ProgressBar

        public void ShowProgressDialog()
        {
            if (progressDialog == null)
            {
                progressDialog = new Dialog(this);
                progressDialog.SetCancelable(false);

                progressDialog.Window.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.Transparent));
                progressDialog.SetContentView(Resource.Layout.progress_layout);

                circularbar = progressDialog.FindViewById<ProgressBar>(Resource.Id.circularProgressbar);
                circularbar.Max = 100;
                circularbar.Progress = 0;
                circularbar.SecondaryProgress = 100;

                progressStatus = 0;
                progressStatus1 = 0;

                progressThread = new System.Threading.Thread(new ThreadStart(delegate {
                    while (progressStatus < 100)
                    {
                        progressStatus += 1;
                        progressStatus1 += 1;
                        circularbar.Progress = progressStatus1;
                        if (progressStatus == 99)
                        {
                            progressStatus = 0;
                            progressStatus1 = 0;
                        }
                        System.Threading.Thread.Sleep(3);
                    }
                }));
                progressThread.Start();
            }

            progressDialog.Show();
        }

        public void HideProgressDialog()
        {
            progressStatus = 101;

            progressDialog.Dismiss();
            progressThread.Abort();

            progressThread = null;
            progressDialog = null;
        }

        #endregion

        #region Android Location Service methods

        ///<summary>
        /// Updates UI with location data
        /// </summary>
        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
        }

        #endregion
    }


}

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

using Plugin.CurrentActivity;
using System.Threading.Tasks;
using Android;
using Android.Support.V4.App;
using Android.Support.Design.Widget;
using Plugin.Permissions;
using Xamarin.Forms.GoogleMaps.Android;

using Android.Gms.Common;
using Firebase.Iid;
using Android.Util;
using Android.App.Job;
using Android.Icu.Util;
using JobSchedulerType = Android.App.Job.JobScheduler;

namespace PaketGlobal.Droid
{
    [Activity(Label = "DeliverIt", ScreenOrientation = ScreenOrientation.Portrait, Icon = "@drawable/icon", Theme = "@style/MyTheme.Base", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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
        static readonly string TAG = "MainActivity";
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;

        internal static MainActivity Instance { get; private set; }

        private Dialog progressDialog;
        private static ProgressBar circularbar;
        private int progressStatus = 0, progressStatus1 = 100;
        private System.Threading.Thread progressThread;

        private Intent EventServiceIntent;
        private Intent PackageServiceIntent;

        public static bool IsStoppedServices = false;
        public static bool IsActive = false;

        JobScheduler jobScheduler;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            jobScheduler = (JobScheduler)GetSystemService(JobSchedulerService);

            XamEffects.Droid.Effects.Init();
            XFGloss.Droid.Library.Init(this, bundle);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            Xamarin.FormsMaps.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, bundle, null);
            Stormlion.PhotoBrowser.Droid.Platform.Init(this);
            Vapolia.Droid.Lib.Effects.PlatformGestureEffect.Init();

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
            CrossCurrentActivity.Current.Activity = this;

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

            //StartPackageService();

            IsPlayServicesAvailable();
            CreateNotificationChannel();

            var refreshedToken = FirebaseInstanceId.Instance.Token;
            if (refreshedToken != null)
            {
                App.Locator.DeviceService.FCMToken = refreshedToken;

                SendRegistrationToServer(refreshedToken);
            }
        }

        internal static async void SendRegistrationToServer(string token)
        {
            if (App.Locator.Profile.Activated)
            {
                var result = await App.Locator.IdentityServiceClient.RegisterFCM(App.Locator.Profile.Pubkey, token);
                Console.WriteLine(result);
            }
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            MainActivity.IsActive = true;
        }

        protected override void OnResume()
        {
            base.OnResume();

            MainActivity.IsActive = true;
        }

        protected override void OnStart()
        {
            base.OnStart();

            MainActivity.IsActive = true;

            PackageService.IsNeedRequestPackages = false;
            EventService.IsNeedSendEvents = false;

            Countly.SharedInstance().OnStart(this);

        }

        protected override void OnStop()
        {
            MainActivity.IsActive = false;

            Countly.SharedInstance().OnStop();

            PackageService.IsNeedRequestPackages = true;
            EventService.IsNeedSendEvents = false;

            base.OnStop();
        }

        protected override void OnDestroy()
        {
            MainActivity.IsActive = false;

            PackageService.IsNeedRequestPackages = true;
            EventService.IsNeedSendEvents = false;

            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            if (App.Locator.DeviceService.IsNeedAlertDialogToCloseLaunchPackage)
            {
                EventHandler handler = (se, ee) =>
                {
                    if (ee != null)
                    {
                        base.OnBackPressed();
                    }
                };

                App.Locator.NotificationService.ShowErrorMessage(AppResources.LaunchLeaveMessage, false, handler, AppResources.Leave, AppResources.Cancel);
            }
            else if (App.Locator.DeviceService.IsNeedAlertDialogToClose)
            {
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("PaketGlobal");
                builder.SetMessage("Do you really want to exit?");
                builder.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    this.FinishAffinity();
                });
                builder.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                });

                // Keep what runs on the UI thread to a minimum
                this.RunOnUiThread(() =>
                {
                    try
                    {
                        Dialog dialog = builder.Create();
                        dialog.Show();
                    }
                    catch (WindowManagerBadTokenException)
                    {

                    }
                    catch (Exception)
                    {

                    }
                });
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (IntentHelper.IsMobileIntent(requestCode))
            {
                IntentHelper.ActivityResult(requestCode, data);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private void InitializeUIAsync()         {
            // from https://forums.xamarin.com/discussion/comment/282515#Comment_282515
            try             {                 if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)                 {                     Window.DecorView.SystemUiVisibility = 0;                      var statusBarHeightInfo = typeof(FormsAppCompatActivity).GetField("_statusBarHeight",                         System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);                      if (statusBarHeightInfo != null)                     {                         statusBarHeightInfo.SetValue(this, 0);                     }

                }             }             catch (Exception ex)             {                 Console.WriteLine(ex);             }         }

        private void RegisterServiceContainers()
        {
            if (!SimpleIoc.Default.IsRegistered<INotificationService>())
            {
                SimpleIoc.Default.Register<INotificationService>(() =>
                {
                    return new NotificationService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IAppInfoService>())
            {
                SimpleIoc.Default.Register<IAppInfoService>(() =>
                {
                    return new AppInfoService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IAccountService>())
            {
                SimpleIoc.Default.Register<IAccountService>(() =>
                {
                    return new AccountService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IClipboardService>())
            {
                SimpleIoc.Default.Register<IClipboardService>(() =>
                {
                    return new ClipboardService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IDeviceService>())
            {
                SimpleIoc.Default.Register<IDeviceService>(() =>
                {
                    return new DeviceService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<ILocationSharedService>())
            {
                SimpleIoc.Default.Register<ILocationSharedService>(() =>
                {
                    return new LocationSharedService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IEventSharedService>())
            {
                SimpleIoc.Default.Register<IEventSharedService>(() =>
                {
                    return new EventSharedService();
                });
            }
        }

        #region Addrss Book

        public void OpenAddressBook()
        {
            var contactPickerIntent = new Intent(Intent.ActionPick, Android.Provider.ContactsContract.Contacts.ContentUri);
            StartActivityForResult(contactPickerIntent, 101);
        }

        #endregion

        #region ProgressBar

        public void ShowProgressDialog()
        {
            try{
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

                    progressThread = new System.Threading.Thread(new ThreadStart(delegate
                    {
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
            catch(Exception ex){
                Console.WriteLine(ex);
            }
        }

        public void HideProgressDialog()
        {
            if (progressDialog != null)
            {
                try
                {
                    progressStatus = 101;

                    progressDialog.Dismiss();
                    progressThread.Abort();

                    progressThread = null;
                    progressDialog = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        #endregion

        #region Packages

        public void StartPackageService()
        {
            //if (PackageServiceIntent == null)
            //{
            //    PackageServiceIntent = new Intent(this, typeof(PackageService));
            //    Android.App.Application.Context.StartService(PackageServiceIntent);
            //}

        }

        public void StopPackageService()
        {
            //if (PackageServiceIntent != null && !IsStoppedServices)
            //{
            //    Android.App.Application.Context.StopService(PackageServiceIntent);
            //    PackageServiceIntent = null;
            //}
        }

        #endregion

        #region Events

        public void StartEventsService()
        {
            //if (EventServiceIntent == null && !IsStoppedServices)
            //{
            //    EventServiceIntent = new Intent(this, typeof(EventService));
            //    Android.App.Application.Context.StartService(EventServiceIntent);
            //}
        }

        public void StopEventsService()
        {
            //if (EventServiceIntent != null)
            //{
            //    Android.App.Application.Context.StopService(EventServiceIntent);
            //    EventServiceIntent = null;
            //}
        }

        #endregion

        #region Android Location Service methods

        public void StartLocationUpdate()
        {
            //if (!IsStoppedServices)
            //{
            //    LocationAppManager.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            //    {
            //        LocationAppManager.Current.LocationService.LocationChanged += HandleLocationChanged;
            //    };

            //    LocationAppManager.StartLocationService();
            //}
        }

        public void StopLocationUpdate()
        {
            //  LocationAppManager.StopLocationService();
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {

        }

        #endregion

        #region FCM

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Console.WriteLine(GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Console.WriteLine("not supported");
                    Finish();
                }
                return false;
            }
            else
            {
                Console.WriteLine("Google Play Services is available.");
                return true;
            }
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        #endregion

        #region Job

        public void ScheduleJob()
        {
            try{
                Intent intent = new Intent();
                String packageName = Application.Context.PackageName;

                PowerManager pm = (PowerManager)Application.Context.GetSystemService(Context.PowerService);
                if (pm.IsIgnoringBatteryOptimizations(packageName))
                    intent.SetAction(Android.Provider.Settings.ActionIgnoreBatteryOptimizationSettings);
                else
                {
                    intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                    intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                }

                intent.AddFlags(ActivityFlags.NewTask);

                Application.Context.StartActivity(intent);
            }
            catch (Exception ex){
                Console.WriteLine(ex);
            }

            var tm = (JobSchedulerType)GetSystemService(Context.JobSchedulerService);
            var jobs = tm.AllPendingJobs;
            tm.CancelAll();

            JobInfo.Builder builder = this.CreateJobInfoBuilder()
                .SetPersisted(true)
                .SetRequiresDeviceIdle(false)
                .SetPeriodic(Config.UpdateTimeInterval * 60000)
                .SetRequiredNetworkType(NetworkType.Any);
                
            int result = jobScheduler.Schedule(builder.Build());
            if (result == JobScheduler.ResultSuccess)
            {
                Log.Debug(TAG, "Job started!");
            }
            else
            {
                Log.Warn(TAG, "Problem starting the job " + result);
            }
        }

        #endregion
    }
}

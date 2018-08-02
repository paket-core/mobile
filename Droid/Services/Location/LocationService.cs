using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Locations;


namespace PaketGlobal.Droid
{
    [Service]
    [IntentFilter(new String[] { "PaketGlobalLocation.Droid" })]
    public class LocationService : Service, ILocationListener
    {
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const string ACTION_MAIN_ACTIVITY = "PaketGlobalLocation.action.MAIN_ACTIVITY";
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };

        LocationServiceBinder binder;

        private LocationManager locationManager;
        private double lastLatitude = double.MinValue;
        private double lastLongitude = double.MinValue;
        private DateTime lastUpdated = DateTime.MinValue;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                RegisterForegroundService();
            }

            InitializeBackgroundWork();

            return StartCommandResult.Sticky;
        }


        public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
        {
            binder = new LocationServiceBinder(this);
            return binder;
        }


        private void InitializeBackgroundWork()
        {
            try
            {
                if (locationManager == null)
                {
                    locationManager = GetSystemService(LocationService) as LocationManager;

                    var locationCriteria = new Criteria();
                    locationCriteria.Accuracy = Accuracy.NoRequirement;
                    locationCriteria.PowerRequirement = Power.NoRequirement;

                    // string locationProvider = locationManager.GetBestProvider(locationCriteria, true);

                    string locationProvider = LocationManager.NetworkProvider;
                    locationManager.RequestLocationUpdates(locationProvider, 2000, 0, this);
                    //locationManager.RequestSingleUpdate(locationCriteria, this, null);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void RegisterForegroundService()
        {
            var notificationBuilder = new Notification.Builder(this)
            .SetContentTitle("PaketGlobal")
            .SetContentText("Location update started")
            .SetContentIntent(BuildIntentToShowMainActivity())
            .SetOngoing(true);

            var notification = notificationBuilder.Build();

            // Enlist this instance of the service as a foreground service
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        #region ILocationListener Members


        public void OnLocationChanged(Android.Locations.Location location)
        {
            OnLocationChangedAsync(location);

            lastLatitude = location.Latitude;
            lastLongitude = location.Longitude;

            lastUpdated = DateTime.Now;

            this.LocationChanged(this, new LocationChangedEventArgs(location));
        }

        public void OnProviderDisabled(string provider)
        {
            
        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {

        }

        public async void OnLocationChangedAsync(Android.Locations.Location location)
        {
            var result = await PaketGlobal.App.Locator.ServiceClient.MyPackages();

            if(result.Packages!=null){
                if(result.Packages.Count>0)
                {
                    var v = (Vibrator)Android.App.Application.Context.GetSystemService(Context.VibratorService);
                    v.Vibrate(1000);
                }
            }
        }


        #endregion
    }
}
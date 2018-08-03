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
        private const string SERVICE_STARTED_KEY = "has_service_been_started";
        private const string ACTION_MAIN_ACTIVITY = "PaketGlobalLocation.action.MAIN_ACTIVITY";
        private const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };

        LocationServiceBinder binder;

        private LocationManager locationManager;
        private double lastLatitude = double.MinValue;
        private double lastLongitude = double.MinValue;
        private DateTime lastUpdated = DateTime.MinValue;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                RegisterForegroundService();
            }

            InitializeBackgroundWork();

            return StartCommandResult.Sticky;
        }


        public override IBinder OnBind(Intent intent)
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

					string locationProvider = locationManager.GetBestProvider(locationCriteria, true);
                    //string locationProvider = LocationManager.NetworkProvider;

                    locationManager.RequestLocationUpdates(locationProvider, 2000, 100, this);
                    //locationManager.RequestSingleUpdate(locationCriteria, this, null);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RegisterForegroundService()
        {
			var notificationBuilder = (Build.VERSION.SdkInt >= BuildVersionCodes.O ? new Notification.Builder(this, CreateNotificationChannel()) : new Notification.Builder(this))
            .SetContentTitle("PaketGlobal")
            .SetContentText("Location update started")
            .SetContentIntent(BuildIntentToShowMainActivity())
            .SetOngoing(true);

            var notification = notificationBuilder.Build();

            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

		private string CreateNotificationChannel()
		{
			var channelId = "my_service";
			var channelName = "My Background Service";

			var chan = new NotificationChannel(channelId, channelName, NotificationImportance.None);
			chan.LightColor = Android.Graphics.Color.Blue;
			chan.LockscreenVisibility = NotificationVisibility.Private;
			var service = GetSystemService(Context.NotificationService) as NotificationManager;

			service.CreateNotificationChannel(chan);

			return channelId;
		}

        private PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        #region ILocationListener Members


        public void OnLocationChanged(Location location)
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

        public async void OnLocationChangedAsync(Location location)
        {
            var result = await App.Locator.ServiceClient.MyPackages();

            if (result.Packages != null)
            {
                var packages = result.Packages;
                var myPubkey = App.Locator.Profile.Pubkey;

                foreach (Package package in packages)
                {
                    var myRole = myPubkey == package.LauncherPubkey ? PaketRole.Launcher :
                                                    (myPubkey == package.RecipientPubkey ? PaketRole.Recipient : PaketRole.Courier);
                    
                    if (myRole == PaketRole.Courier)
                    {
						var locationString = location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        if (locationString.Length > 24)
                        {
                            locationString = locationString.Substring(0, 24);
                        }
                        await App.Locator.ServiceClient.ChangeLocation(package.PaketId, locationString);
                    }
                }
            }
        }


        #endregion

     }
}
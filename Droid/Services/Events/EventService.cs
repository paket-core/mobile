using System;
using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using System.Threading;
using Plugin.Geolocator;
using Android.Support.V4.App;

namespace PaketGlobal.Droid
{

    [Service]
    public class EventService : Service
    {
        public static bool IsNeedSendEvents = false;

        private int DELAY_BETWEEN_LOG_MESSAGES = 600000;
        private int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        private const string SERVICE_STARTED_KEY = "enets_has_service_been_started";
        private const string ACTION_MAIN_ACTIVITY = "PaketGlobalEvent.action.MAIN_ACTIVITY";
        private const string ACTION_STOP_SERVICE = "ACTION_STOP_SERVICE";

        private Handler handler;
        private Action runnable;

        public override void OnCreate()
        {
            base.OnCreate();

            handler = new Handler();

            runnable = new Action(() =>
            {
                SendUsedEvent();

                handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
            });
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent.Action != null)
            {
                if (intent.Action.Equals(ACTION_STOP_SERVICE))
                {
                    StopForeground(true);
                    StopSelf();

                    MainActivity.IsStoppedServices = true;
                    MainActivity.Instance.StopLocationUpdate();
                    MainActivity.Instance.StopEventsService();
                    MainActivity.Instance.StopPackageService();
                }

                return StartCommandResult.NotSticky;
            }

            RegisterForegroundService();

            handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);

            return StartCommandResult.Sticky;
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        public override void OnDestroy()
        {
            // Stop the handler.
            handler.RemoveCallbacks(runnable);

            base.OnDestroy();
        }


        private void RegisterForegroundService()
        {
            var notificationBuilder = (Build.VERSION.SdkInt >= BuildVersionCodes.O ? new Notification.Builder(this, CreateNotificationChannel()) : new Notification.Builder(this))
                .SetContentTitle("DeliverIt")
                .SetContentText("DeliverIt background service is running")
                .SetSmallIcon(Resource.Drawable.ic_notification)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .AddAction(BuildStopServiceAction())
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

        Notification.Action BuildStopServiceAction()
        {
            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            var builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause,
                                                          GetText(Resource.String.stop_service),
                                                          stopServicePendingIntent);
            return builder.Build();

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

        private async void SendUsedEvent()
        {
            if (App.Locator.Profile.Activated && EventService.IsNeedSendEvents)
            {
                var result = await App.Locator.RouteServiceClient.AddEvent(Constants.EVENT_APP_USED);
                Console.WriteLine(result);

                var resultPackages = await App.Locator.RouteServiceClient.MyPackages();

                if (resultPackages != null && resultPackages.Packages != null)
                {
                    var packages = resultPackages.Packages;

                    bool enabled = App.Locator.AccountService.ShowNotifications;

                    if (enabled)
                    {
                        foreach (Package p1 in packages)
                        {
                            var isExpiredNeedShow = App.Locator.Packages.IsPackageExpiredNeedShow(p1);

                            if (isExpiredNeedShow)
                            {
                                var text = "Your package " + p1.ShortEscrow + " expired";
                                PublishLocalNotification(text, "Please check your Packages archive for more details");
                            }
                        }
                    }
                }
            }
        }

        private void PublishLocalNotification(string title, string text)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("title", title); // Passed parameters to MainActivity.cs
            intent.PutExtra("text", text);
            intent.AddFlags(ActivityFlags.ClearTop);

            var pendingIntent = PendingIntent.GetActivity(this, 1, intent, PendingIntentFlags.Immutable);

            // Instantiate the builder and set notification elements:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "90")
                .SetContentTitle(title)
                .SetContentText(text)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetSmallIcon(Resource.Drawable.ic_notification);


            // Build the notification:
            Notification notification = builder.Build();
            notification.Defaults |= NotificationDefaults.Vibrate;
            notification.Defaults |= NotificationDefaults.Sound;


            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
        }

    }
}

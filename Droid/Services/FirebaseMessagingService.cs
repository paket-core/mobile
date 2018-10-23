using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;

using Firebase.Messaging;
using PaketGlobal;
using PaketGlobal.Droid;
using Xamarin.Forms;

namespace FCMNotifications
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class DroidFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";

        public override void OnMessageReceived(RemoteMessage message)
        {
            try{
                Log.Debug(TAG, "From: " + message.From);

                var body = message.GetNotification().Body;
                var title = message.GetNotification().Title;

                if (title == null)
                {
                    title = "";
                }

                if (body == null)
                {
                    body = "";
                }

                Log.Debug(TAG, "Notification Message Body: " + body);
                SendNotification(title, body, message.Data);
            }
            catch (Exception ex){
                Console.WriteLine(ex);
            }
        }

        void SendNotification(string title, string messageBody, IDictionary<string, string> data)
        {
            if(MainActivity.IsActive)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Xamarin.Forms.MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.REFRESH_PACKAGES, "");
                });
            }

            try{
                App.Locator.NotificationService.ShowPackageStringNotification(title, messageBody, DidClickNotification);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(this, MainActivity.NOTIFICATION_ID, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                                            .SetSmallIcon(Resource.Drawable.ic_notification)
                                      .SetContentTitle(title)
                                      .SetContentText(messageBody)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }

        private void DidClickNotification(string obj)
        {
            if(obj==null)
            {
                obj = "";
            }
            Xamarin.Forms.MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.CLICK_PACKAGE_NOTIFICATION, obj);
        }

    }
}

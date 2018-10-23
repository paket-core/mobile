using System;
using Android.App;
using Firebase.Iid;
using Android.Util;
using PaketGlobal;
using PaketGlobal.Droid;

namespace FCMClient
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            App.Locator.DeviceService.FCMToken = refreshedToken;

            SendRegistrationToServer(refreshedToken);
        }

        void SendRegistrationToServer(string token)
        {
            MainActivity.SendRegistrationToServer(token);
        }
    }
}
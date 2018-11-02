using System;
using System.Collections.Generic;
using System.Net.Http;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Com.Facebook.Common.References;
using Newtonsoft.Json;

namespace PaketGlobal.Droid
{

    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class EventJob : JobService
    {
        public static readonly string TAG = typeof(EventJob).FullName;

        CalculatorTask calculator;

        JobParameters parameters;

        public EventJob()
        {
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override bool OnStartJob(JobParameters @params)
        {
            parameters = @params;
            calculator = new CalculatorTask(this);

            calculator.Execute();

            return false; // No more work to do!
        }

        public override bool OnStopJob(JobParameters @params)
        {
            Log.Debug(TAG, "System halted the job.");
  
            return true; 
        }
        

        class CalculatorTask : AsyncTask<long, Java.Lang.Void, long>
        {
            readonly EventJob jobService;

            public CalculatorTask(EventJob jobService)
            {
                this.jobService = jobService;
            }

            async System.Threading.Tasks.Task<long> TrySendEventsAndPackages()
            {
                await SendEventsAndCheckPackages();

                return 0;
            }

            HttpClient CreateClient(string pubKey)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Pubkey", pubKey);
                client.DefaultRequestHeaders.Add("Fingerprint", "");
                client.DefaultRequestHeaders.Add("Signature", "");

                return client;
            }

            async System.Threading.Tasks.Task<bool> SendEvent(string pubKey)
            {
              
                var location = "";
                var eventName = Constants.EVENT_APP_USED;

                var locationManager = Android.App.Application.Context.GetSystemService(Context.LocationService) as LocationManager;
                var criteria = new Criteria { PowerRequirement = Power.Medium };
                var bestProvider = locationManager.GetBestProvider(criteria,true);
                var lastLocation = locationManager.GetLastKnownLocation(bestProvider);

                if(lastLocation!=null)
                {
                    location = lastLocation.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + lastLocation.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    if (location.Length > 24)
                    {
                        location = location.Substring(0, 24);
                    }
                }

                string url = Config.RouteServerUrl + "/" + Config.RouteServerVersion + "/add_event";

                var locator = Plugin.Geolocator.CrossGeolocator.Current;

                if(string.IsNullOrEmpty(location))
                {
                    var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5));

                    if (position != null)
                    {
                        location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        if (location.Length > 24)
                        {
                            location = location.Substring(0, 24);
                        }
                    }

                    if (position == null)
                    {
                        position = await locator.GetLastKnownLocationAsync();

                        if (position != null)
                        {
                            location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                            if (location.Length > 24)
                            {
                                location = location.Substring(0, 24);
                            }
                        }
                    }
                }           

                var formContent = new FormUrlEncodedContent(new[]{
                            new KeyValuePair<string, string>("event_type", eventName),
                            new KeyValuePair<string, string>("location", location),
                        });


                var client = CreateClient(pubKey);

                await client.PostAsync(url, formContent);

                return true;
            }

            private bool IsPackageExpiredNeedShow(Package package, string pubKey)
            {
                var keyExpired = package.PaketId + "_expired_" + pubKey;

                if (package.IsExpired)
                {
                    if (package.DeadlineDT.AddHours(1) > DateTime.Now.ToLocalTime())
                    {
                        ISharedPreferences preferences = Application.Context.GetSharedPreferences("DeliverIt_UserInfo", FileCreationMode.Private);

                        string expiredResult = preferences.GetString(keyExpired, "");

                        if (expiredResult.Length>0)
                        {
                            return false;
                        }
                        else
                        {
                            ISharedPreferencesEditor edit = preferences.Edit();
                            edit.PutString(keyExpired, keyExpired);
                            edit.Apply();

                            return true;
                        }
                    }
                }

                return false;
            }

            async System.Threading.Tasks.Task<bool> CheckPackages(string pubKey)
            {
                string url = Config.RouteServerUrl + "/" + Config.RouteServerVersion + "/my_packages";

                var client = CreateClient(pubKey);

                var response = await client.PostAsync(url, null);

                var content = await response.Content.ReadAsStringAsync();

                var resultPackages = JsonConvert.DeserializeObject<PackagesData>(content);

                if (resultPackages != null && resultPackages.Packages != null)
                {
                    var packages = resultPackages.Packages;

                    foreach (Package p1 in packages)
                    {
                        var isExpiredNeedShow = this.IsPackageExpiredNeedShow(p1, pubKey);

                        if (isExpiredNeedShow)
                        {
                            var text = "Your package " + p1.ShortEscrow + " expired";
                            PublishLocalNotification(text, "Please check your Packages archive for more details");
                        }
                    }
                }

                return true;
            }

            async System.Threading.Tasks.Task<long> SendEventsAndCheckPackages()
            {
                //disable doze
                try
                {
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    ISharedPreferences preferences = Application.Context.GetSharedPreferences("DeliverIt_UserInfo", FileCreationMode.Private);
                   
                    string pubKey = preferences.GetString("pubkey", "");

                    if (pubKey.Length > 0)
                    {
                        await SendEvent(pubKey);
                        
                        await CheckPackages(pubKey);
                    }
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return 0;
            }

            protected override long RunInBackground(params long[] @params)
            {
                return TrySendEventsAndPackages().Result;
            }


            protected override void OnPostExecute(long result)
            {
                base.OnPostExecute(result);
                
                jobService.JobFinished(jobService.parameters, false);
            }


            private void PublishLocalNotification(string title, string text)
            {
                var channelId = "90";
                var channelName = "job service";

                if(Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var chan = new NotificationChannel(channelId, channelName, NotificationImportance.High);
                    chan.LightColor = Android.Graphics.Color.Blue;
                    chan.LockscreenVisibility = NotificationVisibility.Public;

                    var service = Android.App.Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                    service.CreateNotificationChannel(chan);
                }


                var intent = new Intent(Android.App.Application.Context, typeof(MainActivity));
                intent.PutExtra("title", title);
                intent.PutExtra("text", text);
                intent.AddFlags(ActivityFlags.ClearTop);

                var pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, 1, intent, PendingIntentFlags.Immutable);

                // Instantiate the builder and set notification elements:
                NotificationCompat.Builder builder = new NotificationCompat.Builder(Android.App.Application.Context, channelId)
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
                    Android.App.Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

                // Publish the notification:
                const int notificationId = 0;
                notificationManager.Notify(notificationId, notification);
            }

        }
    }
}